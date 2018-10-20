using Microsoft.Azure.CognitiveServices.Vision;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Rest;

namespace ServiceHelpers
{
    public static class VisionServiceHelper
    {
        public static int RetryCountOnQuotaLimitError = 6;
        public static int RetryDelayOnQuotaLimitError = 500;

        private static ComputerVisionClient visionClient { get; set; }

        static VisionServiceHelper()
        {
            InitializeVisionService();
        }
        

        private static string apiKey;
        public static string ApiKey
        {
            get
            {
                return apiKey;
            }

            set
            {
                var changed = apiKey != value;
                apiKey = value;
                if (changed)
                {
                    InitializeVisionService();
                }
            }
        }

        private static string endpoint;
        public static string Endpoint
        {
            get
            {
                return endpoint;
            }

            set
            {
                var changed = endpoint != value;
                endpoint = value;
                if (changed)
                {
                    InitializeVisionService();
                }
            }
        }

        private static void InitializeVisionService()
        {
            visionClient = new ComputerVisionClient(new ApiKeyServiceClientCredentials(apiKey));
            visionClient.Endpoint = Endpoint;
        }

        // handle throttling issues
        private static async Task<HttpOperationResponse<ImageAnalysis>> 
            RunTaskWithAutoRetryOnQuotaLimitExceededError(Func<Task<HttpOperationResponse<ImageAnalysis>>> action)
        {
            int retriesLeft = RetryCountOnQuotaLimitError;
            int delay = RetryDelayOnQuotaLimitError;

            HttpOperationResponse<ImageAnalysis> response = null;
            while (true)
            {
                response = await action();
                if (response.Response.StatusCode == (System.Net.HttpStatusCode) 429 && retriesLeft > 0)
                {
                    Debug.WriteLine("Vision API throttling error");

                    await Task.Delay(delay);
                    retriesLeft--;
                    delay *= 2;
                    continue;
                }
                else
                {
                    break;
                }
            }

            return response;
        }
        
        public static async Task<HttpOperationResponse<ImageAnalysis>> AnalyzeImageAsync(Func<Task<Stream>> imageStreamCallback, 
            IList<VisualFeatureTypes> visualFeatures = null)
        {
            return await RunTaskWithAutoRetryOnQuotaLimitExceededError(
                async () =>
                {
                    var imgStream = await imageStreamCallback();
                    return await visionClient.AnalyzeImageInStreamWithHttpMessagesAsync(
                        imgStream, visualFeatures);
                });
        }
        
    }
}

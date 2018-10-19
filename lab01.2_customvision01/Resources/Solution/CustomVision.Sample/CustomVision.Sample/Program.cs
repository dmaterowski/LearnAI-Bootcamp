using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Cognitive.CustomVision.Prediction;
using Microsoft.Cognitive.CustomVision.Training;
using Microsoft.Cognitive.CustomVision.Training.Models;

namespace CustomVision.Sample
{
    class Program
    {
        private static List<MemoryStream> hemlockImages;

        private static List<MemoryStream> japaneseCherryImages;

        private static MemoryStream testImage;

        static void Main(string[] args)
        {
            // You can either add your training key here, pass it on the command line, or type it in when the program runs
            string trainingKey = "";

            // Create the Api, passing in a credentials object that contains the training key
            //TrainingApiCredentials trainingCredentials = new TrainingApiCredentials(trainingKey);
            TrainingApi trainingApi = new TrainingApi();
            trainingApi.ApiKey = trainingKey;

            // Create a new project  
            Console.WriteLine("Creating new project:");
            var project = trainingApi.CreateProject("My First Project");

            // Make two tags in the new project
            var hemlockTag = trainingApi.CreateTag(project.Id, "Hemlock");
            var japaneseCherryTag = trainingApi.CreateTag(project.Id, "Japanese Cherry");

            // Add some images to the tags  
            Console.WriteLine("\\tUploading images");
            LoadImagesFromDisk();

            // Images can be uploaded one at a time  
            foreach (var image in hemlockImages)
            {
                trainingApi.CreateImagesFromData(project.Id, image, new List<string>() {hemlockTag.Id.ToString()});
            }

            var entries = new List<ImageFileCreateEntry>();
            int counter = 0;
            foreach (var image in japaneseCherryImages)
            {
                var bytes = ReadFully(image);
                entries.Add(new ImageFileCreateEntry($"japaneseCherry{counter++}", bytes));
            }

            // Or uploaded in a single batch   
            ImageFileCreateBatch batch = new ImageFileCreateBatch(entries, new List<Guid>() {japaneseCherryTag.Id});
            trainingApi.CreateImagesFromFilesWithHttpMessagesAsync(project.Id, batch).Wait();

            // Now there are images with tags start training the project  
            Console.WriteLine("\\tTraining");
            var iteration = trainingApi.TrainProject(project.Id);

            // The returned iteration will be in progress, and can be queried periodically to see when it has completed  
            while (iteration.Status == "Training")
            {
                Thread.Sleep(1000);

                // Re-query the iteration to get it's updated status  
                iteration = trainingApi.GetIteration(project.Id, iteration.Id);
            }

            // The iteration is now trained. Make it the default project endpoint  
            iteration.IsDefault = true;
            trainingApi.UpdateIteration(project.Id, iteration.Id, iteration);
            Console.WriteLine("Done!\n");

            // Now there is a trained endpoint, it can be used to make a prediction  

            // Add your prediction key from the settings page of the portal 
            // The prediction key is used in place of the training key when making predictions 
            string predictionKey = "";

            // Create a prediction endpoint, passing in a prediction credentials object that contains the obtained prediction key  
            //PredictionEndpointCredentials predictionEndpointCredentials = new PredictionEndpointCredentials(predictionKey);
            PredictionEndpoint endpoint = new PredictionEndpoint();
            endpoint.ApiKey = predictionKey;
            // Make a prediction against the new project  
            Console.WriteLine("Making a prediction:");
            var result = endpoint.PredictImage(project.Id, testImage);

            // Loop over each prediction and write out the results  
            foreach (var c in result.Predictions)
            {
                Console.WriteLine($"\t{c.Tag}: {c.Probability:P1}");
            }

            Console.ReadKey();

        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }

                return ms.ToArray();
            }
        }

        private static void LoadImagesFromDisk()
        {
            // this loads the images to be uploaded from disk into memory
            hemlockImages = Directory.GetFiles(@"..\..\..\..\..\Images\Hemlock")
                .Select(f => new MemoryStream(File.ReadAllBytes(f))).ToList();
            japaneseCherryImages = Directory.GetFiles(@"..\..\..\..\..\Images\Japanese Cherry")
                .Select(f => new MemoryStream(File.ReadAllBytes(f))).ToList();
            testImage = new MemoryStream(File.ReadAllBytes(@"..\..\..\..\..\Images\Test\test_image.jpg"));

        }
    }

}
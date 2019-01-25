using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;

namespace CustomVision.Sample
{
    class Program
    {
        private static List<MemoryStream> hemlockImages;

        private static List<string> japaneseCherryImages;

        private static MemoryStream testImage;

        static void Main(string[] args)
        {
            string trainingKey =
                "<INSERT YOUR TRAINING KEY HERE>";
            string trainingEndpoint = "<TRAINING URL>";

            // Create the Api
            CustomVisionTrainingClient trainingApi = new CustomVisionTrainingClient()
            {
                ApiKey = trainingKey,
                Endpoint = trainingEndpoint
            };

            // Create a new project  
            Console.WriteLine("Creating new project:");
            Project project = trainingApi.CreateProject("project-cherry-hemlock");

            //TODO#2: Make two tags in the new project
            Tag hemlockTag = trainingApi.CreateTag(project.Id, "Hemlock"); //Create Tag "Hemlock";
            Tag japaneseCherryTag = trainingApi.CreateTag(project.Id, "Japanese Cherry"); //Create Tag "Japanese Cherry"

            // Add some images to the tags  
            Console.WriteLine("\tUploading images");
            LoadImagesFromDisk();

            //TODO#3: Images can be uploaded one at a time  
            foreach (var image in hemlockImages)
            {
                trainingApi.CreateImagesFromData(project.Id, image, new List<string>() {hemlockTag.Id.ToString()});
            }

            //TODO#4: Or uploaded in a single batch   
            var imageFiles = japaneseCherryImages
                .Select(img => new ImageFileCreateEntry(Path.GetFileName(img), File.ReadAllBytes(img))).ToList();
            trainingApi.CreateImagesFromFiles(project.Id,
                new ImageFileCreateBatch(imageFiles, new List<Guid>() {japaneseCherryTag.Id}));

            // Now there are images with tags start training the project  
            Console.WriteLine("\tTraining");
            //TODO#5: replace '_' below with a call to training method
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
            string predictionKey = "<PREDICTION KEY>";
            string predictionEndpoint = "<PREDICTION URL>";

            // Create a prediction endpoint
            CustomVisionPredictionClient endpoint = new CustomVisionPredictionClient()
            {
                ApiKey = predictionKey,
                Endpoint = predictionEndpoint
            };
            // Make a prediction against the new project  
            Console.WriteLine("Making a prediction:");
            var result = endpoint.PredictImage(project.Id, testImage);

            // Loop over each prediction and write out the results  
            foreach (var c in result.Predictions)
            {
                Console.WriteLine($"Tag: {c.TagName} - Probability:{c.Probability}");
            }

            Console.ReadKey();

        }

        private static void LoadImagesFromDisk()
        {
            // this loads the images to be uploaded from disk into memory
            hemlockImages = Directory.GetFiles(@"..\..\..\..\..\..\Images\Hemlock")
                .Select(f => new MemoryStream(File.ReadAllBytes(f))).ToList();
            japaneseCherryImages = Directory.GetFiles(@"..\..\..\..\..\..\Images\Japanese Cherry").ToList();
            testImage = new MemoryStream(File.ReadAllBytes(@"..\..\..\..\..\..\Images\Test\test_image.jpg"));

        }
    }
}
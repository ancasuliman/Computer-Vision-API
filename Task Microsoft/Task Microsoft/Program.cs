using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using System.IO;

namespace Task_Microsoft
{
    class Program
    {
        private static string link = "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0";
        private static string key = "788fbaf348834e44ae284bddf7cfa25a";

        static private async Task<AnalysisResult> UploadAndAnalyzeImage(string imageFilePath)
        {
            VisionServiceClient VisionServiceClient = new VisionServiceClient(key, link);
            Console.WriteLine("VisionServiceClient is created");

            using (Stream imageFileStream = File.OpenRead(imageFilePath))
            {
                Console.WriteLine("Calling VisionServiceClient.AnalyzeImageAsync()...");
                VisualFeature[] visualFeatures = new VisualFeature[] { VisualFeature.Adult, VisualFeature.Categories, VisualFeature.Color, VisualFeature.Description, VisualFeature.Faces, VisualFeature.ImageType, VisualFeature.Tags };
                AnalysisResult analysisResult = await VisionServiceClient.GetTagsAsync(imageFileStream);
                return analysisResult;
            }

        }
        static void Main(string[] args)
        {   
            //Task 3
            Task.Run(async () =>
            {
                AnalysisResult task = await UploadAndAnalyzeImage("9.png");
                for (int i = 0; i < task.Tags.Length; ++i)
                    Console.WriteLine("Name " + task.Tags[i].Name + "\nConfidence " + task.Tags[i].Confidence);
            }).GetAwaiter().GetResult();

            //Task 4
            string[] files = Directory.GetFiles(".");
            Directory.CreateDirectory("Tagless");
            foreach (string fileName in files)
            {
                Task.Run(async () =>
                {
                    AnalysisResult task = await UploadAndAnalyzeImage(fileName);
                    Console.WriteLine(fileName);
                    if (task.Tags.Length == 0)
                        Console.WriteLine("No Tags");
                    else
                        Console.WriteLine("It has tags");
                    if (task.Tags.Length == 0)
                    {
                        String copy = fileName;
                        String[] split = copy.Split(new char[] { '\\' });
                        File.Copy(fileName, "Tagless" + "\\" + split[1], true);
                    }
                    else
                    {
                        Double max = task.Tags[0].Confidence;
                        int max_index = 0;
                        for (int i = 0; i < task.Tags.Length; ++i)
                            if (max.CompareTo(task.Tags[i].Confidence) >= 0)
                            {
                                max = task.Tags[i].Confidence;
                                max_index = i;
                            }
                        Console.WriteLine(task.Tags[max_index].Name + " " + max_index + " " + task.Tags[max_index].Confidence);
                        String new_folder = task.Tags[max_index].Name;
                        String copy = fileName;
                        Directory.CreateDirectory(new_folder);
                        String[] split = copy.Split(new char[] { '\\' });
                        File.Copy(fileName, new_folder + "\\" + split[1], true);

                    }
                }).GetAwaiter().GetResult();
            }
        }
    }
}

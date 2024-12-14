using System;
using System.IO;
using System.Threading.Tasks;
using Astator.Script;
using Compunet.YoloSharp;

namespace _07.YOLO调用
{
    public class Program
    {
        private static string workDir = string.Empty;

        [EntryMethod]
        public static async Task MainAsync(string workDir)
        {
            Console.WriteLine("Loading detection model...");
            using var detectPredictor = new YoloPredictor(Path.Combine(workDir, "assets", "./models/yolo11n.onnx"));


            var img = Path.Combine(workDir, "assets", "images", "sports.jpg");
            var result = await detectPredictor.DetectAsync(img);


            for (var i = 0; i < result.Count; i++)
            {
                Console.WriteLine($"{result[i].Name}: {result[i].Bounds}");
            }
        }
    }
}

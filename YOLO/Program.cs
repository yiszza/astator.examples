using System;
using System.IO;
using System.Threading.Tasks;
using Astator.Script;
using Compunet.YoloSharp;
using Compunet.YoloSharp.Plotting;

namespace YOLO;

public class Program
{
    [EntryMethod]
    public static async Task<object> MainAsync(string workDir)
    {
        Console.WriteLine("Loading detection model...");
        using var detectPredictor = new YoloPredictor(
            Path.Combine(workDir, "assets", "yolo11n.onnx")
        );

        var img = Path.Combine(workDir, "assets", "sports.jpg");

        var result = await detectPredictor.DetectAsync(img);
        // var result1 = await detectPredictor.DetectAndSaveAsync(
        //    img,
        //    Path.Combine(workDir, "assets", "sports_detect.png")
        // );

        for (var i = 0; i < result.Count; i++)
        {
            Console.WriteLine($"{result[i].Name}: {result[i].Bounds}");
        }

        return 0;
    }
}

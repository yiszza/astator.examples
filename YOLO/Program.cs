using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Astator.Graphics;
using Astator.Script;
using YoloDotNet;
using YoloDotNet.ExecutionProvider.Cpu;
using YoloDotNet.Extensions;
using YoloDotNet.Models;

namespace YOLO;

public class Program
{
    [EntryMethod]
    public static async Task MainAsync(string workDir)
    {
        try
        {
            Console.WriteLine("Loading detection model...");

            using var yolo = new Yolo(
                new YoloOptions
                {
                    ExecutionProvider = new CpuExecutionProvider(
                        Path.Combine(workDir, "assets", "yolo11n.onnx")
                    ),
                }
            );

            var imgPath = Path.Combine(workDir, "assets", "sports.jpg");
            var image = WrapImage.CreateFromFile(imgPath);
            using var bitmap = image.GetBitmap();

            var sw = Stopwatch.StartNew();

            var result = yolo.RunObjectDetection(bitmap, confidence: 0.25, iou: 0.7);

            sw.Stop();

            for (var i = 0; i < result.Count; i++)
            {
                Console.WriteLine($"{result[i].Label}: {result[i].BoundingBox}");
            }

            Console.WriteLine($"识别耗时: {sw.ElapsedMilliseconds} ms");

            bitmap.Draw(result);
            bitmap.Save(Path.Combine(workDir, "assets", "sports_out.jpg"));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}

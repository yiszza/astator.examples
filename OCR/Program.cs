using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Astator.Graphics;
using Astator.Script;
using Sdcb.SimdPaddleOCR;
using SkiaSharp;

namespace OCR;

public class Program
{
    [EntryMethod]
    public static async Task MainAsync(string workDir)
    {
        try
        {
            var clsPath = Path.Combine(
                workDir,
                "assets",
                "ch_PP-LCNet_x0_25_textline_ori_cls_mobile.onnx"
            );

            var detPath = Path.Combine(workDir, "assets", "PP-OCRv6_det_tiny.onnx");
            var recPath = Path.Combine(workDir, "assets", "PP-OCRv6_rec_tiny.onnx");
            var keysPath = Path.Combine(workDir, "assets", "ppocrv6_tiny_dict.txt");

            using PaddleOcrAll ocr = await PaddleOcrAll.LoadAsync(
                detectionPath: detPath,
                classificationPath: clsPath,
                recognitionPath: recPath,
                dictionaryPath: keysPath
            );

            var imgPath = Path.Combine(workDir, "assets", "test.png");
            var image = WrapImage.CreateFromFile(imgPath);

            var sw = Stopwatch.StartNew();

            PaddleOcrResult result = ocr.Run(
                image.Data,
                image.Width,
                image.Height,
                image.RowStride,
                ImagePixelFormat.Rgba32
            );

            sw.Stop();
            Console.WriteLine(result.Text);
            Console.WriteLine($"识别耗时: {sw.ElapsedMilliseconds} ms");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}

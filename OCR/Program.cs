using System;
using System.IO;
using Astator.Graphics;
using Astator.Script;
using RapidOcrNet;
using SkiaSharp;

namespace OCR;

public class Program
{
    [EntryMethod]
    public static void Main(string workDir)
    {
        try
        {
            var clsPath = Path.Combine(
                workDir,
                "assets",
                "ch_PP-LCNet_x0_25_textline_ori_cls_mobile.onnx"
            );

            // var detPath = Path.Combine(workDir, "assets", "PP-OCRv6_det_tiny.onnx");
            // var recPath = Path.Combine(workDir, "assets", "PP-OCRv6_rec_tiny.onnx");

            // var keysPath = Path.Combine(workDir, "assets", "ppocrv6_tiny_dict.txt");

            // using var ocr = new RapidOcr();

            // var v6 = RapidOcrModelSet.PPOCRv6Tiny with
            // {
            //     ClsModelPath = clsPath,
            //     DetModelPath = detPath,
            //     RecModelPath = recPath,
            //     KeysPath = keysPath,
            // };
            // ocr.InitModels(v6);

            var detPath = Path.Combine(workDir, "assets", "PP-OCRv6_det_small.onnx");
            var recPath = Path.Combine(workDir, "assets", "PP-OCRv6_rec_small.onnx");

            var keysPath = Path.Combine(workDir, "assets", "ppocrv6_dict.txt");

            using var ocr = new RapidOcr();

            var v6 = RapidOcrModelSet.PPOCRv6Small with
            {
                ClsModelPath = clsPath,
                DetModelPath = detPath,
                RecModelPath = recPath,
                KeysPath = keysPath,
            };
            ocr.InitModels(v6);

            var testPngPath = Path.Combine(workDir, "assets", "test.png");
            var testImg = SKBitmap.Decode(testPngPath);
            var result = ocr.Detect(testImg, RapidOcrOptions.Default);

            Console.WriteLine(result.StrRes);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}

using System;
using System.IO;
using Astator.Graphics;
using Astator.Script;
using Astator.ThirdParty.RapidOCR;

namespace OCR;

public class Program
{
    [EntryMethod]
    public static void Main(string workDir)
    {
        try
        {
            var clsPath = Path.Combine(workDir, "assets", "ch_ppocr_mobile_v2.0_cls_infer.onnx");
            //var detPath = Path.Combine(workDir, "assets", "ch_PP-OCRv5_mobile_det.onnx");
            //var recPath = Path.Combine(workDir, "assets", "ch_PP-OCRv5_rec_mobile_infer.onnx");

            var detPath = Path.Combine(workDir, "assets", "ch_PP-OCRv4_det_infer.onnx");
            var recPath = Path.Combine(workDir, "assets", "ch_PP-OCRv4_rec_infer.onnx");

            var keysPath = Path.Combine(workDir, "assets", "ppocr_keys_v1.txt");
            var testPngPath = Path.Combine(workDir, "assets", "test.png");

            var ocr = new RapidOCRHelper(detPath, clsPath, recPath, keysPath);

            var testImg = WrapImage.CreateFromFile(testPngPath);
            var result = ocr.Detect(testImg);

            Console.WriteLine(result.StrRes);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}

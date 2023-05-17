using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SharpZebra;
using SharpZebra.Printing;
using UnityEngine;
using Zen.Barcode;
using Graphics = System.Drawing.Graphics;

public static class LabelMaker
{
    public static byte[] ConvertTexture2DToByteArray(Texture2D texture)
    {
        byte[] imageData = texture.EncodeToPNG();
        return imageData;
    }

    public static Texture2D GenerateQRFromGoogleAPI(string text)
    {
        string url = $"https://chart.googleapis.com/chart?cht=qr&chs=256x256&chl={text}";
        using (var webClient = new WebClient())
        {
            byte[] imageData = webClient.DownloadData(url);
            Texture2D texture = new Texture2D(256, 256);
            texture.LoadImage(imageData);
            return texture;
        }
    }
    public static string SendZPLCommandToUSBPrinter(string zplCommand)
    {
        PrinterSettings x = new PrinterSettings();
        x.PrinterName = "Zebra";
        x.PrinterPort = 0;
       
        IZebraPrinter printer = new USBPrinter(x);
        printer.Settings = x;
        byte[] y = Encoding.ASCII.GetBytes(zplCommand);
        printer.Print(y);

        return zplCommand;
    }
    public static Texture2D GenerateUPCABarcodeTexture(string barcodeValue, int width = 300, int height = 150)
    {
        var barcode = BarcodeDrawFactory.GetSymbology(BarcodeSymbology.Code128);
        Image barcodeImage = barcode.Draw(barcodeValue, width, height);

        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color32[] barcodeColors = new Color32[width * height];

        using (Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb))
        {
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.DrawImage(barcodeImage, 0, 0, width, height);
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    System.Drawing.Color color = bitmap.GetPixel(x, y);
                    barcodeColors[x + y * width] = new Color32(color.R, color.G, color.B, color.A);
                }
            }
        }

        texture.SetPixels32(barcodeColors);
        texture.Apply();

        return texture;
    }
    public static async Task SendTexture2DToUSBPrinterAsync(Texture2D texture)
    {
        string zplCommand = CreateZPLCommand(texture);
        Debug.LogError(texture.width);
        SendZPLCommandToUSBPrinter(zplCommand);
    }

    public static string CreateZPLCommand(Texture2D texture)
    {
        byte[] imageData = ConvertTexture2DToByteArray(texture);
        ZplImageConverter zplConverter = new ZplImageConverter();
        Image image = null;
        using (var ms = new MemoryStream(imageData))
        {
            image = Image.FromStream(ms);
            zplConverter = new ZplImageConverter(1200, 800);
        }
        string zplCommand = zplConverter.BuildLabel(image);
        return zplCommand;
    }
}

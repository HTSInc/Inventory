using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SharpZebra.Printing;
using UnityEngine;

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

    public static void SendZPLCommandToUSBPrinter(string zplCommand)
    {
        PrinterSettings x = new PrinterSettings();
        x.PrinterName = "Zebra";
        x.PrinterPort = 0;
        IZebraPrinter printer = new USBPrinter(x);
        // Convert the zplCommand string to a byte array
        RawPrinterHelper.SendStringToPrinter("Zebra", zplCommand);
    }

    public static async Task SendTexture2DToUSBPrinterAsync(Texture2D texture, int x = 0, int y = 0)
    {
        string zplCommand = CreateZPLCommand(texture, x, y);
        Debug.LogError(texture.width);
        SendZPLCommandToUSBPrinter(zplCommand);
    }

    public static string CreateZPLCommand(Texture2D texture, int x, int y)
    {
        byte[] imageData = ConvertTexture2DToByteArray(texture);
        using (var ms = new MemoryStream(imageData))
        {
            Image image = Image.FromStream(ms);
            ZplImageConverter zplConverter = new ZplImageConverter(texture.width, texture.height);
            string zplCommand = zplConverter.BuildLabel(image);
            return zplCommand;
        }
    }
}

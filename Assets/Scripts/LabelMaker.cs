using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Net.Sockets;
using System.IO;
using QRCoder;
using UnityQRCode = QRCoder.Unity.UnityQRCode;
using System.Threading.Tasks;
using System;
using Object = UnityEngine.Object;

public class LabelMaker
{
    public const int Width = 1200;
    public const int Height = 1800;
    public Texture2D LabelTexture { get; private set; }
    private Camera labelMakerCamera;
    public LabelMaker()
    {
        LabelTexture = new Texture2D(Width, Height, TextureFormat.RGBA32, false);
        // Create a separate camera for the LabelMaker
        labelMakerCamera = GameObject.Find("LabelMaker Camera").GetComponent<Camera>();
        labelMakerCamera.clearFlags = CameraClearFlags.Color;
        labelMakerCamera.backgroundColor = Color.clear;
        labelMakerCamera.enabled = false;
        labelMakerCamera.transform.position = new Vector3(0, 0, -1);
        labelMakerCamera.orthographic = true;
    }
    public Texture2D GenerateLabelTexture(string labelText, List<StringTexturePair> iconTextPairs, Texture2D qrCodeImage, int labelWidth, int labelHeight)
    {
        ClearTexture(labelWidth, labelHeight);

        string[] lines = labelText.Split('\n');
        int lineHeight = 50; // Change this value to adjust the space between lines of text
        int fontSize = 24; // Change this value to adjust the font size of the text
        Debug.LogError("AddString selected.");
        for (int i = 0; i < lines.Length; i++)
        {
            Vector2 position = new Vector2(labelWidth / 2, labelHeight - (i + 1) * lineHeight);
            Debug.LogError("AddString " + i);
            Debug.Log(i);
            AddString(lines[i], position, fontSize, Color.black);
        }
        Debug.LogError("AddString Done.");
        Vector2[] iconPositions = CalculateIconPositions(iconTextPairs.Count, labelWidth, labelHeight);
        Vector2 qrCodePosition = CalculateQRCodePosition(labelWidth, labelHeight);
        for (int i = 0; i < iconTextPairs.Count; i++)
        {
            AddTexture(iconTextPairs[i].Icon, iconPositions[i], new Vector2Int(64, 64));
        }

        AddTexture(qrCodeImage, qrCodePosition, new Vector2Int(128, 128));
        return LabelTexture;
    }
    private Vector2[] CalculateIconPositions(int iconCount, int labelWidth, int labelHeight)
    {
        int iconSize = 64;
        int iconSpacing = 16;
        int totalWidth = iconSize * iconCount + iconSpacing * (iconCount - 1);
        int startX = (labelWidth - totalWidth) / 2;
        int iconY = labelHeight / 2; // Change this value to adjust the vertical position of the icons
        Vector2[] positions = new Vector2[iconCount];
        for (int i = 0; i < iconCount; i++)
        {
            positions[i] = new Vector2(startX + (iconSize + iconSpacing) * i, iconY);
        }
        return positions;
    }

    private Vector2 CalculateQRCodePosition(int labelWidth, int labelHeight)
    {
        int qrCodeSize = 128;
        int qrCodeX = (labelWidth - qrCodeSize) / 2;
        int qrCodeY = labelHeight / 4; // Change this value to adjust the vertical position of the QR code
        return new Vector2(qrCodeX, qrCodeY);
    }

    public string CreateZPLTexture(Texture2D texture, int x, int y, int width, int height)
    {
        string zplHexData = ConvertTextureToZPLHex(texture);
        string zplCommand = CreateZPLCommandString(zplHexData, x, y, width, height);
        return zplCommand;
    }
    public void ClearTexture(int labelWidth, int labelHeight)
    {
        Color clearColor = Color.white;
        for (int y = 0; y < labelHeight; y++)
        {
            for (int x = 0; x < labelWidth; x++)
            {
                LabelTexture.SetPixel(x, y, clearColor);
            }
        }
        LabelTexture.Apply();
    }
    public Texture2D GenerateQRCode(string text)
    {
        int width = 256;
        int height = 256;
        Debug.LogError("qrGenerator before.");
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
        UnityQRCode qrCode = new UnityQRCode(qrCodeData);
        Debug.LogError("texture before.");
        Texture2D texture = null;
        try
        {
            texture = qrCode.GetGraphic(1, Color.black, Color.white);
        }
        catch (Exception ex)
        {
            Debug.LogError("Exception: " + ex.Message);
        }
        Debug.LogError("texture after.");

        Debug.LogError("Width: " + width);
        Debug.LogError("Height: " + height);
        Debug.LogError("return " + texture);
        return texture;
    }




    public void AddString(string text, Vector2 position, int fontSize, Color textColor)
    {
        GameObject textMeshProParent = new GameObject("TextMeshProParent");
        Canvas canvas = AppManager.can;

        // Add TextMeshProUGUI component
        TextMeshProUGUI tmp = textMeshProParent.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.color = textColor;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.enableAutoSizing = true;
        tmp.ForceMeshUpdate();
        Bounds bounds = tmp.bounds;

        textMeshProParent.transform.SetParent(canvas.transform);
        textMeshProParent.transform.position = Vector3.zero;
        textMeshProParent.transform.rotation = Quaternion.identity;

        RenderTexture renderTexture = new RenderTexture((int)bounds.size.x, (int)bounds.size.y, 0);
        renderTexture.antiAliasing = 8;

        labelMakerCamera.targetTexture = renderTexture;
        labelMakerCamera.Render();

        RenderTexture.active = renderTexture;
        Texture2D textTexture = new Texture2D((int)bounds.size.x, (int)bounds.size.y, TextureFormat.RGBA32, false);
        textTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        textTexture.Apply();
        RenderTexture.active = null;

        Texture2DUtility.AddTexture(LabelTexture, textTexture, position);

        Object.Destroy(textMeshProParent);
    }


    public void AddTexture(Texture2D texture, Vector2 position, Vector2Int size)
    {
        Texture2D resizedTexture = Texture2DUtility.ResizeTexture(texture, size.x, size.y);
        Texture2DUtility.AddTexture(LabelTexture, resizedTexture, position);
    }

    public string ConvertTextureToZPLHex(Texture2D texture)
    {
        int width = texture.width;
        int height = texture.height;
        Texture2D monoTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color color = texture.GetPixel(x, y);
                float grayscale = color.grayscale;
                monoTexture.SetPixel(x, y, grayscale > 0.5f ? Color.white : Color.black);
            }
        }
        monoTexture.Apply();

        string hexData = "";
        int byteData = 0;
        int nibble = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                byteData = (byteData << 1) | (monoTexture.GetPixel(x, y).grayscale > 0.5f ? 0 : 1);
                nibble++;

                if (nibble == 8)
                {
                    hexData += byteData.ToString("X2");
                    byteData = 0;
                    nibble = 0;
                }
            }
        }

        return hexData;
    }

    public string CreateZPLCommandString(string zplHexData, int x, int y, int width, int height)
    {
        int bytesPerRow = width / 8;
        string zplCommand = $"^XA^FO{x},{y}^GFA,{width * height / 8},{width * height / 8},{bytesPerRow},{zplHexData}^FS^XZ";
        return zplCommand;
    }

    public async Task SendZPLCommandToPrinterAsync(string zplCommand, string printerName, string printerPort, bool isNetworkPrinter = false, string printerIPAddress = "", int printerNetworkPort = 9100)
    {
        if (isNetworkPrinter)
        {
            // Network-connected printer
            using (TcpClient client = new TcpClient(printerIPAddress, printerNetworkPort))
            using (StreamWriter writer = new StreamWriter(client.GetStream()))
            {
                writer.Write(zplCommand);
                writer.Flush();
            }
        }
        else
        {
            // USB-connected printer
            await Task.Run(() =>
            {
                RawPrinterHelper.SendStringToPrinter(printerName, zplCommand);
            });
        }
    }
}


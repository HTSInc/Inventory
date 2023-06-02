using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class PrintCam : MonoBehaviour
{
    public static Material QrTexture;
    public Material _QrTexture;
    public static Material BarTexture;
    public Material _BarTexture;
    public static TextMeshPro BarCode;
    public TextMeshPro _BarCode;
    public static TextMeshPro CompText;
    public TextMeshPro _CompText;
    public static TextMeshPro LabelText;
    public TextMeshPro _LabelText;
    public static TextMeshPro ProdText;
    public TextMeshPro _ProdText;
    public static TextMeshPro SubheadText;
    public TextMeshPro _SubheadText;
    public static TextMeshPro PkNText;
    public TextMeshPro _PkNText;
    public static Camera labelCam;
    public Camera _labelCam;
    public static RenderTexture renderTexture;
    public RenderTexture _renderTexture;

    public static TextMeshPro NameText;
    public TextMeshPro _NameText;
    public static TextMeshPro DateText;
    public TextMeshPro _DateText;
    public static TextMeshPro StatusText;
    public TextMeshPro _StatusText;
    public static TextMeshPro LotText;
    public TextMeshPro _LotText;

    public GameObject _deviceLabel;
    public static GameObject deviceLabel;
    public GameObject _inventoryLabel;
    public static GameObject inventoryLabel;

    private void Start()
    {
        deviceLabel = _deviceLabel;
        inventoryLabel = _inventoryLabel;
        deviceLabel.SetActive(true);
        inventoryLabel.SetActive(true);
        labelCam = _labelCam;
        renderTexture = _renderTexture;
        QrTexture = _QrTexture;
        BarTexture = _BarTexture;
        BarCode = _BarCode;
        CompText = _CompText;
        LabelText = _LabelText;
        ProdText = _ProdText;
        SubheadText = _SubheadText;
        PkNText = _PkNText;
        NameText = _NameText;
        DateText = _DateText;
        StatusText = _StatusText;
        LotText = _LotText;
    }
    public static async System.Threading.Tasks.Task<string> AssignLabel(Texture2D qr, Texture2D bar, string barCode, string companyText, string labelNameText, string productText, string subheadingText, int pkNum, int quantity)
    {
        deviceLabel.SetActive(true);
        inventoryLabel.SetActive(false);
        QrTexture.mainTexture = qr;
        BarTexture.mainTexture = bar;
        LabelText.text = labelNameText;
        BarCode.text = barCode;
        CompText.text = companyText;
        ProdText.text = productText;
        SubheadText.text = subheadingText;
        PkNText.text = $"{quantity} of {pkNum}";
        return null;
    }
    public static async System.Threading.Tasks.Task<string> AssignInventoryLabel(Texture2D qr, string name, string date, string status, string lot)
    {
        deviceLabel.SetActive(false);
        inventoryLabel.SetActive(true);
        QrTexture.mainTexture = qr;
        NameText.text = name;
        DateText.text = date;
        StatusText.text = status;
        LotText.text = lot;
        return null;
    }
    public static async System.Threading.Tasks.Task<Texture2D> PrintLabel(Camera labelCam, RenderTexture renderTexture)
    {
        RenderTexture previousActive = RenderTexture.active;
        RenderTexture.active = renderTexture;
        labelCam.targetTexture = renderTexture;

        // Render the camera output
        labelCam.Render();

        // Read the output pixels and store them in a Texture2D
        Texture2D outTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        outTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        outTexture.Apply();

        // Clean up and reset the active RenderTexture
        RenderTexture.active = previousActive;

        // Send the texture to the printer
        //await LabelMaker.SendTexture2DToUSBPrinterAsync(outTexture);

        // Save the texture to a PNG file
        //byte[] pngData = outTexture.EncodeToPNG();
        //string filePath = Path.Combine(Application.dataPath, "SavedOutputTexture.png");
        //File.WriteAllBytes(filePath, pngData);

        return outTexture;
    }
}

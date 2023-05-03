using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ZebraLabelPrinter : MonoBehaviour
{
    private string _printerName;

    public ZebraLabelPrinter(string printerName)
    {
        _printerName = printerName;
    }
    public void Print(DeviceLabel deviceLabel, int printQuantity)
    {
        string zplData = GenerateZPL(deviceLabel);
        //SendZplToPrinter(zplData, printQuantity);
    }
    string CompanyName = "HTS Inc";
    private string GenerateZPL(DeviceLabel deviceLabel)
    {
        StringBuilder zpl = new StringBuilder();

        zpl.AppendLine("^XA");
        zpl.AppendLine("^FO50,50");
        zpl.AppendLine("^A0N,50,50");
        zpl.AppendLine($"^FD{CompanyName}^FS");
        zpl.AppendLine("^FO50,100");
        zpl.AppendLine("^A0N,50,50");
        zpl.AppendLine($"^FD{deviceLabel.ProductHeading}^FS");
        zpl.AppendLine("^FO50,150");
        zpl.AppendLine("^A0N,50,50");
        zpl.AppendLine($"^FD{deviceLabel.ProductSubHeading}^FS");

        // Add more fields as needed

        // QR Code
        // Assuming you have a method to generate the QR code data (e.g., GenerateQRCodeData)
       // string qrCodeData = GenerateQRCodeData(deviceLabel);

      //  zpl.AppendLine("^FO400,50"); // Adjust X,Y coordinates to position the QR code on the label
      //  zpl.AppendLine($"^BQN,2,10"); // QR Code with Normal orientation, 2 = Model 2, 10 = Magnification factor
       // zpl.AppendLine($"^FDQA,{qrCodeData}^FS"); // Add QR code data to the label

        zpl.AppendLine("^XZ");

        return zpl.ToString();
    }

    //private string GenerateLabelDataAtPosition(DeviceLabel deviceLabel, int xPosition, int yPosition)
    //{
    //    StringBuilder zpl = new StringBuilder();
    //    zpl.AppendLine("^XA");
    //    zpl.AppendLine($"^FO{xPosition},50");
    //    zpl.AppendLine("^A0N,50,50");
    //    zpl.AppendLine($"^FD{deviceLabel.CompanyName}^FS");
    //    zpl.AppendLine($"^FO{xPosition},{yPosition}");
    //    zpl.AppendLine("^A0N,50,50");
    //    zpl.AppendLine($"^FD{deviceLabel.ProductHeading}^FS");
    //    string qrCodeData = GenerateQRCodeData(deviceLabel);
    //    zpl.AppendLine($"^FO{xPosition + 400},{yPosition}");
    //    zpl.AppendLine("^BQN,2,10");
    //    zpl.AppendLine($"^FDQA,{qrCodeData}^FS");
    //    zpl.AppendLine("^XZ");
    //    return zpl.ToString();
    //}

    //private async Task PrintLabelAtPosition(DeviceLabel selectedLabel, int xPosition, int yPosition)
    //{
    //    if (selectedLabel != null)
    //    {
    //        string zplData = GenerateLabelDataAtPosition(selectedLabel, xPosition, yPosition);
    //        zebraPrinter.Print(zplData, 1);
    //        await Task.Delay(1000);
    //    }
    //    else
    //    {
    //        Debug.LogError("No label selected for printing");
    //    }
    //}

    //public async Task PrintInventoryAtPositions(int printQuantity, Dictionary<int, int> positions)
    //{
    //    for (int i = 0; i < printQuantity; i++)
    //    {
    //        foreach (DeviceLabel deviceLabel in DeviceLabels)
    //        {
    //            foreach (KeyValuePair<int, int> position in positions)
    //            {
    //                stringplData = GenerateLabelDataAtPosition(deviceLabel, position.Key, position.Value);
    //                zebraPrinter.Print(zplData, 1);
    //                await Task.Delay(1000);
    //            }
    //        }
    //    }
    //}
    //private void SendZplToPrinter(string zplData, int printQuantity)
    //{
    //    for (int i = 0; i < printQuantity; i++)
    //    {
    //        byte[] zplCommand = System.Text.Encoding.ASCII.GetBytes(zplData);
    //        IntPtr unmanagedData = System.Runtime.InteropServices.Marshal.AllocHGlobal(zplCommand.Length);
    //        System.Runtime.InteropServices.Marshal.Copy(zplCommand, 0, unmanagedData, zplCommand.Length);
    //        RawPrinterHelper.SendBytesToPrinter(_printerName, unmanagedData, zplCommand.Length);
    //        System.Runtime.InteropServices.Marshal.FreeHGlobal(unmanagedData);
    //    }
    //}
    //private string GenerateQRCodeData(DeviceLabel deviceLabel)
    //{
    //    // Implement your QR code generation logic here
    //    // Return the QR code data as a string
    //    return "QR Code Data";
    //}
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Mono.Data.SqliteClient;
using static RawPrinterHelper;
using static Texture2DUtility;
using TMPro;
using System.Collections;
using SharpZebra.Printing;
using Object = UnityEngine.Object;
using static UIManager;
using static UnityEditor.PlayerSettings;

public class InventoryManager : MonoBehaviour
{
    public UIManager uiManager;
    public IncomingOutgoingPanel incomingOutgoingPanel;
    public List<InventoryItem> InventoryItems { get; private set; }
    public List<DeviceLabel> deviceLabels { get; private set; }
    private string connectionString;
    public static Texture2D icon1;
    public static Texture2D icon2;
    public static Texture2D icon3;
    public static List<StringTexturePair> pairs = new List<StringTexturePair> {
            new StringTexturePair("Text1", icon1),
            new StringTexturePair("Text2", icon2),
            new StringTexturePair("Text3", icon3)
        };
    string companyText = $"HTS Inc";
    string labelNameText = $"LBL-23-004";
    string productText = $"HTS2";
    string subheadingText = $"Online Vergence Exercises.";
    string pkNumText = $"of";
    string addressText = @"HTS Inc.
        6756 S Kings Ranch Rd.
        Gold Canyon, AZ 85118
        800-346-4925";
    int pkNum = 1;
    void Start()
    {
        InventoryItems = new List<InventoryItem>();
        deviceLabels = new List<DeviceLabel>();
        connectionString = "URI=file:" + Application.dataPath + "/InventoryDatabase.db";
        StartCoroutine(InitializeDatabase());
        unityMainThreadDispatcher = GameObject.Find("UnityMainThreadDispatcher").GetComponent<UnityMainThreadDispatcher>();
        unityMainThreadDispatcher = UnityMainThreadDispatcher.Instance();
        icon1 = Resources.Load<Texture2D>("Resourcefolder/icon1");
        icon2 = Resources.Load<Texture2D>("Resourcefolder/icon2");
        icon3 = Resources.Load<Texture2D>("Resourcefolder/icon3");
    }
    private IEnumerator InitializeDatabase()
    {
        if (!File.Exists(connectionString))
        {
            CreateDatabase();
        }
        else
        {
            ConnectToDatabase();
            
        }
        yield return new WaitForFixedUpdate();
        LoadDataFromDatabase();
        yield break;

    }

    private void CreateDatabase()
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            // Table for item, lot, and serialized items
            string createInventoryItemTable = "CREATE TABLE IF NOT EXISTS InventoryItem(Name TEXT PRIMARY KEY, Description TEXT, IsLot INTEGER, IsSerialed INTEGER)";
            // Table for item history
            string createItemHistoryTable = "CREATE TABLE IF NOT EXISTS ItemHistory(Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT, Date DATETIME, Quantity INTEGER)";
            // Table for lot history
            string createLotHistoryTable = "CREATE TABLE IF NOT EXISTS LotHistory(Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT, Date DATETIME, Incoming INTEGER, LotNumber INTEGER)";
            // Table for lot history
            string createLotCurrent = "CREATE TABLE IF NOT EXISTS LotCurrent(Name TEXT PRIMARY KEY, InLot INTEGER, InDate DATETIME, OutLot INTEGER, OutDate DATETIME)";
            // Table for serialized history
            string createSerializedHistoryTable = "CREATE TABLE IF NOT EXISTS SerializedHistory(Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT, Date DATETIME, SerialNumber TEXT)";
            // Table for Device Labels
            string createDeviceLabelTable = "CREATE TABLE IF NOT EXISTS DeviceLabel(Id INTEGER PRIMARY KEY AUTOINCREMENT, ProductHeading TEXT, ProductSubHeading TEXT, LabelName TEXT)";
            // Table for Device Label history
            string createDeviceLabelHistoryTable = "CREATE TABLE IF NOT EXISTS DeviceLabelHistory(Id INTEGER PRIMARY KEY AUTOINCREMENT, LabelName TEXT, Date DATETIME, HoloSN TEXT, LaptopSN TEXT, XboxSN TEXT, RouterSN TEXT)";

            using (var command = new SqliteCommand(createInventoryItemTable, connection))
            {
                command.ExecuteNonQuery();
            }
            using (var command = new SqliteCommand(createItemHistoryTable, connection))
            {
                command.ExecuteNonQuery();
            }
            using (var command = new SqliteCommand(createLotHistoryTable, connection))
            {
                command.ExecuteNonQuery();
            }
            using (var command = new SqliteCommand(createSerializedHistoryTable, connection))
            {
                command.ExecuteNonQuery();
            }
            using (var command = new SqliteCommand(createDeviceLabelTable, connection))
            {
                command.ExecuteNonQuery();
            }
            using (var command = new SqliteCommand(createDeviceLabelHistoryTable, connection))
            {
                command.ExecuteNonQuery();
            }
            using (var command = new SqliteCommand(createLotCurrent, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }

    public List<string> GetItemNames(int isLot, int isSerialized)
{
    List<string> itemNames = new List<string>();
    using (var connection = new SqliteConnection(connectionString))
    {
        connection.Open();
        string query = "SELECT Name FROM InventoryItem WHERE IsLot = @IsLotParam AND IsSerialed = @IsSerialedParam";
        using (var command = new SqliteCommand(query, connection))
        {
            var isLotParam = new SqliteParameter("@IsLotParam", isLot);
            var isSerializedParam = new SqliteParameter("@IsSerialedParam", isSerialized);
            command.Parameters.Add(isLotParam);
            command.Parameters.Add(isSerializedParam);

            using (IDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    itemNames.Add(reader.GetString(0));
                }
            }
        }
    }

    // Print the number of item names found
    Console.WriteLine($"Found {itemNames.Count} item names");

    return itemNames;
}

    public void UpdateInventoryItem(InventoryItem item)
    {
        int index = InventoryItems.FindIndex(i => i.Name == item.Name);
        if (index != -1)
        {
            InventoryItems[index] = item;

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE InventoryItem SET Name = @Name, Description = @Description, IsLot = @IsLot, IsSerialed = @IsSerialed WHERE Name = @OldName";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.Add("@OldName", item.Name);
                    command.Parameters.Add("@Name", item.Name);
                    command.Parameters.Add("@Description", item.Description);
                    command.Parameters.Add("@IsLot", item.IsLot);
                    command.Parameters.Add("@IsSerialed", item.IsSerialed);
                    command.ExecuteNonQuery();
                }
            }
        }
    }

    public void AddInventoryItem(InventoryItem item)
    {
        InventoryItems.Add(item);

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            string query = "INSERT INTO InventoryItem (Name, Description, IsLot, IsSerialed) VALUES (@Name, @Description, @IsLot, @IsSerialed)";

            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.Add("@Name", item.Name);
                command.Parameters.Add("@Description", item.Description);
                command.Parameters.Add("@IsLot", item.IsLot);
                command.Parameters.Add("@IsSerialed", item.IsSerialed);
                command.ExecuteNonQuery();
            }
        }
    }

    public void AddOrUpdateLotCurrent(InventoryItem item, DateTime date, int inLot, int outLot)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            string query;

            if (inLot > 0)
            {
                query = "INSERT OR REPLACE INTO LotCurrent (Name, InLot, InDate) VALUES (@Name, @InLot, @InDate)";
            }
            else
            {
                query = "INSERT OR REPLACE INTO LotCurrent (Name, OutLot, OutDate) VALUES (@Name, @OutLot, @OutDate)";
            }

            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.Add("@Name", DbType.String).Value = item.Name;

                if (inLot > 0)
                {
                    command.Parameters.Add("@InLot", DbType.Int32).Value = inLot;
                    command.Parameters.Add("@InDate", DbType.DateTime).Value = date;
                }
                else
                {
                    command.Parameters.Add("@OutLot", DbType.Int32).Value = outLot;
                    command.Parameters.Add("@OutDate", DbType.DateTime).Value = date;
                }

                command.ExecuteNonQuery();
            }
        }
    }

    public LotCurrent GetLotCurrent(string itemName)
    {
        LotCurrent lotCurrent = null;

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM LotCurrent WHERE Name = @Name";

            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.Add("@Name", DbType.String).Value = itemName;

                using (IDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        lotCurrent = new LotCurrent(
                            reader.GetString(0),
                            reader.GetInt32(1),
                            reader.GetDateTime(2),
                            reader.GetInt32(3),
                            reader.GetDateTime(4)
                        );
                    }
                }
            }
        }

        return lotCurrent;
    }

    private void ConnectToDatabase()
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
        }
    }

    private void LoadDataFromDatabase()
    {
        int i = 0;
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            string query;

            query = "SELECT * FROM InventoryItem";
            using (var command = new SqliteCommand(query, connection))
            {
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {                       
                        var item = new InventoryItem(i, reader.GetString(0), reader.GetString(1),  reader.GetInt32(2),  reader.GetInt32(3));
                        InventoryItems.Add(item);
                    }
                }
            }

            query = "SELECT * FROM DeviceLabel";
            using (var command = new SqliteCommand(query, connection))
            {
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var label = new DeviceLabel(reader.GetString(0), reader.GetString(1), reader.GetString(2));
                        deviceLabels.Add(label);
                    }
                }
            }
        }
    }
    public void AddDeviceLabel(DeviceLabel label)
    {
        deviceLabels.Add(label);
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            string query = "INSERT INTO DeviceLabel(ProductHeading, ProductSubHeading, LabelName, HoloSN, LaptopSN, XboxSN, RouterSN) VALUES(@ProductHeading, @ProductSubHeading, @LabelName, @HoloSN, @LaptopSN, @XboxSN, @RouterSN)";
            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.Add("@ProductHeading", label.ProductHeading);
                command.Parameters.Add("@ProductSubHeading", label.ProductSubHeading);
                command.Parameters.Add("@LabelName", label.LabelName);
                command.Parameters.Add("@HoloSN", label.HoloSN);
                command.Parameters.Add("@LaptopSN", label.LaptopSN);
                command.Parameters.Add("@XboxSN", label.XboxSN);
                command.Parameters.Add("@RouterSN", label.RouterSN);
                command.ExecuteNonQuery();
            }
        }
    }
    public void AddItemHistory(InventoryItem item, DateTime date, int quantity)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            string query = "INSERT INTO ItemHistory (Name, Date, Quantity) VALUES (@Name, @Date, @Quantity)";

            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.Add("@Name", item.Name);
                command.Parameters.Add("@Date", date);
                command.Parameters.Add("@Quantity", quantity);
                command.ExecuteNonQuery();
            }
        }
    }
    public void AddLotHistory(InventoryItem item, DateTime date, int incoming, int lotNumber)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            string query = "INSERT INTO LotHistory (Name, Date, Incoming, LotNumber) VALUES (@Name, @Date, @Incoming, @LotNumber)";

            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.Add("@Name", item.Name);
                command.Parameters.Add("@Date", date);
                command.Parameters.Add("@Incoming", incoming);
                command.Parameters.Add("@LotNumber", lotNumber);
                command.ExecuteNonQuery();
            }
        }
    }
    public void AddSerializedHistory(InventoryItem item, DateTime date, string serialNumber)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            string query = "INSERT INTO SerializedHistory (Name, Date, SerialNumber) VALUES (@Name, @Date, @SerialNumber)";

            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.Add("@Name", item.Name);
                command.Parameters.Add("@Date", date);
                command.Parameters.Add("@SerialNumber", serialNumber);
                command.ExecuteNonQuery();
            }
        }
    }
    public void AddDeviceLabelHistory(DeviceLabel label, DateTime date)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            string query = "INSERT INTO DeviceLabelHistory(LabelName, Date, HoloSN, LaptopSN, XboxSN, RouterSN) VALUES(@LabelName, @Date, @HoloSN, @LaptopSN, @XboxSN, @RouterSN)";
            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.Add("@LabelName", label.LabelName);
                command.Parameters.Add("@Date", date);
                command.Parameters.Add("@HoloSN", label.HoloSN);
                command.Parameters.Add("@LaptopSN", label.LaptopSN);
                command.Parameters.Add("@XboxSN", label.XboxSN);
                command.Parameters.Add("@RouterSN", label.RouterSN);
                command.ExecuteNonQuery();
            }
        }
    }

    public void UpdateDeviceLabel(DeviceLabel label)
    {
        int index = deviceLabels.FindIndex(l => l.LabelName == label.LabelName);
        if (index != -1)
        {
            deviceLabels[index] = label;
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE DeviceLabel SET ProductHeading=@ProductHeading, ProductSubHeading=@ProductSubHeading, LabelName=@LabelName WHERE LabelName=@OldLabelName";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.Add("@OldLabelName", label.LabelName);
                    command.Parameters.Add("@ProductHeading", label.ProductHeading);
                    command.Parameters.Add("@ProductSubHeading", label.ProductSubHeading);
                    command.Parameters.Add("@LabelName", label.LabelName);
                    command.ExecuteNonQuery();
                }
            }
        }
    }

    public InventoryItem GetInventoryItemByName(string name)
    {
        return InventoryItems.Find(item => item.Name == name);
    }
    public DeviceLabel GetDeviceLabelByName(string name)
    {
        return deviceLabels.Find(label => label.LabelName == name);
    }
    public List<DeviceLabel> GetDeviceLabelNames()
    {
        deviceLabels = new List<DeviceLabel>();
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            string query = "SELECT LabelName, ProductHeading, ProductSubHeading FROM DeviceLabel";
            using (var command = new SqliteCommand(query, connection))
            {
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string labelName = reader.GetString(1);
                        string productHeading = reader.GetString(2);
                        string productSubHeading = reader.GetString(0);
                        string laptopSN = uiManager.uiObjects["InputField3"].GetComponent<TMP_InputField>().text.Trim();
                        string routerSN = uiManager.uiObjects["InputField4"].GetComponent<TMP_InputField>().text.Trim();
                        string holoSN = uiManager.uiObjects["InputField5"].GetComponent<TMP_InputField>().text.Trim();
                        string xboxSN = uiManager.uiObjects["InputField6"].GetComponent<TMP_InputField>().text.Trim();
                        DeviceLabel deviceLabel = new DeviceLabel(labelName, productHeading, productSubHeading, holoSN, laptopSN, xboxSN, routerSN);
                        deviceLabels.Add(deviceLabel);
                    }
                }
            }
        }
        return deviceLabels;
    }
    public void ShowSearchResults(string searchTerm)
    {
        List<InventoryItem> searchResults = InventoryItems.Where(item => item.Name.Contains(searchTerm)).ToList();
        uiManager.DisplaySearchResults(searchResults);
    }
    public string GenerateInventoryLabelData(InventoryItem item, int lotNumber = 0, string serialNumber = "")
    {
        string labelData = $"Item Name: {item.Name}\nDescription: {item.Description}\n";
        if (item.IsLot == 1)
        {
            labelData += $"Lot Number: {lotNumber}\n";
        }
        if (item.IsSerialed == 1)
        {
            labelData += $"Serial Number: {serialNumber}\n";
        }
        return labelData;
    }
    private UnityMainThreadDispatcher unityMainThreadDispatcher;
    public Task WrapCoroutineInTask(Coroutine coroutine)
    {
        TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
        StartCoroutine(CompleteWhenDone(coroutine, tcs));
        return tcs.Task;
    }
    private IEnumerator CompleteWhenDone(Coroutine coroutine, TaskCompletionSource<object> tcs)
    {
        yield return coroutine;
        tcs.SetResult(null);
    }
    public async Task<Texture2D> GenerateDeviceLabelQR(string qrCodeData)
    {      
        Texture2D qrCodeImage = null;
        qrCodeImage = LabelMaker.GenerateQRFromGoogleAPI(qrCodeData);
        return qrCodeImage;
    }
    public async Task<Texture2D> GenerateInventoryLabelQR(string nameText, string dateText, string statusText, string lotText)
    {
        string qrCodeData = nameText + dateText + statusText + lotText;
        Debug.LogError("GenerateInventoryQRCode selected. qrCodeData: " + qrCodeData);
        Texture2D qrCodeImage = null;
        qrCodeImage = LabelMaker.GenerateQRFromGoogleAPI(qrCodeData);
        return qrCodeImage;
    }
    public async Task<Texture2D> GenerateDeviceLabelUPC(DeviceLabel label)
    {
        string upcCodeData = $"860009762107";
        Debug.LogError("GenerateQRCode selected. upcCodeData: " + upcCodeData);
        Texture2D UPCCodeImage = null;
        UPCCodeImage = LabelMaker.GenerateUPCABarcodeTexture(upcCodeData);
        return UPCCodeImage;
    }
    public async Task PrintInventoryLabelsAsync(InventoryItem item, int printQuantity, string printerIPAddress)
    {
        string nameText = $"{item.Name}";
        string dateText = $"{item.Date}";
        string statusText = $"{item.Incoming}";
        string lotText = $"{item.LotNumber}"; 

        unityMainThreadDispatcher.Enqueue(async () =>
        {
            for (int i = 0; i < printQuantity; i++)
            {
                Texture2D qrTexture = null;
                qrTexture = await GenerateInventoryLabelQR(nameText, dateText, statusText, lotText);
                _ = await PrintCam.AssignInventoryLabel(qrTexture, nameText, dateText, statusText, lotText);
                _ = LabelMaker.SendTexture2DToUSBPrinterAsync(PrintCam.PrintLabel(PrintCam.labelCam, PrintCam.renderTexture).Result);
            }
        });
    }
    public async Task PrintDeviceLabelsAsync(DeviceLabel labelprint, int printQuantity, string qrCodeData)
    {
        unityMainThreadDispatcher.Enqueue(async () =>
        {
            for (int i = 0; i < printQuantity; i++)
            {
                Texture2D qrTexture = null;
                Texture2D upcTexture = null;
                qrTexture = await GenerateDeviceLabelQR(qrCodeData);
                upcTexture = await GenerateDeviceLabelUPC(labelprint);
                
                _ = await PrintCam.AssignLabel(qrTexture, upcTexture, "860009762107", companyText, labelprint.LabelName, labelprint.ProductHeading, labelprint.ProductSubHeading, i + 1, printQuantity);
                await Task.Delay(100);
                Texture2D pic = PrintCam.PrintLabel(PrintCam.labelCam, PrintCam.renderTexture).Result;
                await Task.Delay(100);
                _ = LabelMaker.SendTexture2DToUSBPrinterAsync(pic);
                await Task.Delay(100);
                _ = LabelMaker.SendTexture2DToUSBPrinterAsync(pic);
  
            }
        });
    }

}
public class InventoryItem
{
    public int Id { get; set; }
    public string Name { get; set; }    
    public DateTime Date { get; set; }    
    public string Description { get; set; }
    public string SerialNumber { get; set; }
    public int Quantity { get; set; }
    public int LotNumber { get; set; }
    public int IsLot { get; set; }
    public int IsSerialed { get; set; }
    public int Incoming { get; set; }

    public InventoryItem(int id, string name, string description, int isLot, int isSerialed) // item&lot&serialied items
    {
        Name = name; // unique
        Description = description; 
        IsLot = isLot; // unique
        IsSerialed = isSerialed; // unique
    }
    public InventoryItem(string name, int amount) // item history
    {
        Name = name; 
        Date = DateTime.Now;
        Quantity = amount;
    }
    public InventoryItem(string name, string description, int incoming, int lotNum) // lot history
    {
        Name = name;
        Date = DateTime.Now;
        Incoming = incoming;
        LotNumber = lotNum;
    }
    public InventoryItem(string name, string sn) // serialied history
    {
        Name = name;
        Date = DateTime.Now;
        SerialNumber = sn;
    }
}

public class DeviceLabel
{
    public int ProductId { get; set; }
    public string ProductHeading { get; set; }
    public string ProductSubHeading { get; set; }
    public string LabelName { get; set; }
    public string HoloSN { get; set; }
    public string LaptopSN { get; set; }
    public string XboxSN { get; set; }
    public string RouterSN { get; set; }



    public DeviceLabel(string productHeading, string productSubHeading, string labelName) 
    {
        ProductHeading = productHeading;
        ProductSubHeading = productSubHeading;
        LabelName = labelName;
    }
    public DeviceLabel(string productHeading, string productSubHeading, string labelName, string holoSN, string laptopSN, string xboxSN, string routerSN)
    {
        ProductHeading = productHeading;
        ProductSubHeading = productSubHeading;
        LabelName = labelName;
        HoloSN = holoSN;
        LaptopSN = laptopSN;
        XboxSN = xboxSN;
        RouterSN = routerSN;
    }
}

// Define the LotCurrent class if it hasn't been defined already
public class LotCurrent
{
    public string Name { get; set; }
    public int InLot { get; set; }
    public DateTime InDate { get; set; }
    public int OutLot { get; set; }
    public DateTime OutDate { get; set; }

    public LotCurrent(string name, int inlot, DateTime indate, int outlot, DateTime outdate)
    {
        Name = name;
        InLot = inlot;
        InDate = indate;
        OutLot = outlot;
        OutDate = outdate;
    }
}

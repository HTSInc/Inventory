using Mono.Data.SqliteClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<InventoryItem> InventoryItems { get; private set; }
    public List<DeviceLabel> DeviceLabels { get; private set; }

    private string connectionString;

    void Start()
    {
        InventoryItems = new List<InventoryItem>();
        DeviceLabels = new List<DeviceLabel>();
        connectionString = "URI=file:" + Application.dataPath + "/InventoryDatabase.db";
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        if (!File.Exists(connectionString))
        {
            CreateDatabase();
        }
        else
        {
            ConnectToDatabase();
            LoadDataFromDatabase();
        }
    }

    private void CreateDatabase()
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            string createInventoryItemTable = "CREATE TABLE IF NOT EXISTS InventoryItem(Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT, Heading TEXT, SubHeading TEXT, LabelName TEXT, Quantity INTEGER)";
            string createDeviceLabelTable = "CREATE TABLE IF NOT EXISTS DeviceLabel(Id INTEGER PRIMARY KEY AUTOINCREMENT, CompanyName TEXT, ProductHeading TEXT, ProductSubHeading TEXT, LabelName TEXT, QR TEXT)";

            using (var command = new SqliteCommand(createInventoryItemTable, connection))
            {
                command.ExecuteNonQuery();
            }

            using (var command = new SqliteCommand(createDeviceLabelTable, connection))
            {
                command.ExecuteNonQuery();
            }
        }
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
        // Load inventory items
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT * FROM InventoryItem";
            using (var command = new SqliteCommand(query, connection))
            {
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new InventoryItem(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetInt32(5));
                        InventoryItems.Add(item);
                    }
                }
            }
        }

        // Load device labels
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT * FROM DeviceLabel";
            using (var command = new SqliteCommand(query, connection))
            {
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var label = new DeviceLabel(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4));
                        DeviceLabels.Add(label);
                    }
                }
            }
        }
    }

    public void AddInventoryItem(string name, string heading, string subHeading, string labelName, int quantity)
    {
        var item = new InventoryItem(name, heading, subHeading, labelName, quantity);
        InventoryItems.Add(item);

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            string query = "INSERT INTO InventoryItem(Name, Heading, SubHeading, LabelName, Quantity) VALUES(@Name, @Heading, @SubHeading, @LabelName, @Quantity)";
            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Heading", heading);
                command.Parameters.AddWithValue("@SubHeading", subHeading);
                command.Parameters.AddWithValue("@LabelName", labelName);
                command.Parameters.AddWithValue("@Quantity", quantity);

                command.ExecuteNonQuery();
            }
        }
    }

    public void RemoveInventoryItem(int id)
    {
        InventoryItems.RemoveAll(x => x.Id == id);

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            string query = "DELETE FROM InventoryItem WHERE Id = @Id";
            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
        }
    }

    public void AddDeviceLabel(string companyName, string productHeading, string productSubHeading, string labelName, string qr)
    {
        var label = new DeviceLabel(companyName, productHeading, productSubHeading, labelName, qr);
        DeviceLabels.Add(label);

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            string query = "INSERT INTO DeviceLabel(CompanyName, ProductHeading, ProductSubHeading, LabelName, QR) VALUES(@CompanyName, @ProductHeading, @ProductSubHeading, @LabelName, @QR)";
            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@CompanyName", companyName);
                command.Parameters.AddWithValue("@ProductHeading", productHeading);
                command.Parameters.AddWithValue("@ProductSubHeading", productSubHeading);
                command.Parameters.AddWithValue("@LabelName", labelName);
                command.Parameters.AddWithValue("@QR", qr);

                command.ExecuteNonQuery();
            }
        }
    }

    public void RemoveDeviceLabel(int id)
    {
        DeviceLabels.RemoveAll(x => x.Id == id);

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            string query = "DELETE FROM DeviceLabel WHERE Id = @Id";
            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
        }
    }
}

[System.Serializable]
public class InventoryItems
{
    public int Id;
    public string Name;
    public string Heading;
    public string SubHeading;
    public string LabelName;
    public int Quantity;

    public InventoryItem(int id, string name, string heading, string subHeading, string labelName, int quantity)
    {
        Id = id;
        Name = name;
        Heading = heading;
        SubHeading = subHeading;
        LabelName = labelName;
        Quantity = quantity;
    }
}

[System.Serializable]
public class DeviceLabel
{
    public int Id;
    public string CompanyName;
    public string ProductHeading;
    public string ProductSubHeading;
    public string LabelName;
    public string QR;

    public DeviceLabel(int id, string companyName, string productHeading, string productSubHeading, string labelName, string qr)
    {
        Id = id;
        CompanyName = companyName;
        ProductHeading = productHeading;
        ProductSubHeading = productSubHeading;
        LabelName = labelName;
        QR = qr;
    }
}

[System.Serializable]
public class InventoryItem
{
    public int Id;
    public string Name;
    public string Heading;
    public string SubHeading;
    public string LabelName;
    public int Quantity;
    public string LotNumber;

    public InventoryItem(int id, string name, string heading, string subHeading, string labelName, int quantity, string lotNumber)
    {
        Id = id;
        Name = name;
        Heading = heading;
        SubHeading = subHeading;
        LabelName = labelName;
        Quantity = quantity;
        LotNumber = lotNumber;
    }
}

[System.Serializable]
public class DeviceLabel
{
    public int Id;
    public string CompanyName;
    public string ProductHeading;
    public string ProductSubHeading;
    public string LabelName;

    public DeviceLabel(int id, string companyName, string productHeading, string productSubHeading, string labelName)
    {
        Id = id;
        CompanyName = companyName;
        ProductHeading = productHeading;
        ProductSubHeading = productSubHeading;
        LabelName = labelName;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Data;
using Mono.Data.SqliteClient;

public class CRMManager : MonoBehaviour
{
    public List<CRMEntry> Entries = new List<CRMEntry>();
    public InventoryManager InventoryManager;

    public void Initialize(InventoryManager inventoryManager)
    {
        InventoryManager = inventoryManager;
        // Load CRM entries from the database
    }
    public CRMManager(InventoryManager i)
    {
        InventoryManager = i;
    }
    public void AddEntry(CRMEntry e)
    {
        Entries.Add(e);
        if (e.CallType == "order")
        {
            InventoryManager.Inventory.FindAll(x => e.ProductOrdered.Split(',').Contains(x.Name)).ForEach(x => x.Quantity--);
        }
    }
    public void HandleCRMAction(string action)
    {
        CRMPanel crmPanel = FindObjectOfType<CRMPanel>();
        switch (action)
        {
            case "Add CRM Entry":
                crmPanel.OnAddEntry();
                break;
            case "Remove CRM Entry":
                crmPanel.OnRemoveEntry();
                break;
            case "View CRM Entries":
                // Display CRM entries in a UI element, such as a scrollable list or a table
                break;
            default:
                Debug.LogError("Invalid CRM action: " + action);
                break;
        }
    }

    public void RemoveEntry(CRMEntry e)
    {
        Entries.RemoveAll(x => x == e);
    }

    public List<CRMEntry> GetAllEntries()
    {
        return Entries;
    }

    public List<CRMEntry> GetEntriesByCallType(string t)
    {
        return Entries.FindAll(x => x.CallType == t);
    }

    public List<CRMEntry> GetEntriesByEmployeeName(string n)
    {
        return Entries.FindAll(x => x.EmployeeName == n);
    }

    public List<CRMEntry> GetEntriesByAccountNumber(string a)
    {
        return Entries.FindAll(x => x.AccountNumber == a);
    }

    public List<CRMEntry> GetEntriesByOrderNumber(string o)
    {
        return Entries.FindAll(x => x.OrderNumber == o);
    }

    public void UpdateCRMEntry(CRMEntry oldEntry, CRMEntry newEntry)
    {
        int index = Entries.IndexOf(oldEntry);
        if (index != -1)
        {
            Entries[index] = newEntry;
        }
    }

    public void UpdateInventoryQuantities()
    {
        Entries.FindAll(x => x.CallType == "order").ForEach(e =>
        {
            foreach (string p in e.ProductOrdered.Split(','))
            {
                InventoryItem i = InventoryManager.Inventory.Find(x => x.Name == p);
                i.Quantity--;
            }
        });
    }
}

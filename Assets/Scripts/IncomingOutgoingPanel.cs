using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IncomingOutgoingPanel : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public Dropdown inventoryDropdown;
    public InputField numberOfPackagesInput, trackingNumberInput, notesInput, productNameInput, labelNameInput, productHeading, productSubHeading, lblName;
    public Text currentDateDisplay;
    public Toggle incomingOutgoingToggle;
    public Button cancelButton, addButton, removeButton;


    public void Initialize(InventoryManager manager)
    {
        inventoryManager = manager;
        UpdateUI();
    }

    public void UpdateUI()
    {
        // Populate the dropdown with item names from the InventoryManager
        inventoryDropdown.ClearOptions();
        List<string> itemNames = inventoryManager.GetItemNames();
        inventoryDropdown.AddOptions(itemNames);

        // Set current date display
        currentDateDisplay.text = DateTime.Now.ToString("yyyy-MM-dd");
    }
    public void OnSubmit()
    {
        // Process user input and call inventoryManager.UpdateInventoryItem() with the updated item
        string selectedItemName = inventoryDropdown.options[inventoryDropdown.value].text;
        InventoryItems.InventoryItem item = inventoryManager.GetInventoryItemByName(selectedItemName);

        int numberOfPackages = int.Parse(numberOfPackagesInput.text);
        bool isIncoming = incomingOutgoingToggle.isOn;

        if (item is InventoryItems.LotItem lotItem)
        {
            if (isIncoming)
            {
                lotItem.Quantity += numberOfPackages;
                lotItem.LastLot += numberOfPackages;
            }
            else
            {
                lotItem.Quantity -= numberOfPackages;
                lotItem.LastLot -= numberOfPackages;
            }
        }
        else if (item is InventoryItems.SerialedItem serialedItem)
        {
            if (isIncoming)
            {
                serialedItem.Quantity += numberOfPackages;
            }
            else
            {
                serialedItem.Quantity -= numberOfPackages;
            }
        }
        else if (item is InventoryItems.InventoryItem nonLotItem)
        {
            if (isIncoming)
            {
                nonLotItem.Quantity += numberOfPackages;
            }
            else
            {
                nonLotItem.Quantity -= numberOfPackages;
            }
        }

        if (isIncoming)
            inventoryManager.AddQuantity(selectedItemName, numberOfPackages);
        else
            inventoryManager.RemoveQuantity(selectedItemName, numberOfPackages);

        UpdateUI();
    }

    public void OnCancel()
    {
        gameObject.SetActive(false);
    }

    public void OnAdd()
    {
        InventoryItems.DeviceLabel deviceLabel = new InventoryItems.DeviceLabel(productNameInput.text, productHeading.text, productSubHeading.text, lblName.text, "");
        inventoryManager.AddDeviceLabel(deviceLabel);
        inventoryManager.SaveInventory();
    }

    public void OnRemove()
    {
        InventoryItems.DeviceLabel deviceLabelToRemove = inventoryManager.GetDeviceLabelByName(lblName.text);
        inventoryManager.RemoveDeviceLabel(deviceLabelToRemove);
        inventoryManager.SaveInventory();
    }
}
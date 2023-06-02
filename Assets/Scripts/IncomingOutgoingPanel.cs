using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IncomingOutgoingPanel : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public UIManager uiManager;
    public TMP_Dropdown inventoryDropdown;
    private Toggle incomingOutgoingToggle;
    private GameObject[] inputFields = new GameObject[7];
    private Button centerActionButton;
    private Button centerCancelButton;
    int isLot = 0;
    int isSerialed = 0;
    public static List<string> itemNames;
    public static List<DeviceLabel> deviceLabels;

    public void Initialize(InventoryManager manager, UIManager uiInstance)
    {
        inventoryManager = manager;
        uiManager = uiInstance;
        centerActionButton = uiManager.uiObjects["actionButton"].GetComponent<Button>();
        centerCancelButton = uiManager.uiObjects["cancelButton"].GetComponent<Button>();
        inventoryDropdown = uiManager.uiObjects["Dropdown"].GetComponent<TMP_Dropdown>();
        incomingOutgoingToggle = uiManager.uiObjects["Toggle"].GetComponent<Toggle>();

        for (int i = 0; i < uiManager.maxCChildren; i++)
        {
            inputFields[i] = uiManager.uiObjects[$"InputField{i}"].gameObject;
        }
    }

    public async void RunAsyncWithoutBlocking(Func<Task> asyncMethod)
    {
        await Task.Run(async () => await asyncMethod());
    }

    public void SetCenterButtonActions(string panelName, int action)
    {
        centerActionButton.onClick.RemoveAllListeners();
        centerCancelButton.onClick.RemoveAllListeners();

        centerActionButton.onClick.AddListener(() => RunAsyncWithoutBlocking(() => OnSubmit(panelName, action)));
        centerCancelButton.onClick.AddListener(() => OnCancel());
    }

    public void UpdateUI()
    {
        inventoryDropdown.ClearOptions();
        ClearInputFields();
        switch (uiManager.curView)
        {
            case "LotHandling":
                itemNames = inventoryManager.GetItemNames(1, 0);
                break;
            case "Shipment":
                deviceLabels = inventoryManager.GetDeviceLabelNames();
                itemNames = deviceLabels.Select(label => $"{label.LabelName}").ToList();
                break;
            case "Inventory":
                itemNames = inventoryManager.GetItemNames(0, 0);
                break;
            default:
                break;
        }

        inventoryDropdown.AddOptions(itemNames);
        uiManager.OnDropdownValueChanged(inventoryDropdown.value);
        uiManager.uiObjects["Date"].GetComponent<TextMeshProUGUI>().text = DateTime.Now.ToString("ddMMyy");
    }

    public void HandleLeftButtonAction(string panelName)
    {
        uiManager.EnableRightButtons(false);
        uiManager.EnableCenterContent(false);
        Button[] leftButtons = uiManager.leftButtonParent.GetComponentsInChildren<Button>();
        Color defaultColor = Color.white;
        Color selectedColor = new Color(0.75f, 0.75f, 0.75f);

        for (int i = 0; i < leftButtons.Length; i++)
        {
            uiManager.SetButtonColor(leftButtons[i], defaultColor);
        }

        switch (panelName)
        {
            case "LotHandling":
                uiManager.curView = "LotHandling";
                uiManager.curViewLKey = "L0";
                uiManager.SetButtonColor(leftButtons[0], selectedColor);
                break;
            case "Shipment":
                uiManager.curView = "Shipment";
                uiManager.curViewLKey = "L1";
                uiManager.SetButtonColor(leftButtons[1], selectedColor);
                break;
            case "Inventory":
                uiManager.curView = "Inventory";
                uiManager.curViewLKey = "L2";
                uiManager.SetButtonColor(leftButtons[2], selectedColor);
                break;
            default:
                break;
        }

        uiManager.UpdateButtonTexts("right");
        uiManager.EnableRightButtons(true);
    }

    public void HandleRightButtonAction(string panelName, int action)
    {
        uiManager.EnableCenterContent(false);
        uiManager.UpdateCenterContent(panelName, action);
        uiManager.EnableCenterContent(true);
        SetCenterButtonActions(panelName, action);
        UpdateUI();
    }

    private async Task PrintInventoryLabelsAsync()
    {
        string itemName = inventoryDropdown.options[inventoryDropdown.value].text;
        InventoryItem item = inventoryManager.GetInventoryItemByName(itemName);
        if (item == null)
        {
            Debug.LogError("Invalid item selected.");
            return;
        }

        int printQuantity = int.Parse(inputFields[8].GetComponent<TMP_InputField>().text.Trim());
        string printerIPAddress = inputFields[9].GetComponent<TMP_InputField>().text.Trim();
        int lotNumber = item.IsLot == 1 ? int.Parse(inputFields[10].GetComponent<TMP_InputField>().text.Trim()) : 0;
        string serialNumber = item.IsSerialed == 1 ? inputFields[11].GetComponent<TMP_InputField>().text.Trim() : "";

        await inventoryManager.PrintInventoryLabelsAsync(item, printQuantity, printerIPAddress);
    }

    private void PrintDeviceLabels()
    {
        string labelName = inventoryDropdown.options[inventoryDropdown.value].text;
        DeviceLabel label = inventoryManager.GetDeviceLabelByName(labelName);
        if (label == null)
        {
            Debug.LogError("Invalid label selected.");
            return;
        }
        DeviceLabel labelprint = new DeviceLabel("", "", "", "", "", "", "");
        int pkNum = 1;
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            Debug.LogError("Valid label selected.");
            labelprint = new DeviceLabel(label.ProductHeading, label.ProductSubHeading, label.LabelName, uiManager.uiObjects["InputField3"].GetComponent<TMP_InputField>().text.Trim(), uiManager.uiObjects["InputField4"].GetComponent<TMP_InputField>().text.Trim(), uiManager.uiObjects["InputField5"].GetComponent<TMP_InputField>().text.Trim(), uiManager.uiObjects["InputField6"].GetComponent<TMP_InputField>().text.Trim());
            pkNum = int.Parse(uiManager.uiObjects["InputField2"].GetComponent<TMP_InputField>().text.Trim());
        });
        
        string qCodeData = "L: " + labelprint.LabelName + "PH: " + labelprint.ProductHeading + "PSH: " + labelprint.ProductSubHeading + "ID: " + labelprint.ProductId + "LSN: " + labelprint.LaptopSN + "HSN: " + labelprint.HoloSN + "ESN: " + labelprint.RouterSN + "XSN: " + labelprint.XboxSN + " ";
        _ = Task.Run(async () =>
        {
            AddToDeviceLabelHistory(labelprint); 
            _ = inventoryManager.PrintDeviceLabelsAsync(labelprint, pkNum, qCodeData);
        });
    }

    private void ClearInputFields()
    {
        for (int i = 0; i <= uiManager.maxCChildren; i++)
        {
            uiManager.uiObjects[$"InputField{i}"].GetComponent<TMP_InputField>().text = "";
        }
    }

    private void AddOrUpdateInventoryItem()
    {
        string itemName = uiManager.uiObjects["InputField0"].GetComponent<TMP_InputField>().text.Trim();
        string itemDescription = uiManager.uiObjects["InputField1"].GetComponent<TMP_InputField>().text.Trim();

        if (string.IsNullOrEmpty(itemName) || string.IsNullOrEmpty(itemDescription))
        {
            Debug.LogError("Item name and description are required.");
            return;
        }

        InventoryItem existingItem = inventoryManager.GetInventoryItemByName(itemName);

        if (existingItem != null)
        {
            existingItem.Description = itemDescription;
            existingItem.IsLot = isLot;
            existingItem.IsSerialed = isSerialed;
            inventoryManager.UpdateInventoryItem(existingItem);
            Debug.Log("Updated existing inventory item: " + itemName);
        }
        else
        {
            InventoryItem newItem = new InventoryItem(0, itemName, itemDescription, isLot, isSerialed);
            inventoryManager.AddInventoryItem(newItem);
            Debug.Log("Added new inventory item: " + itemName);
        }

        ClearInputFields();
    }
    private void AddOrUpdateDeviceLabelItem()
    {
        bool isAdding = uiManager.uiObjects["Toggle"].GetComponent<Toggle>().isOn;
        string productHeading = uiManager.uiObjects["InputField0"].GetComponent<TMP_InputField>().text.Trim();
        string productSubheading = uiManager.uiObjects["InputField1"].GetComponent<TMP_InputField>().text.Trim();
        string labelName = uiManager.uiObjects["InputField2"].GetComponent<TMP_InputField>().text.Trim();

        if (string.IsNullOrEmpty(productHeading) || string.IsNullOrEmpty(productSubheading) || string.IsNullOrEmpty(labelName))
        {
            Debug.LogError("Product heading, subheading, and label name are required.");
            return;
        }

        DeviceLabel existingLabel = inventoryManager.GetDeviceLabelByName(labelName);

        if (isAdding)
        {
            if (existingLabel != null)
            {
                Debug.LogError("Label with the same name already exists.");
                return;
            }

            DeviceLabel newLabel = new DeviceLabel(productHeading, productSubheading, labelName);
            inventoryManager.AddDeviceLabel(newLabel);
            Debug.Log("Added new device label: " + labelName);
        }
        else
        {
            if (existingLabel == null)
            {
                Debug.LogError("Label not found.");
                return;
            }

            existingLabel.ProductHeading = productHeading;
            existingLabel.ProductSubHeading = productSubheading;
            inventoryManager.UpdateDeviceLabel(existingLabel);
            Debug.Log("Updated existing device label: " + labelName);
        }

        ClearInputFields();
    }
    private void AddToInventoryHistory(int index)
    {
        DateTime date = DateTime.Now;
        string itemName = uiManager.uiObjects["Dropdown"].GetComponent<TMP_Dropdown>().options[inventoryDropdown.value].text;
        InventoryItem item = inventoryManager.GetInventoryItemByName(itemName);

        if (item == null)
        {
            Debug.LogError("Invalid item selected.");
            return;
        }

        int quantity = int.Parse(uiManager.uiObjects["InputField3"].GetComponent<TMP_InputField>().text.Trim());

        switch (index)
        {
            case 0:
                bool isIncoming = uiManager.uiObjects["Toggle"].GetComponent<Toggle>().isOn;

                if (isIncoming)
                {
                    int lotNumber = int.Parse(uiManager.uiObjects["InputField4"].GetComponent<TMP_InputField>().text.Trim());
                    inventoryManager.AddLotHistory(item, date, quantity, lotNumber);
                    inventoryManager.AddOrUpdateLotCurrent(item, date, quantity, -1);
                    Debug.Log("Added incoming lot history.");
                }
                else
                {
                    int lotNumber = int.Parse(uiManager.uiObjects["InputField5"].GetComponent<TMP_InputField>().text.Trim());
                    inventoryManager.AddLotHistory(item, date, -quantity, lotNumber);
                    inventoryManager.AddOrUpdateLotCurrent(item, date, -quantity, -1);
                    Debug.Log("Added outgoing lot history.");
                }

                ClearInputFields();
                break;
            case 1:
                string orderNumber = uiManager.uiObjects["InputField6"].GetComponent<TMP_InputField>().text.Trim();
                string accountNumber = uiManager.uiObjects["InputField7"].GetComponent<TMP_InputField>().text.Trim();
                Debug.Log($"Shipment saved for Order #{orderNumber}, Account #{accountNumber}");
                ClearInputFields();
                break;
            case 2:
                bool isAdding = uiManager.uiObjects["Toggle"].GetComponent<Toggle>().isOn;

                if (isAdding)
                {
                    inventoryManager.AddItemHistory(item, date, quantity);
                    Debug.Log($"Added {quantity} to current inventory of {itemName}");
                }
                else
                {
                    inventoryManager.AddItemHistory(item, date, -quantity);
                    Debug.Log($"Removed {quantity} from current inventory of {itemName}");
                }

                ClearInputFields();
                break;
            default:
                break;
        }
    }
    public void AddToDeviceLabelHistory(DeviceLabel lbl)
    {
        DateTime date = DateTime.Now;

        if (lbl == null)
        {
            Debug.LogError("Invalid item selected.");

        }
        inventoryManager.AddDeviceLabelHistory(lbl, date);

    }

    public async Task OnSubmit(string panelName, int action)
    {
        switch (panelName)
        {
            case "LotHandling":
                switch (action)
                {
                    case 0:
                        AddToInventoryHistory(0);
                        await PrintInventoryLabelsAsync();
                        break;
                    case 1:
                        isLot = 1;
                        isSerialed = 0;
                        AddOrUpdateInventoryItem();
                        await PrintInventoryLabelsAsync();
                        break;
                    case 2:
                        PrintInventoryLabelsAsync();
                        break;
                }
                break;
            case "Shipment":
                switch (action)
                {
                    case 0:
                        PrintDeviceLabels();
                        break;
                    case 1:
                        AddOrUpdateDeviceLabelItem();
                        break;
                    case 2:
                        PrintDeviceLabels();
                        break;
                }
                break;
            case "Inventory":
                switch (action)
                {
                    case 0:
                        AddToInventoryHistory(2);
                        await PrintInventoryLabelsAsync();
                        break;
                    case 1:
                        isLot = 0;
                        isSerialed = 0;
                        AddOrUpdateInventoryItem();
                        await PrintInventoryLabelsAsync();
                        break;
                }
                break;
        }
    }

    public void OnCancel()
    {
        gameObject.SetActive(false);
        uiManager.EnableAllLeftButtons();
        uiManager.EnableRightButtons(false);
        uiManager.EnableCenterContent(false);
    }
}


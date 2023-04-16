using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public CRMManager crmManager;
    private Dictionary<string, GameObject> panels;
    public InputField itemNameInput, newNameInput, newDescriptionInput, newQuantityInput, newLotInput, newLastLotInput, searchTermInput;
    public IncomingOutgoingPanel incomingOutgoingPanel;
    public Canvas canVas;
    public GameObject canVasObject;
    public void Initialize(InventoryManager manager, CRMManager crmManagerInstance)
    {
        inventoryManager = manager;
        crmManager = crmManagerInstance;
        incomingOutgoingPanel.Initialize(inventoryManager);
    }
    public void ShowCRMPanel()
    {
        SetActivePanel("CRM");
    }

    public void ShowIncomingOutgoingPanel()
    {
        SetActivePanel("IncomingOut");
    }

    void Start()
    {
        crmManager = new CRMManager(inventoryManager);
        CreateCanvasAndPanels();
        CreateLeftSidebarButtons();
        CreateRightSidebarButtons();
        SetActivePanel("MainMenu");
        itemNameInput = CreateInputField(panels["Inventory"].transform, "ItemNameInput", new Vector2(0.3f, 0.7f), new Vector2(0.7f, 0.8f));
        newNameInput = CreateInputField(panels["Inventory"].transform, "NewNameInput", new Vector2(0.3f, 0.55f), new Vector2(0.7f, 0.65f));
        newDescriptionInput = CreateInputField(panels["Inventory"].transform, "NewDescriptionInput", new Vector2(0.3f, 0.4f), new Vector2(0.7f, 0.5f));
        newQuantityInput = CreateInputField(panels["Inventory"].transform, "NewQuantityInput", new Vector2(0.3f, 0.25f), new Vector2(0.7f, 0.35f));
        newLotInput = CreateInputField(panels["Inventory"].transform, "NewLotInput", new Vector2(0.3f, 0.1f), new Vector2(0.7f, 0.2f));
        newLastLotInput = CreateInputField(panels["Inventory"].transform, "NewLastLotInput", new Vector2(0.3f, -0.05f), new Vector2(0, 0.05f));
        searchTermInput = CreateInputField(panels["Inventory"].transform, "SearchTermInput", new Vector2(0.3f, -0.2f), new Vector2(0.7f, -0.1f));
    }

    private InputField CreateInputField(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject inputFieldObject = new GameObject(name);
        inputFieldObject.transform.SetParent(parent);
        RectTransform rectTransform = inputFieldObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;

        GameObject textArea = new GameObject("Text Area");
        textArea.transform.SetParent(inputFieldObject.transform);
        RectTransform textAreaRectTransform = textArea.AddComponent<RectTransform>();
        textAreaRectTransform.anchorMin = Vector2.zero;
        textAreaRectTransform.anchorMax = Vector2.one;
        textAreaRectTransform.anchoredPosition = Vector2.zero;
        textAreaRectTransform.sizeDelta = Vector2.zero;

        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        InputField inputField = inputFieldObject.AddComponent<InputField>();
        inputField.textComponent = CreateText(textArea.transform, "").GetComponent<Text>();
        inputField.textComponent.color = Color.black;

        return inputField;
    }

    private GameObject CreateText(Transform parent, string text)
    {
        GameObject textObject = new GameObject("Text");
        textObject.transform.SetParent(parent);
        Text buttonText = textObject.AddComponent<Text>();
        buttonText.text = text;
        buttonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        buttonText.fontSize = 24;
        buttonText.color = Color.white;
        buttonText.alignment = TextAnchor.MiddleCenter;

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;

        return textObject;
    }

    private void SetSubButtonAnchors(Button button, int index, int totalCount)
    {
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        float buttonHeight = 1f / totalCount;
        float buttonTopAnchor = 1f - index * buttonHeight;
        rectTransform.anchorMin = new Vector2(0.2f, buttonTopAnchor - buttonHeight);
        rectTransform.anchorMax = new Vector2(0.8f, buttonTopAnchor);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;
    }

    private void CreateCanvasAndPanels()
    {
        canVas.renderMode = RenderMode.ScreenSpaceOverlay;
        canVasObject.AddComponent<CanvasScaler>();
        canVasObject.AddComponent<GraphicRaycaster>();

        panels = new Dictionary<string, GameObject>
        {
            { "LotHandling", CreatePanel(canVas.transform, "LotHandling", new Vector2(0f, 1f), new Vector2(0.2f, 0f)) },
            { "DeviceLabeling", CreatePanel(canVas.transform, "DeviceLabeling", new Vector2(0f, 1f), new Vector2(0.2f, 0f)) },
            { "Inventory", CreatePanel(canVas.transform, "Inventory", new Vector2(0f, 1f), new Vector2(0.2f, 0f)) },
            { "CRM", CreatePanel(canVas.transform, "CRM", new Vector2(0, 1f), new Vector2(0.2f, 0f)) },
            { "MainMenu", CreatePanel(canVas.transform, "MainMenu", new Vector2(0, 1f), new Vector2(0.2f, 0f)) }
        };
    }

    private Dropdown CreateDropdown(Transform parent, string name, List<string> options, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject dropdownObject = new GameObject(name);
        dropdownObject.transform.SetParent(parent);
        RectTransform rectTransform = dropdownObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;

        Dropdown dropdown = dropdownObject.AddComponent<Dropdown>();
        dropdown.AddOptions(options);

        return dropdown;
    }

    private Toggle CreateToggle(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject toggleObject = new GameObject(name);
        toggleObject.transform.SetParent(parent);
        RectTransform rectTransform = toggleObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;

        Toggle toggle = toggleObject.AddComponent<Toggle>();

        return toggle;
    }

    private GameObject CreatePanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject panelObject = new GameObject(name);
        panelObject.transform.SetParent(parent);
        RectTransform rectTransform = panelObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;
        panelObject.SetActive(false);

        return panelObject;
    }

    public void SetActivePanel(string panelName)
    {
        foreach (var panel in panels)
        {
            panel.Value.SetActive(panel.Key == panelName);
        }
    }

    private void CreateLeftSidebarButtons()
    {
        string[] buttonNames = { "LotHandling", "DeviceLabel", "Inventory", "CRM" };
        for (int i = 0; i < buttonNames.Length; i++)
        {
            GameObject buttonObject = new GameObject(buttonNames[i] + "Button");
            buttonObject.transform.SetParent(panels["MainMenu"].transform);
            Button button = buttonObject.AddComponent<Button>();
            Image buttonImage = buttonObject.AddComponent<Image>();
            buttonImage.color = Color.gray;
            RectTransform rectTransform = buttonObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0f, 1f - (i + 1) * 0.25f);
            rectTransform.anchorMax = new Vector2(1f, (1f - i) * 0.25f);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
            int index = i;
            button.onClick.AddListener(() => SetActivePanel(buttonNames[index]));
            Text buttonText = CreateButtonText(buttonObject.transform, buttonNames[i]);
        }
    }

    private Text CreateButtonText(Transform parent, string text)
    {
        GameObject textObject = new GameObject("Text");
        textObject.transform.SetParent(parent);
        Text buttonText = textObject.AddComponent<Text>();
        buttonText.text = text;
        buttonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        buttonText.fontSize = 24;
        buttonText.color = Color.white;
        buttonText.alignment = TextAnchor.MiddleCenter;
        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;

        return buttonText;
    }

    private void CreateRightSidebarButtons()
    {
        Dictionary<string, List<string>> subButtons = new Dictionary<string, List<string>>
        {
            {"LotHandling", new List<string>{"Incoming/Outgoing", "Add/Remove Lot Inventory item", "Reprint"}},
            {"DeviceLabeling", new List<string>{"Shipment", "Preprep", "Add/Remove Device Label item", "Reprint"}},
            {"Inventory", new List<string>{"Update Inventory Item", "Get Item By Name", "Search Inventory Items"}},
            {"CRM", new List<string>{"Add CRM Entry", "Remove CRM Entry", "View CRM Entries"}}
        };

        foreach (var mainButton in subButtons)
        {
            for (int i = 0; i < mainButton.Value.Count; i++)
            {
                GameObject buttonObject = new GameObject(mainButton.Key + mainButton.Value[i] + "Button");
                buttonObject.transform.SetParent(panels[mainButton.Key].transform);
                Button button = buttonObject.AddComponent<Button>();
                button.transition = Selectable.Transition.ColorTint;
                button.colors = CreateColorBlock(new Color(0.3f, 0.3f, 0.3f), new Color(0.5f, 0.5f, 0.5f), new Color(0.1f, 0.1f, 0.1f));
                SetSubButtonAnchors(button, i, mainButton.Value.Count);
                int index = i;

                // Add onClick listeners with appropriate arguments for HandleInventoryMenuAction
                if (mainButton.Key == "Inventory")
                {
                    button.onClick.AddListener(() => inventoryManager.HandleInventoryMenuAction(mainButton.Value[index], itemNameInput.text, newNameInput.text, newDescriptionInput.text, newQuantityInput.text, newLotInput.text, newLastLotInput.text, searchTermInput.text));
                }
                else if (mainButton.Key == "CRM")
                {
                    button.onClick.AddListener(() => crmManager.HandleCRMAction(mainButton.Value[index]));
                }

                Text buttonText = CreateButtonText(buttonObject.transform, mainButton.Value[i]);
            }
        }
    }
    public void DisplayInventoryItem(InventoryItems.InventoryItem item)
    {
        GameObject itemDetailsPanel = GameObject.Find("ItemDetailsPanel");
        itemDetailsPanel.transform.Find("ItemName").GetComponent<Text>().text = item.Name;
        itemDetailsPanel.transform.Find("ItemDescription").GetComponent<Text>().text = item.Description;
        itemDetailsPanel.transform.Find("ItemQuantity").GetComponent<Text>().text = item.Quantity.ToString();
        itemDetailsPanel.transform.Find("ItemLot").GetComponent<Text>().text = item.LastLot.ToString();
        itemDetailsPanel.transform.Find("ItemLastLot").GetComponent<Text>().text = item.LastLot.ToString();
        itemDetailsPanel.SetActive(true);
    }

    public void UpdateInventoryTable(List<InventoryItems.LotItem> items)
    {
        GameObject inventoryTable = GameObject.Find("InventoryTable");
        foreach (Transform child in inventoryTable.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (InventoryItems.LotItem item in items)
        {
            GameObject rowPrefab = Resources.Load("InventoryRowPrefab") as GameObject;
            GameObject newRow = Instantiate(rowPrefab, inventoryTable.transform);
            newRow.transform.Find("ItemName").GetComponent<Text>().text = item.Name;
            newRow.transform.Find("ItemDescription").GetComponent<Text>().text = item.Description;
            newRow.transform.Find("ItemQuantity").GetComponent<Text>().text = item.Quantity.ToString();
            newRow.transform.Find("ItemLot").GetComponent<Text>().text = item.Lot.ToString();
            newRow.transform.Find("ItemLastLot").GetComponent<Text>().text = item.LastLot.ToString();
            newRow.transform.SetParent(inventoryTable.transform, false);
        }
    }

    private ColorBlock CreateColorBlock(Color normalColor, Color highlightedColor, Color pressedColor)
    {
        ColorBlock colorBlock = new ColorBlock();
        colorBlock.normalColor = normalColor;
        colorBlock.highlightedColor = highlightedColor;
        colorBlock.pressedColor = pressedColor;
        colorBlock.disabledColor = Color.gray;
        colorBlock.colorMultiplier = 1;
        colorBlock.fadeDuration = 0.1f;
        return colorBlock;
    }
}
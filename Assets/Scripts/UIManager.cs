using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Threading.Tasks;

public class UIManager : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public IncomingOutgoingPanel incomingOutgoingPanel;
    public Canvas canvas;
    public GameObject rightButtonParent;
    public GameObject leftButtonParent;
    public GameObject centerContentParent;
    public int maxRChildren;
    public int maxLChildren;
    public int maxCChildren;
    public string curView = "None";
    public string curViewKey = "L0R0";
    public string curViewLKey = "L0";
    public Dictionary<string, GameObject> uiObjects;

    public object LeanTween { get; private set; }

    public void UpdateButtonTexts(string buttonParent)
    {        
        var maxlength = buttonParent == "left" ? maxLChildren : maxRChildren;
        for (int i = 0; i < maxlength; i++)
        {
            if (buttonParent == "left")            
                leftButtonParent.transform.GetChild(i).gameObject.SetActive(true);            
            else
                rightButtonParent.transform.GetChild(i).gameObject.SetActive(true);
        }
        Button[] buttonArray = buttonParent == "left" ? leftButtonParent.GetComponentsInChildren<Button>() : rightButtonParent.GetComponentsInChildren<Button>();
        for (int i = 0; i < maxlength; i++)
        {
            string key;
           
            if (buttonParent == "left")
            {
                key = $"L{i}";
                if (AppManager.uiTextDictionary.ContainsKey(key) && !AppManager.uiTextDictionary.Keys.Any(k => k.StartsWith("R")))
                {
                    buttonArray[i].gameObject.SetActive(true);
                    buttonArray[i].GetComponentInChildren<TextMeshProUGUI>().text = AppManager.uiTextDictionary[key];
                }
                else if (buttonArray[i] != null)
                    buttonArray[i].gameObject.SetActive(false);
            }
            else
            {
                key = curViewLKey + $"R{i}";
                if (AppManager.uiTextDictionary.ContainsKey(key))
                {
                    buttonArray[i].gameObject.SetActive(true);
                    buttonArray[i].GetComponentInChildren<TextMeshProUGUI>().text = AppManager.uiTextDictionary[key];
                }
                else if(buttonArray[i] != null)
                    buttonArray[i].gameObject.SetActive(false);
            }
        }
    }
    private void EnableDropDown(bool enable)
    {
        uiObjects["Dropdown"].SetActive(enable);
        if (enable)
        {
            string key = curViewKey + "D0";
            if (AppManager.uiTextDictionary.ContainsKey(key))
            {
                uiObjects["Dropdown"].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = AppManager.uiTextDictionary[key];
            }
        }
    }
    public void OnToggleText(bool isOn)
    {
        if (curView == "LotHandling")
        {
            
            uiObjects["Toggle"].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = isOn ? "Incoming" : "Outgoing";
            if(curViewKey == "L0R0")
                uiObjects["actionButton"].GetComponentInChildren<TextMeshProUGUI>().text = isOn ? "Save Inc" : "Save Out";
            else if (curViewKey == "L0R1")
                uiObjects["actionButton"].GetComponentInChildren<TextMeshProUGUI>().text = isOn ? "Add" : "Remove";
            else
                uiObjects["actionButton"].GetComponentInChildren<TextMeshProUGUI>().text = isOn ? "Print Inc" : "Print Out";
        }
        else if (curView == "Shipment")
        {
            uiObjects["Toggle"].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = isOn ? "Add" : "Remove";
            uiObjects["actionButton"].GetComponentInChildren<TextMeshProUGUI>().text = isOn ? "Add LabelType" : "Remove LabelType";
        }
        else if (curView == "Inventory")
        {
            uiObjects["Toggle"].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = isOn ? "Add" : "Subtract";
            uiObjects["actionButton"].GetComponentInChildren<TextMeshProUGUI>().text = isOn ? "Add" : "Subtract";
        }
    }
    public void OnDropdownValueChanged(int selectedIndex)
    {
        if (curView == "Shipment")
        {
            string selectedProductSubHeading = IncomingOutgoingPanel.deviceLabels[selectedIndex].ProductSubHeading;
            string selectedProductHeading = IncomingOutgoingPanel.deviceLabels[selectedIndex].ProductHeading;
            uiObjects["LabelDescriptor"].GetComponent<TextMeshProUGUI>().text = "Product: " + selectedProductHeading + " - "+selectedProductSubHeading;
        }
    }
    private void EnableLblTitle(bool enable)
    {
        uiObjects["LabelDescriptor"].SetActive(enable);
    }
    private void EnableToggle(bool enable)
    {
        uiObjects["Toggle"].SetActive(enable);
        if (enable)
        {
            string key = curViewKey + "T0";
            if (AppManager.uiTextDictionary.ContainsKey(key))
            {
                uiObjects["Toggle"].GetComponentInChildren<TextMeshProUGUI>().text = AppManager.uiTextDictionary[key];
            }
        }
    }
    private void EnableAction(bool enable)
    {
        uiObjects["actionButton"].SetActive(enable);
        uiObjects["cancelButton"].SetActive(enable);
        if (enable)
        {
            string keyA1 = curViewKey + "A1";
            if (AppManager.uiTextDictionary.ContainsKey(keyA1))
            {
                uiObjects["actionButton"].GetComponentInChildren<TextMeshProUGUI>().text = AppManager.uiTextDictionary[keyA1];
            }
            string keyA2 = curViewKey + "A2";
            if (AppManager.uiTextDictionary.ContainsKey(keyA2))
            {
                uiObjects["cancelButton"].GetComponentInChildren<TextMeshProUGUI>().text = AppManager.uiTextDictionary[keyA2];
            }
        }
    }
    public void EnableRightButtons(bool enable)
    {
        uiObjects["rightButtons"].SetActive(enable);
    }
    public void EnableCenterContent(bool enable)
    {
        uiObjects["centerContent"].SetActive(enable);
    }
    private void EnableInputFields(int index)
    {
        for (int i = 0; i < 7; i++)
        {
            uiObjects[$"InputField{i}"].SetActive(i <= index ? true : false);
            if (i <= index)
            {
                string key = curViewKey + $"I{i}";
                if (AppManager.uiTextDictionary.ContainsKey(key))
                {                    
                    uiObjects[$"InputField{i}"].GetComponentInChildren<TextMeshProUGUI>().text = AppManager.uiTextDictionary[key];
                    SetInputFieldPlaceholder(uiObjects[$"InputField{i}"].GetComponent<TMP_InputField>(), "Enter " + AppManager.uiTextDictionary[key]);
                }
            }
        }
    }
    public void SetButtonColor(Button button, Color color)
    {
        ColorBlock colorBlock = button.colors;
        colorBlock.normalColor = color == Color.white ? new Color(0.2f, 0.4f, 0.8f) : color;
        colorBlock.highlightedColor = Color.Lerp(colorBlock.normalColor, Color.white, 0.5f);
        colorBlock.pressedColor = Color.Lerp(colorBlock.normalColor, Color.black, 0.5f);
        button.colors = colorBlock;
    }
    public static class TextToTexture
    {
        public static async Task<Texture2D> StringToTexture2D(string text, int width, int height, TMP_FontAsset fontAsset, int fontSize, Color textColor)
        {
            // Create a new TextMeshProUGUI object
            GameObject textObject = new GameObject("TextToTexture");
            TextMeshProUGUI textMeshPro = textObject.AddComponent<TextMeshProUGUI>();

            // Set TextMeshPro properties
            textMeshPro.text = text;
            textMeshPro.fontSize = fontSize;
            textMeshPro.font = fontAsset;
            textMeshPro.color = textColor;
            textMeshPro.alignment = TextAlignmentOptions.Center;
            textMeshPro.enableWordWrapping = true;

            // Create a new Canvas object
            GameObject canvasObject = new GameObject("TextToTextureCanvas");
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();

            // Set the TextMeshProUGUI object as a child of the Canvas object
            textObject.transform.SetParent(canvasObject.transform);

            // Create a new Camera object
            GameObject cameraObject = new GameObject("TextToTextureCamera");
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = height / 2;
            camera.transform.position = new Vector3(width / 2, height / 2, -10);

            // Wait for one frame to let the canvas update
            await Task.Delay(1);

            // Create a new RenderTexture
            RenderTexture renderTexture = new RenderTexture(width, height, 24);
            renderTexture.Create();

            // Render the canvas to the RenderTexture using the camera
            camera.targetTexture = renderTexture;
            camera.Render();

            // Convert the RenderTexture to a Texture2D
            Texture2D finalTexture = null;
            finalTexture = new Texture2D(width, height);
            RenderTexture.active = renderTexture;
            finalTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            finalTexture.Apply();
            RenderTexture.active = null;

            // Clean up the created objects
            Object.Destroy(textObject);
            Object.Destroy(canvasObject);
            Object.Destroy(cameraObject);
            Object.Destroy(renderTexture);

            return finalTexture;
        }
    }
    public void Initialize(InventoryManager manager, Canvas can, IncomingOutgoingPanel iop)
    {
        inventoryManager = manager;
        inventoryManager.uiManager = this;
        incomingOutgoingPanel = iop;
        canvas = can;
        InitializeUIObjects();
        SetDefaultButtonActions("left");
        SetDefaultButtonActions("right");

        Button[] leftButtonArray = leftButtonParent.GetComponentsInChildren<Button>();
        for (int i = 0; i < leftButtonArray.Length; i++)
        {
            SetButtonColor(leftButtonArray[i], Color.white);
        }

        Button[] rightButtonArray = rightButtonParent.GetComponentsInChildren<Button>();
        for (int i = 0; i < rightButtonArray.Length; i++)
        {
            SetButtonColor(rightButtonArray[i], Color.white);
        }
    }
    private void InitializeUIObjects()
    {
        uiObjects = new Dictionary<string, GameObject>();
        uiObjects["canvas"] = canvas.gameObject;
        uiObjects["leftButtons"] = canvas.transform.Find("Left Buttons").gameObject;
        leftButtonParent = uiObjects["leftButtons"];
        uiObjects["rightButtons"] = canvas.transform.Find("Right Buttons").gameObject;
        rightButtonParent = uiObjects["rightButtons"];
        uiObjects["centerContent"] = canvas.transform.Find("Center Content").gameObject;
        centerContentParent = uiObjects["centerContent"];

        maxRChildren = uiObjects["rightButtons"].transform.childCount;
        maxLChildren = uiObjects["leftButtons"].transform.childCount;

        for (int i = 0; i < uiObjects["leftButtons"].transform.childCount; i++)
        {
            uiObjects[$"L{i}"] = uiObjects["leftButtons"].transform.GetChild(i).gameObject;
        }

        for (int i = 0; i < uiObjects["rightButtons"].transform.childCount; i++)
        {
            uiObjects[$"R{i}"] = uiObjects["rightButtons"].transform.GetChild(i).gameObject;
        }

        uiObjects["Dropdown"] = uiObjects["centerContent"].transform.Find("Dropdown").gameObject;
        uiObjects["Toggle"] = uiObjects["centerContent"].transform.Find("Toggle").gameObject;
        uiObjects["Date"] = uiObjects["centerContent"].transform.Find("Date Display").gameObject;
        uiObjects["Title"] = uiObjects["centerContent"].transform.Find("Title").gameObject;
        uiObjects["LabelDescriptor"] = uiObjects["centerContent"].transform.Find("LabelDescriptor").gameObject;
        uiObjects["Dropdown"].GetComponent<TMP_Dropdown>().onValueChanged.AddListener(OnDropdownValueChanged);
        uiObjects["Toggle"].GetComponent<Toggle>().onValueChanged.AddListener(OnToggleText);

        int inputFieldIndex = 0;
        Transform currentInputField = uiObjects["centerContent"].transform.Find($"InputField (TMP) ({inputFieldIndex})");
        while (currentInputField != null)
        {
            uiObjects[$"InputField{inputFieldIndex}"] = currentInputField.gameObject;
            maxCChildren = inputFieldIndex;
            inputFieldIndex++;
            currentInputField = uiObjects["centerContent"].transform.Find($"InputField (TMP) ({inputFieldIndex})");
        }

        uiObjects["actionButton"] = uiObjects["centerContent"].transform.Find("Button (0)").gameObject;
        uiObjects["cancelButton"] = uiObjects["centerContent"].transform.Find("Button (1)").gameObject;
        EnableRightButtons(false);
        EnableCenterContent(false);
        UpdateButtonTexts("left");
    }
    public void UpdateCenterContent(string panelName, int action)
    {
        switch (panelName)
        {
            case "LotHandling":
                switch (action)
                {
                    case 0:
                        curViewKey = "L0R0";
                        SetUIElements("LotHandling", 0, true, true, false);
                        break;
                    case 1:
                        curViewKey = "L0R1";
                        SetUIElements("LotHandling", 1, false, true, false);
                        break;
                    case 2:
                        curViewKey = "L0R2";
                        SetUIElements("LotHandling", 0, true, true, false);
                        break;
                }
                break;
            case "Shipment":
                switch (action)
                {
                    case 0:
                        curViewKey = "L1R0";
                        SetUIElements("Shipment",  6, true, false, true);
                        break;
                    case 1:
                        curViewKey = "L1R1";
                        SetUIElements("Shipment", 2, false, true, false);
                        break;
                    case 2:
                        curViewKey = "L1R2";
                        SetUIElements("Shipment", 2, true, false, false);
                        break;
                }
                break;
            case "Inventory":
                switch (action)
                {
                    case 0:
                        curViewKey = "L2R0";
                        SetUIElements("Inventory", 0, true, true, false);
                        break;
                    case 1:
                        curViewKey = "L2R1";
                        SetUIElements("Inventory", 1, false, true, false);
                        break;
                }
                break;
        }
    }
    public void SetInputFieldPlaceholder(TMP_InputField inputField, string placeholderText)
    {
        TMP_Text placeholder = inputField.placeholder.GetComponent<TMP_Text>();
        placeholder.text = placeholderText;
        placeholder.fontSize = 14;
        placeholder.alignment = TextAlignmentOptions.Midline;
        placeholder.color = new Color(0.5f, 0.5f, 0.5f, 1f);
    }
    private void SetUIElements(string panelName, int inputFieldsCount, bool isDropDown, bool isToggle, bool isLblName)
    {
        EnableDropDown(isDropDown);
        EnableLblTitle(isLblName);
        EnableToggle(isToggle);
        EnableInputFields(inputFieldsCount);
        EnableAction(true);

        uiObjects["Title"].GetComponent<TextMeshProUGUI>().text = panelName;
        uiObjects["Title"].GetComponent<TextMeshProUGUI>().fontSize = 24;
        uiObjects["Title"].GetComponent<TextMeshProUGUI>().color = new Color(0.2f, 0.4f, 0.8f);

        switch (panelName)
        {
            case "LotHandling":
                uiObjects["Title"].GetComponent<TextMeshProUGUI>().text = "Lot Handling";
                break;
            case "Shipment":
                uiObjects["Title"].GetComponent<TextMeshProUGUI>().text = "Shipment";
                break;
            case "Inventory":
                uiObjects["Title"].GetComponent<TextMeshProUGUI>().text = "Inventory";
                break;
        }
    }
  
    public void SetDefaultButtonActions(string buttonParent)
    {
        Button[] buttonArray = buttonParent == "left" ? leftButtonParent.GetComponentsInChildren<Button>() : rightButtonParent.GetComponentsInChildren<Button>();
        if (buttonParent == "left")
        {
            buttonArray[0].onClick.AddListener(() => { incomingOutgoingPanel.HandleLeftButtonAction("LotHandling"); });
            buttonArray[1].onClick.AddListener(() => { incomingOutgoingPanel.HandleLeftButtonAction("Shipment");  });
            buttonArray[2].onClick.AddListener(() => { incomingOutgoingPanel.HandleLeftButtonAction("Inventory");  });
        }
        else
        {
            buttonArray[0].onClick.AddListener(() => { incomingOutgoingPanel.HandleRightButtonAction(curView, 0);});
            buttonArray[1].onClick.AddListener(() => { incomingOutgoingPanel.HandleRightButtonAction(curView, 1);  });
            buttonArray[2].onClick.AddListener(() => { incomingOutgoingPanel.HandleRightButtonAction(curView, 2);  });
        }
    }
    public void DisplaySearchResults(List<InventoryItem> searchResults)
    {
        if (searchResults != null && searchResults.Count > 0)
        {
            // Display search results in a suitable UI element, such as a list or table
            // This will depend on your specific UI implementation
        }
        else
        {
            Debug.Log("No search results found");
        }
    }
    public void EnableAllLeftButtons()
    {
        Button[] leftButtonArray = leftButtonParent.GetComponentsInChildren<Button>();
        for (int i = 0; i < leftButtonArray.Length; i++)
        {
            leftButtonArray[i].interactable = true;
        }
    }
    public void UpdateUI()
    {
        incomingOutgoingPanel.UpdateUI();
    }
}
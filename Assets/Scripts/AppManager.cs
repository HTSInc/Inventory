using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AppManager : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public CRMManager crmManager;
    public UIManager uIManager;
    public IncomingOutgoingPanel IOP;
    public static Canvas can;
    public Canvas realCan;
    public static TMP_FontAsset tmpFont;
    public TMP_FontAsset tmpFontSet;

    public static Dictionary<string, string> uiTextDictionary = new Dictionary<string, string>
{
    { "L0", "LotHandling" },
    { "L1", "Shipment" },
    { "L2", "Inventory" },
    { "L0R0", "Add Lot" },
    { "L0R0D0", "Lot Item" },
    { "L0R0T0", "Incoming" },
    { "L0R0I0", "Print Quantity" },
    { "L0R0A1", "Save Inc" },
    { "L0R0A2", "Cancel" },
    { "L0R1", "Modify LotTypes" },
    { "L0R1T0", "Add" },
    { "L0R1I0", "Name" },
    { "L0R1I1", "Description" },
    { "L0R1A1", "Add" },
    { "L0R1A2", "Cancel" },
    { "L0R2", "Reprint Lot Labels" },
    { "L0R2D0", "Lot Item" },
    { "L0R2T0", "Incoming" },
    { "L0R2I0", "Print Quantity" },
    { "L0R2A1", "Print Inc" },
    { "L0R2A2", "Cancel" },
    { "L1R0", "Ship Device" },
    { "L1R0D0", "Product" },
    { "L1R0I0", "Order #" },
    { "L1R0I1", "Account #" },
    { "L1R0I2", "Label Quantity" },
    { "L1R0A1", "Save & Ship" },
    { "L1R0A2", "Cancel" },
    { "L1R1", "Modify LabelType" },
    { "L1R1T0", "Add" },
    { "L1R1I0", "Product" },
    { "L1R1I1", "Product Heading" },
    { "L1R1I2", "Label #" },
    { "L1R1A1", "Add LabelType" },
    { "L1R1A2", "Cancel" },
    { "L1R2", "Reprint Device Label" },
    { "L1R2D0", "Product" },
    { "L1R2I1", "Order #" },
    { "L1R2I2", "Account #" },
    { "L1R2I3", "Quantity" },
    { "L1R2A1", "Print" },
    { "L1R2A2", "Cancel" },
    { "L2R0", "Current Inventory" },
    { "L2R0D0", "Inventory Item" },
    { "L2R0T0", "Add" },
    { "L2R0I0", "Quantity" },
    { "L2R0A1", "Save" },
    { "L2R0A2", "Cancel" },
    { "L2R1", "Modify InventoryTypes" },
    { "L2R1T0", "Inventory Item" },
    { "L2R1I0", "Name" },
    { "L2R1I1", "Description" },
    { "L2R1A1", "Save" },
    { "L2R1A2", "Cancel" } };


    private void Start()
    {
        can = realCan;
        tmpFont = tmpFontSet;
        StartCoroutine(InitializeManagers());
    }
    public void SetAppResolution()
    {
        Screen.SetResolution(1920, 1080, false);
        SetCanvasResolution();
    }
    private void SetCanvasResolution()
    {
        can.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        can.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
        can.GetComponent<CanvasScaler>().screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        can.GetComponent<CanvasScaler>().matchWidthOrHeight = 1f;
    }
    private IEnumerator InitializeManagers()
    {
        yield return new WaitForEndOfFrame();
        GameObject inventoryManagerGO = new GameObject("InventoryManager");
        inventoryManagerGO.transform.parent = transform;
        inventoryManager = inventoryManagerGO.AddComponent<InventoryManager>();   
        yield return new WaitForEndOfFrame();
        GameObject crmManagerGO = new GameObject("CRMManager");
        crmManagerGO.transform.parent = transform;
        crmManager = crmManagerGO.AddComponent<CRMManager>();
        crmManager.Initialize(inventoryManager);
        yield return new WaitForEndOfFrame();
        SetAppResolution();
        // Create UIManager instance
        GameObject uIManagerGO = new GameObject("UIManager");
        uIManagerGO.transform.parent = transform;
        uIManager = uIManagerGO.AddComponent<UIManager>();
        GameObject iopGO = new GameObject("iopManager");
        iopGO.transform.parent = transform;
        IOP = iopGO.AddComponent<IncomingOutgoingPanel>();
        uIManager.Initialize(inventoryManager, can, IOP);
        yield return new WaitForEndOfFrame();
        IOP.Initialize(inventoryManager, uIManager);
        inventoryManager.incomingOutgoingPanel = IOP;
    }
}

public static class TransformDeepChildExtension
{
    public static Transform FindDeepChild(this Transform parent, string name)
    {
        Transform result = parent.Find(name);

        if (result != null)
        {
            return result;
        }

        foreach (Transform child in parent)
        {
            result = child.FindDeepChild(name);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }
}


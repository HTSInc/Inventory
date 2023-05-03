using System.Collections;
using System.Collections.Generic;
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

    public static Dictionary<string, string> uiTextDictionary = new Dictionary<string, string> { { "L0", "LotHandling" }, { "L1", "Shipment" }, { "L2", "Inventory" }, { "L0R0", "In/OutLot" }, { "L0R0C0", "Item" }, { "L0R0C1", "Incoming" }, { "L0R0C2", "PrintQuantity" }, { "L0R0A1", "Save&Print" }, { "L0R0A2", "Cancel" }, { "L0R1", "Add/RemoveLotInventoryitem" }, { "L0R1C0", "Name" }, { "L0R1C1", "Description" }, { "L0R1A1", "Add/Remove" }, { "L0R1A2", "Cancel" }, { "L0R2", "Reprint" }, { "L0R2C0", "Item" }, { "L0R2C1", "Incoming" }, { "L0R2C2", "PrintQuantity" }, { "L0R2A1", "Print" }, { "L0R2A2", "Cancel" }, { "L1R0", "Outgoing" }, { "L1R0C0", "Product" }, { "L1R0C1", "Order#" }, { "L1R0C2", "Account#" }, { "L1R0C3", "Quantity" }, { "L1R0A1", "Save&Ship" }, { "L1R0A2", "Cancel" }, { "L1R1", "Add/RemoveDeviceLabelitem" }, { "L1R1C0", "Add" }, { "L1R1C1", "ProductHeading" }, { "L1R1C2", "ProductSubheading" }, { "L1R1C3", "LabelName" }, { "L1R1A1", "Save" }, { "L1R1A2", "Cancel" }, { "L1R2", "Reprint" }, { "L1R2C0", "Product" }, { "L1R2C1", "Order#" }, { "L1R2C2", "Account#" }, { "L1R2C3", "ShipmentQuantity" }, { "L1R2A1", "Print" }, { "L1R2A2", "Cancel" }, { "L2R0", "Add/RemovefromcurrentInventory" }, { "L2R0C0", "Item" }, { "L2R0C1", "Add" }, { "L2R0C2", "Quantity" }, { "L2R0A1", "Save" }, { "L2R0A2", "Cancel" }, { "L2R1", "Add/RemoveInventoryitems" }, { "L2R1C0", "Add" }, { "L2R1C1", "Name" }, { "L2R1C2", "Description" }, { "L2R1A1", "Save" }, { "L2R1A2", "Cancel" } };

    private void Start()
    {
        can = realCan;
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


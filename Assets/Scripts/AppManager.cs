using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AppManager : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public CRMManager crmManager;
    public UIManager uIManager;
    public Canvas canVas;
    private void Start()
    {
        StartCoroutine(InitializeManagers());
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
        GameObject UIManagerGO = new GameObject("UIManager");
        UIManagerGO.transform.parent = transform;
        uIManager = UIManagerGO.AddComponent<UIManager>();
        uIManager.Initialize(inventoryManager, crmManager);
    }
}
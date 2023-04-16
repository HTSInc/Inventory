using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CRMPanel : MonoBehaviour
{
    public CRMManager crmManager;
    public InputField callerNameInput, phoneNumberInput, notesInput, callTypeInput, accountNumberInput, employeeNameInput, productOrderedInput, orderNumberInput;
    public Button addEntryButton, removeEntryButton, cancelButton;

    public void Initialize(CRMManager manager)
    {
        crmManager = manager;
    }

    public void OnAddEntry()
    {
        CRMEntry newEntry = new CRMEntry(
            callerNameInput.text,
            phoneNumberInput.text,
            notesInput.text,
            callTypeInput.text,
            accountNumberInput.text,
            employeeNameInput.text,
            productOrderedInput.text,
            orderNumberInput.text
        );
        crmManager.AddEntry(newEntry);
    }


    public void OnRemoveEntry()
    {
        CRMEntry entryToRemove = crmManager.GetEntriesByOrderNumber(orderNumberInput.text).FirstOrDefault();
        if (entryToRemove != null)
        {
            crmManager.RemoveEntry(entryToRemove);
        }
    }

    public void OnCancel()
    {
        gameObject.SetActive(false);
    }
}
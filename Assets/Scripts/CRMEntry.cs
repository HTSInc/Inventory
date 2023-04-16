using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Data;
using Mono.Data.SqliteClient;

public class CRMEntry : MonoBehaviour
{
    public string CallerName, PhoneNumber, Notes, CallType, AccountNumber, EmployeeName, ProductOrdered, OrderNumber;
    public CRMEntry(string c, string p, string n, string t, string a, string e, string pd, string o)
    {
        CallerName = c;
        PhoneNumber = p;
        Notes = n;
        CallType = t;
        AccountNumber = a;
        EmployeeName = e;
        ProductOrdered = pd;
        OrderNumber = o;
    }
    public void UpdateCallerName(string newName)
    {
        CallerName = newName;
    }
    public void UpdatePhoneNumber(string newPhoneNumber)
    {
        PhoneNumber = newPhoneNumber;
    }
    public void UpdateNotes(string newNotes)
    {
        Notes = newNotes;
    }
    public void UpdateCallType(string newCallType)
    {
        CallType = newCallType;
    }
    public void UpdateAccountNumber(string newAccountNumber)
    {
        AccountNumber = newAccountNumber;
    }
    public void UpdateEmployeeName(string newEmployeeName)
    {
        EmployeeName = newEmployeeName;
    }
    public void UpdateProductOrdered(string newProductOrdered)
    {
        ProductOrdered = newProductOrdered;
    }
    public void UpdateOrderNumber(string newOrderNumber)
    {
        OrderNumber = newOrderNumber;
    }

}

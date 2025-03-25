using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Inventory : MonoBehaviour
{
    public List<string> items = new List<string>();
    public int maxInventorySize = 5;
    public int currentItemIndex = 0;

    public bool AddItem(string itemName)
    {
        if(items.Count < maxInventorySize)
        {
            items.Add(itemName);
            return true;
        } 
        return false;
    }

    public string GetCurrentItemName()
    {
        if (items.Count > 0)
        {
            return items[currentItemIndex];
        }
        return "";
    }

    public void RemoveItem(string itemName)
    {
        if (items.Contains(itemName))
        {
            items.Remove(itemName);
            currentItemIndex = Mathf.Clamp(currentItemIndex, 0, items.Count - 1);
        }
    }

    public void SwitchItem(int index)
    {
        if(index >= 0 && index < items.Count)
        {
            currentItemIndex = index;
        }
    }
}

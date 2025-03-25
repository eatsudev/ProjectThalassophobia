using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchItem(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchItem(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchItem(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SwitchItem(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SwitchItem(4);
    }
}

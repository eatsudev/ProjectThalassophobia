using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<string> inventory = new List<string>();
    public int maxItems = 5;

    public void AddItem(string itemName)
    {
        if (inventory.Count < maxItems)
        {
            inventory.Add(itemName);
            Debug.Log(itemName + " added to inventory!");
        }
        else
        {
            Debug.Log("Inventory Full!");
        }
    }
}

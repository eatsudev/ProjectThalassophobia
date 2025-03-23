using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Text inventoryText;
    public Inventory inventory;

    void Update()
    {
        inventoryText.text = "Obtained: " + string.Join(", ", inventory.inventory);
    }
}

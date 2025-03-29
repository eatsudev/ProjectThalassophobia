using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventorySlotPrefab;
    private ItemPickup itemPickup;
    public Transform inventoryPanel;

    private List<GameObject> itemSlots = new List<GameObject>();
    private const int MAX_ITEMS = 5;

    void Start()
    {
        itemPickup = FindObjectOfType<ItemPickup>();
        if (itemPickup == null)
        {
            Debug.LogError("ItemPickup script not found.");
        }

        InitializeInventoryUI();
        UpdateInventoryUI();
    }

    void Update()
    {
        //ViewItemName();
    }

    /*void ViewItemName()
    {
        if (inventoryText != null && itemPickup != null)
        {
            if (itemPickup.inventory.Count > 0 && itemPickup.currentItemIndex >= 0)
            {
                Item heldItem = itemPickup.GetHeldItem();
                if (heldItem != null)
                {
                    inventoryText.text = "Item: " + heldItem.itemName;
                    return;
                }
            }
        }
        inventoryText.text = "Item: None";
    }*/

    private void InitializeInventoryUI()
    {
        itemSlots.Clear();

        for (int i = 0; i < inventoryPanel.childCount; i++)
        {
            itemSlots.Add(inventoryPanel.GetChild(i).gameObject);
        }
    }

    public void UpdateInventoryUI()
    {
        if (itemPickup == null || inventoryPanel == null) return;

        for (int i = 0; i < itemSlots.Count; i++)
        {
            Transform iconTransform = itemSlots[i].transform.Find("InventoryIcon" + (i + 1));

            if (i < itemPickup.inventory.Count && itemPickup.inventory[i] != null)
            {
                if (iconTransform != null)
                {
                    iconTransform.GetComponent<Image>().sprite = itemPickup.inventory[i].icon;
                    iconTransform.gameObject.SetActive(true);
                }
            }
            else
            {
                if (iconTransform != null)
                {
                    iconTransform.gameObject.SetActive(false);
                }
            }
        }
    }

    public void RemoveItem(Item item)
    {
        if (itemPickup.inventory.Contains(item))
        {
            int itemIndex = itemPickup.inventory.IndexOf(item);

            itemPickup.inventory.RemoveAt(itemIndex);
            Destroy(item.gameObject);

            while (itemPickup.inventory.Count > MAX_ITEMS)
            {
                itemPickup.inventory.RemoveAt(itemPickup.inventory.Count - 1);
            }

            UpdateInventoryUI();
            itemPickup.EquipNextAvailableItem();
        }
    }
}

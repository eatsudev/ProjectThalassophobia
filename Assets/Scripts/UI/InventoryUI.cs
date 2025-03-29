using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
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

            if (iconTransform != null)
            {
                if (i < itemPickup.inventory.Count)
                {
                    if (itemPickup.inventory[i] != null)
                    {
                        iconTransform.GetComponent<Image>().sprite = itemPickup.inventory[i].icon;
                        iconTransform.gameObject.SetActive(true);
                    }
                    else
                    {
                        iconTransform.gameObject.SetActive(false);
                    }
                }
                else
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

            UpdateInventoryUI();

            itemPickup.EquipNextAvailableItem();
        }
    }
}

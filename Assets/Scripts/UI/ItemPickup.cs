using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPickup : MonoBehaviour
{
    public float pickupRange = 3f;
    public LayerMask itemLayer;
    public Transform cameraTransform;
    public Transform itemHolder;
    public Text itemNameUI;

    public List<Item> inventory = new List<Item>();
    public int maxItems = 5;
    public int currentItemIndex = -1;
    public float itemSize = 1f;

    public List<Keycard> keycards = new List<Keycard>();
    public AudioSource pickupSFX;
    public Text heldItemText;

    private void Start()
    {
        AutoPickupRedKeycard();
    }

    void Update()
    {
        ShowItemNameUI();

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPickupItem();
        }

        SwitchItems();

    }

    void ShowItemNameUI()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, pickupRange, itemLayer))
        {
            Item item = hit.collider.GetComponent<Item>();
            if (item != null)
            {
                itemNameUI.text = item.itemName;
                itemNameUI.enabled = true;
                return;
            }
        }
        itemNameUI.enabled = false;
    }

    void TryPickupItem()
    {
        int emptySlot = inventory.IndexOf(null);

        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, pickupRange, itemLayer))
        {
            Item item = hit.collider.GetComponent<Item>();
            if (item != null)
            {
                Item newItem = Instantiate(item, itemHolder);
                newItem.transform.position = itemHolder.position;
                newItem.transform.rotation = itemHolder.rotation;
                newItem.transform.localScale = Vector3.one * itemSize;
                newItem.gameObject.SetActive(false);

                if (emptySlot != -1)
                {
                    inventory[emptySlot] = newItem;
                }
                else if (inventory.Count < maxItems)
                {
                    inventory.Add(newItem);
                }
                else
                {
                    Debug.LogWarning("Inventory is full!");
                    return;
                }

                Destroy(hit.collider.gameObject);

                if (currentItemIndex == -1 || inventory[currentItemIndex] == null)
                {
                    EquipNextAvailableItem();
                }
                pickupSFX.Play();
                FindObjectOfType<InventoryUI>().UpdateInventoryUI();
                UpdateHeldItemUI();
            }
        }
    }

    void AutoPickupRedKeycard()
    {
        GameObject redkeycardObj = GameObject.Find("Red");
        if(redkeycardObj != null)
        {
            Item item = redkeycardObj.GetComponent<Item>();
            if (item != null)
            {
                Item newItem = Instantiate(item, itemHolder);
                newItem.transform.position = itemHolder.position;
                newItem.transform.rotation = itemHolder.rotation;
                newItem.transform.localScale = Vector3.one * itemSize;
                newItem.gameObject.SetActive(false);

                if(inventory.Count < maxItems)
                {
                    inventory.Add(newItem);
                    currentItemIndex = inventory.Count - 1;
                    EquipItem(currentItemIndex);
                }

                Destroy(redkeycardObj);
                Debug.Log("red keycard obtained");
                FindObjectOfType<InventoryUI>().UpdateInventoryUI();
            }
        }
        else
        {
            Debug.LogWarning("red keycard not found");
        }
    }

    void SwitchItems()
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                if (inventory[i] != null)
                {
                    EquipItem(i);
                }
                else
                {
                    Debug.LogWarning("Attempted to equip an empty slot!");
                }
            }
        }
    }

    void EquipItem(int index)
    {
        if (index < 0 || index >= inventory.Count)
        {
            Debug.LogWarning("Invalid index selected! Index: " + index);
            return;
        }

        if (inventory[index] == null)
        {
            Debug.LogWarning("No item in this slot!");
            return;
        }

        if (currentItemIndex >= 0 && currentItemIndex < inventory.Count && inventory[currentItemIndex] != null)
        {
            inventory[currentItemIndex].gameObject.SetActive(false);
        }

        currentItemIndex = index;
        inventory[currentItemIndex].gameObject.SetActive(true);
        UpdateHeldItemUI();
    }

    public void EquipNextAvailableItem()
    {
        if (inventory.Count == 0)
        {
            Debug.LogWarning("EquipNextAvailableItem called, but inventory is empty.");
            currentItemIndex = -1;
            return;
        }

        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i] != null)
            {
                EquipItem(i);
                return;
            }
        }

        Debug.LogWarning("No valid items left in inventory.");
        currentItemIndex = -1;
    }

    public void PickupKeycard(Keycard keycard)
    {
        keycards.Add(keycard);
        Debug.Log("Stored Keycard: " + keycard.keycardID);
    }

    public bool HasKeycard(string id)
    {
        foreach (Keycard keycard in keycards)
        {
            if (keycard.keycardID == id)
                return true;
        }
        return false;
    }

    public Item GetHeldItem()
    {
        if (inventory.Count > 0 && currentItemIndex >= 0 && currentItemIndex < inventory.Count)
        {
            Debug.Log("Currently holding: " + inventory[currentItemIndex].name);
            return inventory[currentItemIndex];
        }

        Debug.Log("No item is being held.");
        return null;
    }

    public void RemoveItem(Item item)
    {
        if (inventory.Contains(item))
        {
            int index = inventory.IndexOf(item);
            inventory.RemoveAt(index);
            Destroy(item.gameObject);
            Debug.Log(item.itemName + " has been removed from inventory.");

            FindObjectOfType<InventoryUI>().UpdateInventoryUI();

            if (currentItemIndex >= inventory.Count)
            {
                currentItemIndex = inventory.Count - 1;
            }

            EquipNextAvailableItem();
            UpdateHeldItemUI();
        }
    }

    void UpdateHeldItemUI()
    {
        if (currentItemIndex >= 0 && currentItemIndex < inventory.Count && inventory[currentItemIndex] != null)
        {
            heldItemText.text = "Currently Holding: " + inventory[currentItemIndex].itemName;
        }
        else
        {
            heldItemText.text = "Currently Holding: -";
        }
    }
}

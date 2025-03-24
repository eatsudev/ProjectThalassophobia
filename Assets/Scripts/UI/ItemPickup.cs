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

    private List<GameObject> inventory = new List<GameObject>();
    private int maxItems = 5;
    private int currentItemIndex = -1;

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
        if (inventory.Count >= maxItems) return; 

        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, pickupRange, itemLayer))
        {
            Item item = hit.collider.GetComponent<Item>();
            if (item != null)
            {
                GameObject newItem = Instantiate(hit.collider.gameObject, itemHolder);
                newItem.transform.localPosition = itemHolder.position;
                newItem.transform.localRotation = itemHolder.rotation;
                newItem.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                newItem.SetActive(false); 
                inventory.Add(newItem);

                Destroy(hit.collider.gameObject); 
                if (inventory.Count == 1)
                {
                    currentItemIndex = 0;
                    inventory[0].SetActive(true);
                }
            }
        }
    }

    void SwitchItems()
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                EquipItem(i);
            }
        }
    }

    void EquipItem(int index)
    {
        if (index < inventory.Count && currentItemIndex != index)
        {
            if (currentItemIndex >= 0) inventory[currentItemIndex].SetActive(false);
            currentItemIndex = index;
            inventory[currentItemIndex].SetActive(true);
        }
    }
}

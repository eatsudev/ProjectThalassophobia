using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemPickup : MonoBehaviour
{
    public float pickupRange = 3f;
    public LayerMask itemLayer = 1 << 0;
    public Transform cameraTransform;
    public Inventory inventory;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPickupItem();
        }
    }

    void TryPickupItem()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, pickupRange, itemLayer))
        {
            Item item = hit.collider.GetComponent<Item>();
            if (item != null)
            {
                inventory.AddItem(item.itemName);
                Destroy(hit.collider.gameObject);
            }
        }
    }
}

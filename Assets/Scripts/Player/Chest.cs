using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public string requiredKeycardID; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ItemPickup itemPickup = other.GetComponent<ItemPickup>();
            if (itemPickup != null && itemPickup.HasKeycard(requiredKeycardID))
            {
                OpenChest();
            }
            else
            {
                Debug.Log("Chest is locked! Requires Keycard " + requiredKeycardID);
            }
        }
    }

    private void OpenChest()
    {
        Debug.Log("Chest " + requiredKeycardID + " opened!");
        // Add animation
        
    }
}

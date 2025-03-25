using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keycard : MonoBehaviour
{
    public string keycardID;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ItemPickup itemPickup = other.GetComponent<ItemPickup>();
            if (itemPickup != null)
            {
                itemPickup.PickupKeycard(this);
                Debug.Log("Picked up Keycard: " + keycardID);
                Destroy(gameObject);
            }
        }
    }
}

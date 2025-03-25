using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenTank : MonoBehaviour
{
    public string itemName = "OxygenTank";
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Inventory inventory = other.GetComponent<Inventory>();
            if(inventory != null)
            {
                if (inventory.AddItem(itemName))
                {
                    Debug.Log("picked up oxygen tank");
                    Destroy(gameObject);
                }
                else
                {
                    Debug.Log("inventory full");
                }
            }
        }
    }
}

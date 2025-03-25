using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenTank : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Inventory inventory = other.GetComponent<Inventory>();
            if (inventory != null)
            {
                bool added = inventory.AddItem("OxygenTank");

                if (added)
                {
                    Debug.Log("Picked up Oxygen Tank!");
                    Destroy(gameObject);
                }
                else
                {
                    Debug.Log("Inventory Full! Cannot pick up Oxygen Tank.");
                }
            }
        }
    }
}

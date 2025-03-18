using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenTank : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Oxygen oxygenSystem = other.GetComponent<Oxygen>();
            if (oxygenSystem != null)
            {
                oxygenSystem.hasOxygenTank = true;
                Debug.Log("Picked up Oxygen Tank!");
                Destroy(gameObject); 
            }
        }
    }
}

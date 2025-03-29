using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Oxygen : MonoBehaviour
{
    public float maxOxygen = 100f;
    private float currentOxygen;
    public float oxygenDepletionRate = 1f; 
    public float depletionInterval = 10f;
    public bool hasOxygenTank = false;
    public Slider oxygenBar; 
    public Inventory inventory;
    public string oxygenTankItemName = "OxygenTank";
    public ItemPickup itemPickup;

    private void Start()
    {
        inventory = GetComponent<Inventory>();
        currentOxygen = maxOxygen;
        StartCoroutine(DecreaseOxygen());
    }

    private void Update()
    {
        if (itemPickup.inventory.Count > 0)
        {
            hasOxygenTank = IsHoldingOxygenTank();
        }
        else
        {
            hasOxygenTank = false;
        }

        if (Input.GetKeyDown(KeyCode.R) && hasOxygenTank)
        {
            ReloadOxygen();
        }
    }
    private IEnumerator DecreaseOxygen()
    {
        while (currentOxygen > 0)
        {
            yield return new WaitForSeconds(depletionInterval);
            currentOxygen -= oxygenDepletionRate;
            currentOxygen = Mathf.Clamp(currentOxygen, 0, maxOxygen);
            UpdateOxygenUI();

            if (currentOxygen <= 0)
            {
                PlayerDrown(); 
            }
        }
    }


    private void ReloadOxygen()
    {
        if (!hasOxygenTank)
        {
            Debug.Log("No Oxygen Tank equipped!");
            return;
        }

        Debug.Log("Refilling Oxygen...");
        currentOxygen = maxOxygen;
        hasOxygenTank = false;

        if (itemPickup.currentItemIndex >= 0 && itemPickup.currentItemIndex < itemPickup.inventory.Count)
        {
            Item usedItem = itemPickup.inventory[itemPickup.currentItemIndex];

            if (usedItem != null && usedItem.name.Contains("OxygenTank"))
            {
                itemPickup.RemoveItem(usedItem);
                Debug.Log("Oxygen Tank used and removed.");
            }
            else
            {
                Debug.Log("Error: No Oxygen Tank found in inventory.");
            }
        }
        else
        {
            Debug.LogWarning("Invalid item index or inventory is empty.");
        }

        if (itemPickup.currentItemIndex >= itemPickup.inventory.Count)
        {
            itemPickup.currentItemIndex = itemPickup.inventory.Count - 1;
        }

        itemPickup.EquipNextAvailableItem();
        UpdateOxygenUI();
    }

    private bool IsHoldingOxygenTank()
    {
        if (itemPickup.inventory.Count > 0 && itemPickup.currentItemIndex >= 0 && itemPickup.currentItemIndex < itemPickup.inventory.Count)
        {
            Item heldItem = itemPickup.inventory[itemPickup.currentItemIndex];
            if (heldItem != null)
            {
                Debug.Log("Currently Holding: " + heldItem.name);
                if (heldItem.name.Contains("OxygenTank"))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void UpdateOxygenUI()
    {
        if (oxygenBar != null)
        {
            oxygenBar.value = currentOxygen / maxOxygen;
        }
    }

    private void PlayerDrown()
    {
        Debug.Log("Player has drowned");
        //death logic
    }
}

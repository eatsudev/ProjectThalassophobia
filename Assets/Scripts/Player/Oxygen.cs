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

    public Slider oxygenBar; 
    public Inventory inventory;

    private void Start()
    {
        inventory = GetComponent<Inventory>();
        currentOxygen = maxOxygen;
        StartCoroutine(DecreaseOxygen());
        
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            TryReloadOxygen();
        }
    }

    private void TryReloadOxygen()
    {
        if (inventory != null)
        {
            string equippedItem = inventory.GetCurrentItemName();
            Debug.Log("Currently equipped item: " + equippedItem);

            if (equippedItem == "OxygenTank")
            {
                currentOxygen = maxOxygen;
                inventory.RemoveItem("OxygenTank");
                Debug.Log("Oxygen Refilled!");
                UpdateOxygenUI();
            }
            else
            {
                Debug.Log("No Oxygen Tank equipped!");
            }
        }
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

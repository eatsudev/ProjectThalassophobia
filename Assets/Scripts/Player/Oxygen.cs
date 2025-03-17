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

    private void Start()
    {
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
        if (Input.GetKeyDown(KeyCode.R) && hasOxygenTank)
        {
            ReloadOxygen();
        }
    }

    private void ReloadOxygen()
    {
        currentOxygen = maxOxygen;
        hasOxygenTank = false; 
        Debug.Log("Oxygen Refilled");
        UpdateOxygenUI();
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

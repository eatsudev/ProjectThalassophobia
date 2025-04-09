using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirBladder : MonoBehaviour
{
    public float initialBuoyancyForce = 5f;
    public float maxBuoyancyForce = 20f;
    public float accelerationRate = 1.5f;
    public bool isUsingAirBladder = false;
    public bool hasAirBladder = false;

    public GameObject winUI;
    public GameObject defaultHand;
    public GameObject airBladderHand;

    private Rigidbody rb;
    private float currentBuoyancyForce;
    public GameObject itemHolder;
    public ItemPickup itemPickup;
    public GameObject airBladderUI;
    public GameObject surface;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        winUI.SetActive(false);
        airBladderHand.SetActive(false);
    }

    void Update()
    {
        if (itemPickup.inventory.Count > 0)
        {
            hasAirBladder = IsHoldingAirBladder();
        }
        else
        {
            hasAirBladder = false;
        }

        if (Input.GetKeyDown(KeyCode.Space) && hasAirBladder)
        {
            UseAirBladder();
        }
    }

    void UseAirBladder()
    {
        if (!isUsingAirBladder && hasAirBladder)
        {
            isUsingAirBladder = true;
            currentBuoyancyForce = initialBuoyancyForce;
            SwapHandModel(true);
            rb.isKinematic = false;
            StartCoroutine(RiseToSurface());
            airBladderUI.SetActive(false);
            surface.SetActive(true);
            itemHolder.SetActive(false);
        }
    }

    IEnumerator RiseToSurface()
    {
        while (isUsingAirBladder)
        {
            rb.velocity = new Vector3(rb.velocity.x, currentBuoyancyForce, rb.velocity.z);
            currentBuoyancyForce = Mathf.Min(currentBuoyancyForce + accelerationRate * Time.deltaTime, maxBuoyancyForce);
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Surface"))
        {
            isUsingAirBladder = false;
            rb.velocity = Vector3.zero;
            winUI.SetActive(true);
            SwapHandModel(false);
            RemoveAirBladderFromInventory();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Debug.Log("You reached the surface!");
        }
    }

    private bool IsHoldingAirBladder()
    {
        if (itemPickup.inventory.Count > 0 && itemPickup.currentItemIndex >= 0 && itemPickup.currentItemIndex < itemPickup.inventory.Count)
        {
            Item heldItem = itemPickup.inventory[itemPickup.currentItemIndex];
            if (heldItem != null)
            {
                Debug.Log("Currently Holding: " + heldItem.name);
                if (heldItem.name.Contains("AirBladder"))
                {
                    airBladderUI.SetActive(true);
                    return true;
                }
            }
        }
        return false;
    }

    private void RemoveAirBladderFromInventory()
    {
        if (itemPickup.currentItemIndex >= 0 && itemPickup.currentItemIndex < itemPickup.inventory.Count)
        {
            Item usedItem = itemPickup.inventory[itemPickup.currentItemIndex];

            if (usedItem != null && usedItem.name.Contains("Air Bladder"))
            {
                itemPickup.RemoveItem(usedItem);
                Debug.Log("Air Bladder used and removed.");
            }
            else
            {
                Debug.Log("Error: No Air Bladder found in inventory.");
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
    }

    void SwapHandModel(bool useAirBladder)
    {
        defaultHand.SetActive(!useAirBladder);
        airBladderHand.SetActive(useAirBladder);
    }
}

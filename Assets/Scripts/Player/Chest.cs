using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public string requiredKeycardID;
    private Animator anim;
    private bool isOpened = false;
    public ParticleSystem bubbles;
    public AudioSource bubbleSFX;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    public void TryOpen(Item heldItem)
    {
        if (isOpened)
        {
            Debug.Log("Chest is already opened.");
            return;
        }

        if (heldItem != null)
        {
            Debug.Log("Held Item Name: " + heldItem.name); 

            if (heldItem.name == requiredKeycardID)
            {
                Debug.Log("Chest opened with: " + heldItem.name);
                OpenChest();

                ItemPickup itemPickup = FindObjectOfType<ItemPickup>();
                if (itemPickup != null)
                {
                    itemPickup.RemoveItem(heldItem);
                }
            }
            else
            {
                Debug.Log("Incorrect keycard: " + heldItem.name + " does not match " + requiredKeycardID);
            }
        }
        else
        {
            Debug.Log("No keycard held.");
        }
    }

    private void OpenChest()
    {
        Debug.Log("Chest opened!");

        anim.SetBool("isOpen", true);
        bubbles.Play();
        bubbleSFX.Play();
    }
}

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
    private Collider chestCollider;
    public GameObject wrongChestText;
    public float detectionRange = 5f;
    public AudioSource proximitySFX;
    private bool isPlayingAudio = false;
    public Transform player;

    private void Start()
    {
        anim = GetComponent<Animator>();
        chestCollider = GetComponent<Collider>();
        wrongChestText.SetActive(false);
    }

    private void Update()
    {
        if (player == null || isOpened) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange && !isPlayingAudio)
        {
            proximitySFX.Play();
            isPlayingAudio = true;
        }
        else if (distance > detectionRange && isPlayingAudio)
        {
            proximitySFX.Stop();
            isPlayingAudio = false;
        }
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
                StartCoroutine(DisableChestText());
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
        chestCollider.enabled = false;

        if (proximitySFX.isPlaying)
        {
            proximitySFX.Stop();
            isPlayingAudio = false;
        }

        isOpened = true;
    }

    private IEnumerator DisableChestText()
    {
        wrongChestText.SetActive(true);
        yield return new WaitForSeconds(3f);
        wrongChestText.SetActive(false);
    }
}

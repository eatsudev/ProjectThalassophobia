using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    public Transform cameraTransform;
    public float interactRange = 2f;
    public LayerMask interactableLayer;
    public ItemPickup itemPickup;
    public Text interactNameUI;

    void Update()
    {
        ShowInteractNameUI();
        if (Input.GetKeyDown(KeyCode.E)) 
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        RaycastHit hit;
        Vector3 rayOrigin = cameraTransform.position;
        Vector3 rayDirection = cameraTransform.forward;

        Debug.DrawRay(rayOrigin, rayDirection * interactRange, Color.red, 1f);

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, interactRange, interactableLayer))
        {
            Debug.Log("Interacted with: " + hit.collider.gameObject.name);

            Chest chest = hit.collider.GetComponent<Chest>();
            if (chest != null)
            {
                chest.TryOpen(itemPickup.GetHeldItem());
            }

            ClueItem clueItem = hit.collider.GetComponent<ClueItem>();
            if (clueItem != null)
            {
                clueItem.Interact();
            }

            Debug.Log("This is not a chest/clue item");
        }
        else
        {
            Debug.Log("Raycast did not hit anything.");
        }
    }

    void ShowInteractNameUI()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, interactRange, interactableLayer))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                interactNameUI.text = interactable.interactName + " (E)";
                interactNameUI.enabled = true;
                return;
            }
        }
        interactNameUI.enabled = false;
    }
}

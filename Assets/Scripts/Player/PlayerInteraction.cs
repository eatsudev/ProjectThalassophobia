using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public Transform cameraTransform;
    public float interactRange = 2f;
    public LayerMask interactableLayer;
    public ItemPickup itemPickup;

    void Update()
    {
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
            else
            {
                Debug.Log("This is not a chest.");
            }
        }
        else
        {
            Debug.Log("Raycast did not hit anything.");
        }
    }
}

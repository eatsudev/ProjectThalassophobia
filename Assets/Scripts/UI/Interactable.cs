using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string interactName = "Interact";

    public virtual void Interact()
    {
        Debug.Log("Interacted with " + interactName);
    }
}

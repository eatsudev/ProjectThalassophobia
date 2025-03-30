using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public Light flashlight;
    private bool isOn = false;
    public AudioSource flashlightSFX;

    void Start()
    {
        flashlight.enabled = true;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            ToggleFlashlight();
            flashlightSFX.Play();
        }
    }

    private void ToggleFlashlight()
    {
        isOn = !isOn;
        flashlight.enabled = isOn;
    }
    
}

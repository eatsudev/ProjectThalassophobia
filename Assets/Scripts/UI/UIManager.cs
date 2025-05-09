using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Text readableText;
    public PlayerController playerController;
    public Image imageTemplate;
    public GameObject closeButton;

    private void Start()
    {
        imageTemplate.enabled = false;
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowReadableUI(string content, Sprite clueSprite)
    {
        readableText.text = content;
        closeButton.SetActive(true);

        if (clueSprite != null)
        {
            imageTemplate.sprite = clueSprite;
            imageTemplate.enabled = true;
        }
        else
        {
            imageTemplate.enabled = false;
            imageTemplate.sprite = null;
        }

        Cursor.lockState = CursorLockMode.None;
        if (playerController != null)
        {
            playerController.canLook = false;
        }
    }

    public void CloseReadableUI()
    {
        readableText.text = null;
        closeButton.SetActive(false);

        imageTemplate.enabled = false;
        imageTemplate.sprite = null;

        Cursor.lockState = CursorLockMode.Locked;
        if (playerController != null)
        {
            playerController.canLook = true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public GameObject readablePanel;
    public Text readableText;
    public Image imageTemplate;
    public PlayerController playerController;

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
        readablePanel.SetActive(true);
        readableText.text = content;
        //Time.timeScale = 0f;

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
        readablePanel.SetActive(false);
        readableText.text = null;
        //Time.timeScale = 1f;

        imageTemplate.enabled = false;
        imageTemplate.sprite = null;

        Cursor.lockState = CursorLockMode.Locked;

        if (playerController != null)
        {
            playerController.canLook = true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlsMenu : MonoBehaviour
{
    public void PlayScene()
    {
        SceneManager.LoadScene("MainScene");
        Time.timeScale = 1.0f;
    }
}

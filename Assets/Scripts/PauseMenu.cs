using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
public class PauseMenu : MonoBehaviour
{
    public static bool Paused = false;
    public GameObject PauseMenuCanvas;
    public GameObject WinMenu;
    public GameObject LoseMenu;
    private FPSController playerFPSController;
    public GameObject player;

    void Start()
    {
        playerFPSController = player.GetComponent<FPSController>();
        PauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f;
        AudioListener.pause = false; // Make sure audio is not paused initially
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(Paused)
            {
                playerFPSController.canMove = true;
                AudioListener.pause = false;
                Play();
            }
            else
            {
                playerFPSController.canMove = false;
                AudioListener.pause = true; // Pause audio
                Stop();
            }
        }
    }

    void Stop()
    {
        AudioListener.pause = true;
        PauseMenuCanvas.SetActive(true);
        Time.timeScale = 0f;
        Paused = true;
    }

    public void Play()
    {
        AudioListener.pause = false;
        PauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f;
        Paused = false;

        // Force the game window to regain focus
        System.Threading.Thread.Sleep(100); // Small delay to ensure the window is ready
        SetFocus();
    }

    private void SetFocus()
    {
        #if !UNITY_EDITOR
            Type type = Type.GetType("UnityEngine.GUIUtility, UnityEngine, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
            if (type != null)
            {
                type.GetMethod("ForceFocusOnGameView", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).Invoke(null, null);
            }
        #endif
    }

    public void GameOverWin()
    {
        playerFPSController.canMove = false;
        AudioListener.pause = true;
        WinMenu.SetActive(true);
        Time.timeScale = 0f;
        Paused = true;
    }
    public void GameOverLose()
    {
        playerFPSController.canMove = false;
        AudioListener.pause = true;
        LoseMenu.SetActive(true);
        Time.timeScale = 0f;
        Paused = true;
    }
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}

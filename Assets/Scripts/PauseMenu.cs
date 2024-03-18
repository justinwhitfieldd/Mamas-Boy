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
    public AudioClip win_sound;
    public AudioClip lose_sound;
    private AudioSource audioSource;
    void Start()
    {
        playerFPSController = player.GetComponent<FPSController>();
        PauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f;
        AudioListener.pause = false; // Make sure audio is not paused initially
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Paused)
            {
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
        if(!playerFPSController.isInteracting) playerFPSController.canMove = true;
        AudioListener.pause = false;
        PauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f;
        Paused = false;
        // Force the game window to regain focus
        System.Threading.Thread.Sleep(100); // Small delay to ensure the window is ready
    }


    public void GameOverWin()
    {
        playerFPSController.canMove = false;
        WinMenu.SetActive(true);
        audioSource.PlayOneShot(win_sound);
    }

    public void GameOverLose()
    {
        playerFPSController.canMove = false;
        LoseMenu.SetActive(true);
        audioSource.PlayOneShot(lose_sound);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool Paused = false;
    public GameObject PauseMenuCanvas;
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
                AudioListener.pause = false;
                playerFPSController.canMove = true;
                Play();
            }
            else
            {
                AudioListener.pause = true; // Pause audio
                playerFPSController.canMove = false;
                Stop();
            }
        }
    }

    void Stop()
    {
        PauseMenuCanvas.SetActive(true);
        Time.timeScale = 0f;
        Paused = true;
    }

    public void Play()
    {
        PauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f;
        Paused = false;
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}

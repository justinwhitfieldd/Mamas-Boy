using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarFill : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float distanceFromCamera = 0.31f; // Distance in front of the camera where the object spawns and moves
    public bool taskFailed = false;
    [SerializeField] private float leftOffset = 0.17f;

    public Scrollbar scrollbar;
    public float fillSpeed = 0.5f; // Adjust as needed
    private bool isFilling = false;
    private bool filledBar = false;

    // Call this method to start the appearance and disappearance coroutine
    // Call this method to start the appearance and disappearance coroutine
    public void StartAppearAndDisappear()
    {
        // Activate the object
        gameObject.SetActive(true);

        // Start the AppearAndDisappear coroutine
        StartCoroutine(AppearAndDisappear());
    }

    // Coroutine for the appearance and disappearance of the object
    private IEnumerator AppearAndDisappear()
    {

        // Set the initial position of the object in front of the camera
        if (mainCamera != null)
        {
            Vector3 cameraPosition = mainCamera.transform.position;
            Vector3 cameraForward = mainCamera.transform.forward;
            Vector3 spawnPosition = cameraPosition + cameraForward * distanceFromCamera - (mainCamera.transform.right * leftOffset);
            transform.position = spawnPosition;
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
        }

        // Move the object to the right over the specified duration
        while (true)
        {
            Debug.Log("Got here");
            if (Input.GetKey(KeyCode.E))
            {
                isFilling = true;
            }

            else
            {
                isFilling = false;
            }

            if (isFilling)
            {
                FillScrollbar();
            }

            else
            {
                taskFailed = true;
                gameObject.SetActive(false); // Deactivate the object
                yield break;
            }

            if (filledBar == true)
            {
                taskFailed = false;
                yield break;
            }

        }
    }

    // Coroutine to wait for the completion of the AppearAndDisappear coroutine
    public IEnumerator WaitForCompletion()
    {
        taskFailed = false;
        // Wait for the AppearAndDisappear coroutine to finish
        yield return new WaitUntil(() => !gameObject.activeSelf);

    }

    void FillScrollbar()
    {
        if (scrollbar.size < 1f)
        {
            scrollbar.size += fillSpeed * Time.deltaTime;
        }
        else
        {
            filledBar = true;
        }
    }

}
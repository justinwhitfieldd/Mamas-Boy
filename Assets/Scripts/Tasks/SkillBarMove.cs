using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBarMove : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    private float speed = 0.45f; // speed of movement
    [SerializeField] private float distanceFromCamera = 0.31f; // Distance in front of the camera where the object spawns and moves
    [SerializeField] private float leftOffset = 0.17f;
    private float SkillTime = 0f;
    public bool taskFailed = false;
    private bool lateFlag = true;

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
            speed = Random.Range(0.28f, 0.78f);
            SkillTime = 0;
            SkillTime = 0;
            Vector3 cameraPosition = mainCamera.transform.position;
            Vector3 cameraForward = mainCamera.transform.forward;
            Vector3 spawnPosition = cameraPosition + cameraForward * distanceFromCamera - (mainCamera.transform.right * leftOffset);
            transform.position = spawnPosition;
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
        }

        // Calculate the position where the object should disappear
        float disappearPosition = 0.345f;

        // Move the object to the right over the specified duration
        while (true)
        {
            // Check if the left mouse button is clicked
            if (Input.GetMouseButtonDown(0))
            {
                // See if it is a success should be 0.238 - 
                if ((SkillTime > 0.236) && (SkillTime < 0.277))
                {
                    taskFailed = false; // Task successful
                    lateFlag = false;
                }
                else
                {
                    taskFailed = true; // Task failed
                    lateFlag = true;
                }

                Debug.Log("(Window Size: " + (0.236) + ", " + 0.277 + ")");
                Debug.Log("SkillTime: " + SkillTime);

                // Wait for 1 second before returning
                yield return new WaitForSeconds(0.5f);

                gameObject.SetActive(false); // Deactivate the object
                yield break; // Exit the coroutine
            }

            // Move the object to the right
            transform.Translate(Vector3.right * speed * Time.deltaTime);
            SkillTime += speed * Time.deltaTime;

            // Check if the object has reached the disappear position
            if (SkillTime >= disappearPosition)
            {
                if (lateFlag)
                {
                    taskFailed = true;
                }

                // Deactivate the object after the movement is complete
                gameObject.SetActive(false);
                yield break; // Exit the coroutine
            }

            yield return null; // Yield to allow other coroutines to execute
        }
    }

    // Coroutine to wait for the completion of the AppearAndDisappear coroutine
    public IEnumerator WaitForCompletion()
    {
        taskFailed = false;
        lateFlag = true;
        // Wait for the AppearAndDisappear coroutine to finish
        yield return new WaitUntil(() => !gameObject.activeSelf);

    }


}

using System.Collections; using System.Collections.Generic; using UnityEngine;
public class MultiSkillBarMove : MonoBehaviour
{
[SerializeField] private Camera mainCamera;
private float speed = 0.45f; // speed of movement
[SerializeField] private float distanceFromCamera = 0.31f; // Distance in front of the camera where the object spawns and moves
[SerializeField] private float leftOffset = 0.17f;
[SerializeField] private AudioSource loseSound;
[SerializeField] private AudioSource winSound;
private float SkillTime = 0f;
public bool taskFailed = false;
private float specialTracker = 4.73f;
private float specialCount = 0f;

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
        speed = Random.Range(0.1f, 0.2f);
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
            Debug.Log("(Window Size: (" + (0.11) + ", " + 0.14 + ") (" + (0.22) + ", " + 0.25 + ") (" + (0.28) + ", " + 0.31 + ")");
            Debug.Log("SkillTime: " + SkillTime);
            Debug.Log("SpecialCount: " + specialCount);
            Debug.Log("SpecialTracker: " + specialTracker);

            // 1st hit, if you do 1 in each you should get the right value
            if ((SkillTime > 0.11) && (SkillTime < 0.14))
            {
                specialCount += 4f;
            }
            // 2nd hit
            else if ((SkillTime > 0.22) && (SkillTime < 0.25))
            {
                specialCount += 0.7f;
            }
            // 3rd hit
            else if ((SkillTime > 0.28) && (SkillTime < 0.31))
            {
                specialCount += 0.03f;

                // Wait for 1 second before returning
                yield return new WaitForSeconds(0.5f);

                // Check if all items were hit successfully
                if (specialCount == specialTracker)
                {
                    winSound.Play();
                }
                else
                {
                    loseSound.Play();
                    taskFailed = true;
                }

                // Deactivate the object after the movement is complete
                gameObject.SetActive(false);
                yield break; // Exit the coroutine
            }
            else
            {
                loseSound.Play();
                taskFailed = true;
                gameObject.SetActive(false);
                yield break; // Exit the coroutine
            }
        }

        // Move the object to the right
        transform.Translate(Vector3.right * speed * Time.deltaTime);
        SkillTime += speed * Time.deltaTime;

        // Check if the object has reached the disappear position
        if (SkillTime >= disappearPosition)
        {
            taskFailed = true;
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
    specialCount = 0f;
    // Wait for the AppearAndDisappear coroutine to finish
    yield return new WaitUntil(() => !gameObject.activeSelf);
}
}
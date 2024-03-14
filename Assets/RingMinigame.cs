using UnityEngine;
using UnityEngine.UI;

public class RingMinigame : MonoBehaviour
{
    public GameObject ring1;
    public GameObject ring2;
    public GameObject ring3;
    public float rotationSpeed = 100f;
    public float rotationSpeed2 = 200f;
    public float rotationSpeed3 = 250f;

    public float successThreshold = 10f;
    public Image resultImage;
    public Color successColor = Color.green;
    public Color failureColor = Color.red;
    public float colorChangeDuration = 1f;

    private bool ring1Stopped = false;
    private bool ring2Stopped = false;
    private bool ring3Stopped = false;
    private Color originalColor;

    private void Start()
    {
        originalColor = resultImage.color;
        StartGame();
    }

    private void Update()
    {
        // Rotate the rings if they are not stopped
        if (!ring1Stopped)
            ring1.transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        if (!ring2Stopped)
            ring2.transform.Rotate(0f, 0f, rotationSpeed2 * Time.deltaTime);
        if (!ring3Stopped)
            ring3.transform.Rotate(0f, 0f, rotationSpeed3 * Time.deltaTime);

        // Check for user click
        if (Input.GetMouseButtonDown(0))
        {
            if (!ring1Stopped)
            {
                ring1Stopped = true;
            }
            else if (!ring2Stopped)
            {
                ring2Stopped = true;
                if (!CheckNextRingPositions(ring1, ring2))
                {
                    EndGame(false);
                    return;
                }
            }
            else if (!ring3Stopped)
            {
                ring3Stopped = true;
                if (!CheckNextRingPositions(ring2, ring3))
                {
                    EndGame(false);
                    return;
                }
                CheckSuccess();
            }
        }
    }

    private bool CheckNextRingPositions(GameObject prevRing, GameObject nextRing)
    {
        float prevRingRotation = prevRing.transform.rotation.eulerAngles.z;
        float nextRingRotation = nextRing.transform.rotation.eulerAngles.z;

        // Check if the next ring's position is within the success threshold of the previous ring
        if (Mathf.Abs(prevRingRotation - nextRingRotation) > successThreshold)
        {
            return false;
        }
        return true;
    }

    private void CheckSuccess()
    {
        float ring1Rotation = ring1.transform.rotation.eulerAngles.z;
        float ring2Rotation = ring2.transform.rotation.eulerAngles.z;
        float ring3Rotation = ring3.transform.rotation.eulerAngles.z;

        // Check if the rings are aligned within the success threshold
        if (Mathf.Abs(ring1Rotation - ring2Rotation) <= successThreshold &&
            Mathf.Abs(ring2Rotation - ring3Rotation) <= successThreshold &&
            Mathf.Abs(ring3Rotation - ring1Rotation) <= successThreshold)
        {
            EndGame(true);
        }
        else
        {
            EndGame(false);
        }
    }


    private void EndGame(bool success)
    {
        if (success)
        {
            Debug.Log("Success!");
            ChangeImageColor(successColor);
        }
        else
        {
            Debug.Log("Failure!");
            ChangeImageColor(failureColor);
        }

        Invoke("RestartGame", colorChangeDuration);
    }

    private void RestartGame()
    {
        StartGame();
    }

    private void StartGame()
    {
        // Reset the game state
        ring1Stopped = false;
        ring2Stopped = false;
        ring3Stopped = false;

        // Randomize the initial rotation of the rings
        ring1.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        ring2.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        ring3.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

        // Reset the image color
        resultImage.color = originalColor;
    }

    private void ChangeImageColor(Color targetColor)
    {
        resultImage.color = targetColor;
    }
}
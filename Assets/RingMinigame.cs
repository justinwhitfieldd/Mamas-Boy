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
    public GameObject SuccessIcon;

    public float successThreshold = 10f;
    public Image resultImage;
    public Image vector1box;
    public Image vector2box;
    public Image vector3box;
    public Color successColor = Color.green;
    public Color failureColor = Color.red;
    public float colorChangeDuration = 1f;

    private bool ring1Stopped = false;
    private bool ring2Stopped = false;
    private bool ring3Stopped = false;
    private Color originalColor;
    private Color originalVectorColor;
    public bool isActive = false;
    public bool isComplete = false;
    public void ToggleGameActivity()
    {
        isActive = !isActive;
    }
    private void Start()
    {
        originalColor = resultImage.color;
        originalVectorColor = vector1box.color;
        StartGame();
    }

    private void Update()
    {

        if (isComplete)
        {
            ring1.transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
            ring2.transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
            ring3.transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        } else {
            if (!isActive)
                return;
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
                    ChangeImageColor(vector1box,successColor);

                }
                else if (!ring2Stopped)
                {
                    ring2Stopped = true;
                    if (!CheckNextRingPositions(ring1, ring2))
                    {
                                    
                        ChangeImageColor(vector2box,failureColor);
                        ChangeImageColor(vector3box,failureColor);
                        EndGame(false);
                        return;
                    } else {
                        ChangeImageColor(vector2box,successColor);
                    }
                }
                else if (!ring3Stopped)
                {
                    ring3Stopped = true;
                    if (!CheckNextRingPositions(ring2, ring3))
                    {
                        ChangeImageColor(vector3box,failureColor);
                        EndGame(false);
                        return;
                    }
                    else {
                        ChangeImageColor(vector3box,successColor);
                    }
                    CheckSuccess();
                }
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
            Mathf.Abs(ring2Rotation - ring3Rotation) <= successThreshold )//&&
            //Mathf.Abs(ring3Rotation - ring1Rotation) <= successThreshold)
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
            SuccessIcon.SetActive(true);
            isActive = false;
            Debug.Log("Success!");


            //ChangeImageColor(resultImage,successColor);
            isComplete = true;
        }
        else
        {
            Debug.Log("Failure!");

            Invoke("RestartGame", colorChangeDuration);
        }
    }

    private void RestartGame()
    {
        if (!isComplete && isActive)
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
        vector1box.color = originalVectorColor;
        vector2box.color = originalVectorColor;
        vector3box.color = originalVectorColor;
    }


    private void ChangeImageColor(Image image,Color targetColor)
    {
        image.color = targetColor;
    }
}
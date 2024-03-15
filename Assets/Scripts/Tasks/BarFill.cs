using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarFill : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    public bool taskFailed = false;

    public Scrollbar scrollbar;
    public float fillTime = 6f; // Adjust as needed
    private bool isFilling = false;
    private bool filledBar = false;
    private float elapsedTime = 0f;

    // Call this method to start the appearance and disappearance coroutine
    // Call this method to start the appearance and disappearance coroutine

    public void StartAppearAndDisappear()
    {
        elapsedTime = 0f; // Reset elapsed time
        Debug.Log("Set up");
        return;
    }

    private void Update()
    {
        isFilling = Input.GetKey(KeyCode.E);

        // Fill the scrollbar if isFilling is true and the scrollbar is not filled
        if (isFilling && !filledBar)
        {
            Debug.Log("we fill");
            FillScrollbar();
        }

        if (filledBar)
        {
            Debug.Log("bingo");
            taskFailed = false;
            gameObject.SetActive(false); // Deactivate the object
        }

        if (!isFilling)
        {
            Debug.Log("not here");
            taskFailed = true;
            gameObject.SetActive(false); // Deactivate the object
        }
    }

    // Coroutine to wait for the completion of the AppearAndDisappear coroutine
    public IEnumerator WaitForCompletion()
    {
        taskFailed = false;
        // Wait for the AppearAndDisappear coroutine to finish
        yield return new WaitUntil(() => !gameObject.activeSelf);

    }

    private void FillScrollbar()
    {
        // Increment elapsed time
        elapsedTime += Time.deltaTime;

        // Calculate the fill amount based on the elapsed time
        float fillAmount = Mathf.Clamp01(elapsedTime / fillTime); // Fill over 20 seconds

        // Update the scrollbar size
        scrollbar.size = fillAmount;
        Debug.Log("fill amount " + fillAmount.ToString());

        // Check if the scrollbar is filled
        if (fillAmount >= 1f)
        {
            filledBar = true;
        }
    }

}
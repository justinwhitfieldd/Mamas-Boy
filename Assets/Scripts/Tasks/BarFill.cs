using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarFill : MonoBehaviour
{
    public GameObject scrollBarr;
    public GameObject success_text;
    public GameObject this_button;
    public bool taskFailed = false;
    public Scrollbar scrollbar;
    [SerializeField] private AudioSource winSound;
    public float fillTime = 10f; // Adjust as needed
    public bool isFilling = false;
    private bool filledBar = false;
    private float elapsedTime = 0f;
    public TaskCounter taskCounter;

    // Call this method to start the appearance and disappearance coroutine
    public void StartAppearAndDisappear()
    {
        elapsedTime = 0f; // Reset elapsed time
        Debug.Log("Set up");
        return;
    }

    private void Update()
    {
        // Check if the button is being clicked
        if (Input.GetMouseButton(0))
        {
            isFilling = true;
        }
        else
        {
            isFilling = false;
        }

        // Fill the scrollbar if isFilling is true and the scrollbar is not filled
        if (isFilling && !filledBar)
        {
            FillScrollbar();
        }

        if (filledBar)
        {
            winSound.Play();
            taskCounter.IncrementCounter("farm");
            taskFailed = false;
            this_button.SetActive(false);
            success_text.SetActive(true);
            scrollBarr.SetActive(false); // Deactivate the object
        }

        if (!isFilling)
        {
            elapsedTime = 0f;
            scrollbar.size = 0;
            taskFailed = true;
            //gameObject.SetActive(false); // Deactivate the object
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

        // Check if the scrollbar is filled
        if (fillAmount >= 1f)
        {
            filledBar = true;
        }
    }
}
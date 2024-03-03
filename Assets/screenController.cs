using UnityEngine;
using UnityEngine.UIElements;

public class ProgressBarColorController : MonoBehaviour
{
    private UIDocument uiDocument;
    private ProgressBar myProgressBar;

    void Start()
    {
        // Get the UIDocument component
        uiDocument = GetComponent<UIDocument>();
        
        // Get the root visual element of the UI document
        var rootVisualElement = uiDocument.rootVisualElement;
        
        // Find the progress bar by its name
        myProgressBar = rootVisualElement.Q<ProgressBar>("ProgressBar");

        // Example: Initialize the progress bar with a specific value
        UpdateProgressBar(10); // Starting with a value of 10
    }

    public void UpdateProgressBar(float value)
    {
        // Update the progress bar's value
        myProgressBar.value = value;

        // Determine the color based on the value (0 = red, 100 = green)
        Color newColor = Color.Lerp(Color.red, Color.green, value / 100.0f);

        // Apply the color to the progress bar's fill element
        var progressBarFill = myProgressBar.Q<VisualElement>(className: "unity-progress-bar__fill");
        if (progressBarFill != null)
        {
            progressBarFill.style.backgroundColor = newColor;
        }
    }
}

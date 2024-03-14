using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TaskIndicatorManager : MonoBehaviour
{
    public Dictionary<string, bool> taskIndicators = new Dictionary<string, bool>();

    // Declare public variables for each task's image
    public Image pressureRegulatorLight;
    public Image guidance_light;
    public GameObject status_bar_guidance;
    public GameObject status_bar_pressure;
    // Add more public variables for each task's image

    // Declare a public variable for the new game object
    public GameObject newObjectPrefab;

    // Variables to store the button and image components of the new object
    private Button newObjectButton;
    private Image newObjectImage;

    private void Start()
    {
        // Initialize the taskIndicators dictionary with task names and false values
        taskIndicators.Add("pressure_regulator_light", false);
        taskIndicators.Add("guidance", false);
        // Add more tasks as needed

        // Fetch the button and image components from the new object prefab
        newObjectButton = newObjectPrefab.GetComponentInChildren<Button>();
        newObjectImage = newObjectPrefab.GetComponentInChildren<Image>();

        // Set the button as not interactable initially
        newObjectButton.interactable = false;
    }

    public void SetIndicatorLight(string taskName, bool completed)
    {
        if (taskIndicators.ContainsKey(taskName))
        {
            taskIndicators[taskName] = completed;
            UpdateIndicatorColor(taskName, completed);
        }
        else
        {
            Debug.LogWarning($"Task '{taskName}' not found in the taskIndicators dictionary.");
        }

        CheckAndTriggerEffect();
    }

    private void UpdateIndicatorColor(string taskName, bool completed)
    {
        // Get the corresponding image for the task name
        Image indicatorImage = GetIndicatorImage(taskName);

        if (indicatorImage != null)
        {
            indicatorImage.color = completed ? Color.green : Color.red;
        }
        else
        {
            Debug.LogWarning($"Light indicator for task '{taskName}' not found.");
        }
    }

    private Image GetIndicatorImage(string taskName)
    {
        switch (taskName)
        {
            case "pressure_regulator_light":
                status_bar_pressure.SetActive(true);
                return pressureRegulatorLight;
            case "guidance":
                status_bar_guidance.SetActive(true);
                return guidance_light;
            // Add more cases for each task
            default:
                return null;
        }
    }

    private void CheckAndTriggerEffect()
    {
        // Check if all task indicators are true
        bool allTasksCompleted = true;
        foreach (bool completed in taskIndicators.Values)
        {
            if (!completed)
            {
                allTasksCompleted = false;
                break;
            }
        }

        if (allTasksCompleted)
        {
            // Trigger the desired effect (e.g., play a particle effect, show a message, etc.)
            Debug.Log("All tasks completed! Triggering effect...");

            // Update the color of the new object's image
            newObjectImage.color = new Color(130f / 255f, 140f / 255f, 255f / 255f);

            // Set the button as interactable
            newObjectButton.interactable = true;
        }
    }
}
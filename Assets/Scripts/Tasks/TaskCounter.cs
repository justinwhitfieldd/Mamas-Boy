using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskCounter : MonoBehaviour
{
    private int taskCounter = 0;
    [SerializeField] public int targetValue = 10; // Change this to the desired target value
    private List<string> completed_tasks = new List<string>();

    public TaskIndicatorManager indicatorManager;

    private void Start()
    {
        indicatorManager = FindObjectOfType<TaskIndicatorManager>();
        if (indicatorManager == null)
        {
            Debug.LogWarning("TaskIndicatorManager script not found in the scene.");
        }
    }

    // Call this method to increment the task counter
    public void IncrementCounter(string taskName)
    {
        taskCounter += 1;
        completed_tasks.Add(taskName); // Add the task name to the list
        Debug.Log("Task done added: " + taskName);

        // Update the task indicator light
        if (indicatorManager != null)
        {
            indicatorManager.SetIndicatorLight(taskName, true);
        }

        // Check if the counter has reached the target value
        if (taskCounter >= targetValue)
        {
            // Perform the action once the target value is reached
            TasksFinished();
        }
    }

    private void TasksFinished()
    {
        Debug.Log("Counter reached target value! Finishing Game...");
        Debug.Log("Completed Tasks: " + string.Join(", ", completed_tasks)); // Print out the entire list of completed tasks
    }
}
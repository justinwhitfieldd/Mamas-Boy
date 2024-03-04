using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskCounter : MonoBehaviour
{
    private int taskCounter = 0;
    [SerializeField] public int targetValue = 10; // Change this to the desired target value

    // Call this method to increment the task counter
    public void IncrementCounter()
    {
        taskCounter += 1; // Increment the counter
        Debug.Log("Task done added");

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
    }




}

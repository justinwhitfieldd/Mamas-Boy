using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AccessiblePoints : MonoBehaviour
{
    public GameObject[] accessiblePoints;
    public int ranking = 0;

    private List<GameObject> shortestPath = null;

    public GameObject GetRandomAccessiblePoint(int newRanking)
    {
        ranking = newRanking;
        float lowestRanking = Mathf.Infinity;
        GameObject lowestPoint = null;
        GameObject[] shufflePoints = Shuffle(accessiblePoints);
        foreach (GameObject accessiblePoint in shufflePoints)
        {
            int pointRanking = accessiblePoint.GetComponent<AccessiblePoints>().ranking;
            if (pointRanking < lowestRanking)
            {
                lowestPoint = accessiblePoint;
                lowestRanking = pointRanking;
            }
        }
        return lowestPoint;
    }

    private List<GameObject> RecurseOverAllPoints(GameObject hostWanderPoint, GameObject destinationPoint, List<GameObject> currentPath)
    {
        if (hostWanderPoint == destinationPoint)
        {
            return currentPath; // Reached destination, return the current path
        }

        GameObject[] hostAccessiblePoints = hostWanderPoint.GetComponent<AccessiblePoints>().accessiblePoints;
        List<GameObject> shortest = null;

        foreach (GameObject wanderPoint in hostAccessiblePoints)
        {
            if (!currentPath.Contains(wanderPoint)) // Avoid revisiting the same point
            {
                List<GameObject> newPath = new List<GameObject>(currentPath);
                newPath.Add(wanderPoint);
                List<GameObject> candidatePath = RecurseOverAllPoints(wanderPoint, destinationPoint, newPath);

                if (candidatePath != null && (shortest == null || candidatePath.Count < shortest.Count))
                {
                    shortest = candidatePath;
                }
            }
        }
        return shortest;
    }


    public GameObject GetPointClosestTo(GameObject destinationPoint)
    {
        shortestPath = RecurseOverAllPoints(gameObject, destinationPoint, new List<GameObject> { gameObject });

        if (shortestPath != null && shortestPath.Count > 1)
        {
            return shortestPath[1]; // The next point in the shortest path after the starting point
        }
        else
        {
            return destinationPoint;
        }
    }

    public static T[] Shuffle<T>(T[] array)
    {
        System.Random random = new System.Random();

        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = random.Next(0, i + 1);

            T temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }

        return array;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccessiblePoints : MonoBehaviour
{
    public GameObject[] accessiblePoints;
    public int ranking = 0;

    public GameObject getRandomAccessiblePoint(int newRanking)
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

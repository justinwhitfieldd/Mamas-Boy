using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccessiblePoints : MonoBehaviour
{
    public GameObject[] accessiblePoints;

    public GameObject getRandomAccessiblePoint()
    {
        return accessiblePoints[Random.Range(0, accessiblePoints.Length)];
    }
}

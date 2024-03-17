using UnityEngine;
using System.Collections;

public class StatusBar : MonoBehaviour {

    public GameObject statusObject1;
    public GameObject statusObject2;
    public GameObject statusObject3;
    public GameObject statusObject4;
    public GameObject statusObject5;
    public GameObject statusObject6;
    public GameObject statusObject7;
    public GameObject statusObject8;
    public GameObject statusObject9;
    public GameObject statusObject10;
    public GameObject statusObject11;
    public GameObject statusObject12;
    public GameObject statusObject13;
    public float change_delay = 0.5f;
    public float start_delay = 0.1f;
    private GameObject[] statusObjects;
    private int currentIndex = 0;

    private void Start() {
        statusObjects = new GameObject[] {
            statusObject1, statusObject2, statusObject3,
            statusObject4, statusObject5, statusObject6,
            statusObject7, statusObject8, statusObject9
        };
    }

    public void IncreaseStatusBar() {
        StartCoroutine(ToggleObjects());
    }

    private IEnumerator ToggleObjects() {
        yield return new WaitForSeconds(start_delay);
        while (currentIndex < statusObjects.Length) {
            statusObjects[currentIndex].SetActive(true);
            currentIndex++;
            yield return new WaitForSeconds(change_delay);
        }
    }
}
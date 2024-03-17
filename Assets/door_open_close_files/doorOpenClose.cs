using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenClose : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator animator;
    public bool isDoorOpen = false;
    public float autoToggieDistane = 4.0f;
    public Transform alien;
    public Transform robot;
    public LayerMask openLayer;
    public LayerMask closedLayer;
    public GameObject doorTopA;
    public GameObject doorTopB;
    public GameObject doorBottomA;
    public GameObject doorBottomB;
    public AudioClip doorSound;
    private AudioSource doorNoise;

    private void Start()
    {
        animator.SetBool("character_nearby", isDoorOpen);
        doorNoise = GetComponent<AudioSource>();
        doorNoise.clip = doorSound;
    }

    // Update is called once per frame
    private void Update()
    {
        float distanceToAlien = Vector3.Distance(transform.position, alien.position);
        float distanceToRobot = Vector3.Distance(transform.position, robot.position);

        if ((distanceToAlien < autoToggieDistane) && (!isDoorOpen)) toggie();
        if ((distanceToRobot < autoToggieDistane) && (!isDoorOpen)) toggie();
    }

    public void toggie()
    {
        doorNoise.PlayOneShot(doorNoise.clip);
        isDoorOpen = !isDoorOpen;
        animator.SetBool("character_nearby",isDoorOpen);
        if (isDoorOpen)
        {
            Debug.Log(message: "Opening " + name);
            gameObject.layer = GetLayerNumberFromMask(openLayer);
            doorTopA.layer = GetLayerNumberFromMask(openLayer);
            doorTopB.layer = GetLayerNumberFromMask(openLayer);
            doorBottomA.layer = GetLayerNumberFromMask(openLayer);
            doorBottomB.layer = GetLayerNumberFromMask(openLayer);
        }
        else
        {
            Debug.Log(message: "Closing " + name);
            gameObject.layer = GetLayerNumberFromMask(closedLayer);
            doorTopA.layer = GetLayerNumberFromMask(closedLayer);
            doorTopB.layer = GetLayerNumberFromMask(closedLayer);
            doorBottomA.layer = GetLayerNumberFromMask(closedLayer);
            doorBottomB.layer = GetLayerNumberFromMask(closedLayer);
        }
    }

    private int GetLayerNumberFromMask(LayerMask layerMask)
    {
        int layerIndex = -1;

        for (int i = 0; i < 32; i++)
        {
            if ((layerMask.value & (1 << i)) != 0)
            {
                layerIndex = i;
                break;
            }
        }

        return layerIndex;
    }
}

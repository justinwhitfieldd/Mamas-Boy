using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenClose : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator animator;
    public bool isDoorOpen = false;
    public Transform alien;

    // Update is called once per frame
    private void Update()
    {
        float distanceToAlien = Vector3.Distance(transform.position, alien.position);

        if ((distanceToAlien < 4.0f) && (!isDoorOpen)) toggie();
    }


    public void toggie()
    {
        isDoorOpen = !isDoorOpen;
        animator.SetBool("character_nearby",isDoorOpen);
        Debug.Log(message:"Opening door");
    }
}

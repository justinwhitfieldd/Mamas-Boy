using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorOpenClose : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator animator;
    public bool isDoorOpen = false;
    // Update is called once per frame
    public void toggie()
    {
        isDoorOpen = !isDoorOpen;
        animator.SetBool("character_nearby",isDoorOpen);
        Debug.Log(message:"Opening door");
    }
}

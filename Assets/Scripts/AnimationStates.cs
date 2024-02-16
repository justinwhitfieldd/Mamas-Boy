using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Animator animator;
    public float moveSpeed = 5f; // Adjusted to a higher value for better visibility of movement

    private void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Reset all movement bools
        ResetMovementBools();

        // Check for forward and backward movement
        if (moveZ > 0) 
        {
            animator.SetBool("IsMovingForward", true);
        }
        else if (moveZ < 0)
        {
            animator.SetBool("IsMovingBackward", true);
        }

        // Check for left and right movement
        if (moveX > 0) 
        {
            animator.SetBool("IsMovingRight", true);
        }
        else if (moveX < 0)
        {
            animator.SetBool("IsMovingLeft", true);
        }

        // Calculate movement direction relative to the character's orientation
        Vector3 moveDirection = transform.forward * moveZ + transform.right * moveX;
        moveDirection.Normalize(); // Normalize to ensure consistent movement speed in all directions

        // Apply the movement to the character
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    private void ResetMovementBools()
    {
        animator.SetBool("IsMovingForward", false);
        animator.SetBool("IsMovingBackward", false);
        animator.SetBool("IsMovingLeft", false);
        animator.SetBool("IsMovingRight", false);
    }
}

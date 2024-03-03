using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    public Animator animator;
    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 10f;
 
 
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
 
 
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;
 
    public bool canMove = true;
 
    public void EnableFPSControl(bool enable)
    {
        canMove = enable;
        playerCamera.gameObject.SetActive(enable);
    }
    public void UpdateCameraReference(Camera newCamera)
    {
        playerCamera = newCamera;
        // If you have any additional setup for the new camera, do it here
    }
    CharacterController characterController;
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }
 
    void Update()
    {
        if(canMove)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        } else {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        #region Handles Movment
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
 
        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (curSpeedX > 0)
        {
            animator.SetBool("IsMovingBackward", false);
            animator.SetBool("IsMovingForward", true);
        }
        else if (curSpeedX < 0)
        {
            animator.SetBool("IsMovingForward", false);
            animator.SetBool("IsMovingBackward", true);
        }
        else
        {
            animator.SetBool("IsMovingBackward", false);
            animator.SetBool("IsMovingForward", false);
        }
        if (curSpeedY > 0)
        {
            animator.SetBool("IsMovingLeft", false);
            animator.SetBool("IsMovingRight", true);
        }
        else if (curSpeedY < 0)
        {
            animator.SetBool("IsMovingRight", false);
            animator.SetBool("IsMovingLeft", true);
        }
        else
        {
            animator.SetBool("IsMovingLeft", false);
            animator.SetBool("IsMovingRight", false);
        }
        #endregion
 
        #region Handles Jumping
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }
 
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
 
        #endregion
 
        #region Handles Rotation
        characterController.Move(moveDirection * Time.deltaTime);
 
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
 
        #endregion
    }
}
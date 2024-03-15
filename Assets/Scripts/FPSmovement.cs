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
    public CharacterController characterController;

    // View bobbing variables
    public float walkingBobbingSpeed = 14f;
    public float bobbingAmount = 0.05f;
    public float strafingBobbingAmount = 0.03f;
    private float defaultYPos = 0;
    private float defaultXPos = 0;
    private float timer = 0;

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

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        defaultYPos = playerCamera.transform.localPosition.y;
        defaultXPos = playerCamera.transform.localPosition.x;
    }

    void Update()
    {
        if (canMove)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }

        #region Handles Movement
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Press Left Shift to run
        bool isRunning = Input.GetButton("Run");
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

        #region Handles View Bobbing
        if (characterController.isGrounded && canMove)
        {
            // Check if the player is moving
            if (Mathf.Abs(curSpeedX) > 0.1f || Mathf.Abs(curSpeedY) > 0.1f)
            {
                timer += Time.deltaTime * walkingBobbingSpeed;
                float yOffset = defaultYPos + Mathf.Abs(Mathf.Sin(timer)) * bobbingAmount;
                float xOffset = defaultXPos + Mathf.Sin(timer) * strafingBobbingAmount;
                playerCamera.transform.localPosition = new Vector3(
                    xOffset,
                    yOffset,
                    playerCamera.transform.localPosition.z);
            }
            else
            {
                timer = 0;
                playerCamera.transform.localPosition = new Vector3(
                    Mathf.Lerp(playerCamera.transform.localPosition.x, defaultXPos, Time.deltaTime * walkingBobbingSpeed),
                    Mathf.Lerp(playerCamera.transform.localPosition.y, defaultYPos, Time.deltaTime * walkingBobbingSpeed),
                    playerCamera.transform.localPosition.z);
            }
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusCamOnScreen : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;
    [SerializeField] private Camera playerCamera; // Reference to the player's current camera
    [SerializeField] private Camera focusCamera; // Reference to the new camera you want to switch to
    [SerializeField] private FPSController fpsController; // Manually assign this in the Inspector

    public string InteractionPrompt => _prompt;
    public bool isFocus = false;

    public bool Interact(Interactor interactor)
    {
        isFocus = !isFocus;
        Debug.Log($"Interaction occurred. isFocus: {isFocus}");

        if(isFocus)
        {
            // Switch to the focus camera
            fpsController.EnableFPSControl(false);
            Debug.Log("Switching to focus camera.");
            playerCamera.gameObject.SetActive(false);
            focusCamera.gameObject.SetActive(true);
            fpsController.UpdateCameraReference(focusCamera); // Update the camera reference in FPSController
        }
        else
        {
            // Switch back to the player's camera
            fpsController.EnableFPSControl(true);
            Debug.Log("Switching to player camera.");
            focusCamera.gameObject.SetActive(false);
            playerCamera.gameObject.SetActive(true);
            fpsController.UpdateCameraReference(playerCamera); // Update the camera reference back to the player camera
        }

        return true;
    }
}

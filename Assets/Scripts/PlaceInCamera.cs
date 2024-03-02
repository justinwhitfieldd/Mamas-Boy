using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceInCamera : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private bool followCamera = true;
    [SerializeField] public bool turnOff = false;
    private Renderer rendererComponent;
    public float distanceFromCamera = 1f;

    void Start()
    {
        rendererComponent = GetComponent<Renderer>();
    }

    void Update()
    {
        if (!followCamera || mainCamera == null)
            return;

        Vector3 cameraPosition = mainCamera.transform.position;
        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 positionInFrontOfCamera = cameraPosition + cameraForward * distanceFromCamera;

        transform.position = positionInFrontOfCamera;

        // Make the object's transform look at the camera's position, but only on the Y-axis
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);

        if (turnOff == true)
        {
            ToggleVisibility();
            turnOff = false;
        }
    }

    public void ToggleFollowingCamera()
    {
        followCamera = !followCamera;
    }

    public void ToggleVisibility()
    {
        if (rendererComponent != null)
            rendererComponent.enabled = !rendererComponent.enabled;
    }
}



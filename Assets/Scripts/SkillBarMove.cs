using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBarMove : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private bool followCamera = true;
    private Renderer rendererComponent;
    public float distanceFromCamera = 1f;
    [SerializeField] private float speed = 5f; // Adjust the speed of movement
    [SerializeField] private float appearDuration = 2f; // Duration for which the object appears

    private Vector3 targetPosition;

    private void Start()
    {
        rendererComponent = GetComponent<Renderer>();
        mainCamera = Camera.main;
        targetPosition = mainCamera.transform.position + mainCamera.transform.forward * distanceFromCamera;
        StartCoroutine(AppearAndDisappear());
    }

    private System.Collections.IEnumerator AppearAndDisappear()
    {
        yield return new WaitForSeconds(appearDuration);
        gameObject.SetActive(false); // Or Destroy(gameObject);
    }

    private void Update()
    {
        if (!followCamera || mainCamera == null)
            return;

        // Move the object to the right
        transform.Translate(Vector3.right * speed * Time.deltaTime);
        Debug.Log("Moving right");
    }

    private void LateUpdate()
    {
        if (!followCamera || mainCamera == null)
            return;

        Vector3 cameraPosition = mainCamera.transform.position;
        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 positionInFrontOfCamera = cameraPosition + cameraForward * distanceFromCamera;

        // Separate x, y, and z components
        float x = positionInFrontOfCamera.x + speed * Time.deltaTime;
        float y = positionInFrontOfCamera.y;
        float z = positionInFrontOfCamera.z;

        // Set the object's position using separated components
        transform.position = new Vector3(x, y, z);

        // Make the object's transform look at the camera's position, but only on the Y-axis
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
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

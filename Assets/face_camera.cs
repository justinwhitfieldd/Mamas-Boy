using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    public Camera mainCamera;

       private void LateUpdate()
    {
        // Make the UI element face the camera without warping
        Vector3 directionToCamera = transform.position - mainCamera.transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);
        
        // Match the x and y rotation of the camera
        float xRotation = mainCamera.transform.rotation.eulerAngles.x;
        float yRotation = targetRotation.eulerAngles.y;
        
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}
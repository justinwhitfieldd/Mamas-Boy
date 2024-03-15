using UnityEngine;

public class ParentRotator : MonoBehaviour
{
    public float rotationSpeed = 45.0f; // Degrees per second

    void Update()
    {
        transform.Rotate(Vector3.up* rotationSpeed * Time.deltaTime);
    }
}

using Unity.VisualScripting;
using UnityEngine;

public class WanderingSystem : MonoBehaviour
{
    public GameObject startingPoint;
    public float speed = 1.0f;
    public float rotationSpeed = 5.0f;
    public float waitTimer = 5.0f;
    public float gravity = 10f;

    private CharacterController characterController;
    private Animator animator;
    private Vector3 previousPosition;
    private GameObject currentPoint;
    private Transform currentTransform;
    private float timer = 0;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        previousPosition = transform.position;
        currentPoint = startingPoint;
        currentTransform = currentPoint.transform;
    }

    void Update()
    {
        bool atTransform = moveToTransform(currentTransform);

        if (atTransform)
        {
            Debug.Log("Arrived. Waiting before getting new point.");
            if (timer < waitTimer)
                timer += Time.deltaTime;
            else
            {
                currentTransform = getNewWanderPoint();
                timer = 0;
            }
        }

        previousPosition = transform.position; // Update previous position for the next frame
    }

    bool moveToTransform(Transform transfromToMove)
    {
        Vector3 directionToTransform = (transfromToMove.position - transform.position).normalized;
        Vector2 flatPosition = new Vector2(transform.position.x, transform.position.z);
        Vector2 flatTransformToMove = new Vector2(transfromToMove.position.x, transfromToMove.position.z);
        float flatDistanceToTransform = Vector2.Distance(flatTransformToMove, flatPosition);

        if (flatDistanceToTransform > 0.1f)
        {
            Debug.Log(flatDistanceToTransform);
            Vector3 velocity = directionToTransform * speed;
            velocity.y = -gravity * Time.deltaTime;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToTransform.x, 0, directionToTransform.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
            characterController.Move(velocity * Time.deltaTime);
            animator.SetBool("Walking", (directionToTransform.magnitude > 0));
            return false;
        }

        animator.SetBool("Walking", false);

        return true;
    }

    Transform getNewWanderPoint()
    {
        Debug.Log("Getting new point.");
        currentPoint = currentPoint.GetComponent<AccessiblePoints>().getRandomAccessiblePoint();
        return currentPoint.transform;
    }
}

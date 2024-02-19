using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Transform player;
    public float speed = 5.0f;
    public float rotationSpeed = 5.0f;

    private CharacterController characterController;
    private Animator animator;
    private Vector3 previousPosition;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        previousPosition = transform.position;
    }

    void Update()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance > 0.1f)
        {
            Vector3 velocity = directionToPlayer * speed;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
            characterController.Move(velocity * Time.deltaTime);
        }

        // Check if the enemy has moved since last frame
    bool isWalking = distance > 0.1f && directionToPlayer.magnitude > 0;
    animator.SetBool("Walking", isWalking);
        previousPosition = transform.position; // Update previous position for the next frame
    }
}

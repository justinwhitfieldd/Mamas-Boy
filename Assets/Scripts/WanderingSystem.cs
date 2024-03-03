using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class WanderingSystem : MonoBehaviour
{
    public GameObject player;
    public float jumpScareRadius = 3.0f;
    public AudioClip jumpScareSound;
    public GameObject ambienceSystem;
    public GameObject startingPoint;
    public float speed = 1.0f;
    public float rotationSpeed = 5.0f;
    public float waitTimer = 5.0f;
    public float gravity = 10f;
    public AudioClip[] alienNoises;
    public AudioClip alienStepL;
    public AudioClip alienStepR;
    public float stepCadence = 1.0f;

    private CharacterController characterController;
    private Animator animator;
    private Vector3 previousPosition;
    private GameObject currentPoint;
    private Transform currentTransform;
    private float timer = 0;
    private AudioSource alienNoise;
    private float stepTimer = 0;
    private bool leftFoot = true;
    private bool jumpScared = false;
    private float jumpScareRotationSpeed = 25.0f;

    void Start()
    {
        alienNoise = GetComponent<AudioSource>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        previousPosition = transform.position;
        currentPoint = startingPoint;
        currentTransform = currentPoint.transform;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer < jumpScareRadius)
        {
            ambienceSystem.SetActive(false);
            animator.SetBool("Walking", false);

            FPSController playerFPSController = player.GetComponent<FPSController>();
            playerFPSController.canMove = false;

            Quaternion playerRotation = Quaternion.LookRotation(transform.position - player.transform.position);
            player.transform.rotation = Quaternion.Lerp(player.transform.rotation, playerRotation, Time.deltaTime * jumpScareRotationSpeed);

            Quaternion alienRotation = Quaternion.LookRotation(player.transform.position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, alienRotation, Time.deltaTime * jumpScareRotationSpeed);

            if (!jumpScared)
            {
                alienNoise.clip = jumpScareSound;
                alienNoise.PlayOneShot(alienNoise.clip);
                jumpScared = true;
            }

            return;
        }

        bool atTransform = moveToTransform(currentTransform);

        if (atTransform)
        {
            if (timer < waitTimer)
                timer += Time.deltaTime;
            else
            {
                makeNoise();
                currentTransform = getNewWanderPoint();
                timer = 0;
            }
        }
        else
        {
            makeFootStep();
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
            if (stepTimer > stepCadence) Debug.Log(flatDistanceToTransform);
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

    void makeNoise()
    {
        Debug.Log("Making noise.");
        alienNoise.clip = alienNoises[Random.Range(0, alienNoises.Length)];
        alienNoise.PlayOneShot(alienNoise.clip);
    }

    void makeFootStep()
    {
        if (stepTimer < stepCadence)
            stepTimer += Time.deltaTime;
        else
        {
            Debug.Log("Making step sound.");
            if (leftFoot)
            {
                alienNoise.clip = alienStepL;
                leftFoot = false;
            }
            else
            {
                alienNoise.clip = alienStepR;
                leftFoot = true;
            }
            alienNoise.PlayOneShot(alienNoise.clip);
            stepTimer = 0;
        }
    }
}

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class WanderingSystem : MonoBehaviour
{
    public GameObject player;
    public float jumpScareRadius = 1.0f;
    public float alienFOV = 25f;
    public float triggerRadius = 2.0f;
    public LayerMask obstacleLayer;
    public LayerMask alienLayer;
    public LayerMask wanderPointLayer;
    public AudioClip jumpScareSound;
    public GameObject ambienceSystem;
    public GameObject startingPoint;
    public float rotationSpeed = 5.0f;
    public float waitTimer = 1.0f;
    public float gravity = 10f;
    public AudioClip[] alienNoises;
    public AudioClip alienStepL;
    public AudioClip alienStepR;
    public float stepCadence = 1.0f;
    public float runCadence = 0.5f;

    private CharacterController characterController;
    private Animator animator;
    private GameObject currentPoint;
    private Transform currentTransform;
    private float timer = 0;
    private AudioSource alienNoise;
    private float stepTimer = 0;
    private bool leftFoot = true;
    private bool jumpScared = false;
    private float jumpScareRotationSpeed = 25.0f;
    private FPSController playerFPSController;
    private bool playerSeen = false;
    private bool screwGravity = false;

    void Start()
    {
        alienNoise = GetComponent<AudioSource>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerFPSController = player.GetComponent<FPSController>();
        currentPoint = startingPoint;
        currentTransform = currentPoint.transform;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        Vector3 directionToPlayer = player.transform.position - transform.position;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        RaycastHit hit;
        bool visible = !Physics.Raycast(transform.position, directionToPlayer.normalized, out hit, distanceToPlayer, obstacleLayer);

        #region Handles Jump Scaring Player
        if (visible && (distanceToPlayer < jumpScareRadius))
        {
            ambienceSystem.SetActive(false);
            animator.SetBool("Walking", false);

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
        #endregion

        #region Handles Charging to Player

        if (visible && ((angleToPlayer < alienFOV) || (distanceToPlayer < triggerRadius)))
        {
            bool atPlayer = MoveToTransform(player.transform, (2 * playerFPSController.runSpeed) / 3);
            if (!atPlayer) MakeFootStep(runCadence);

            if (!playerSeen) MakeNoise();
            playerSeen = true;
            
            return;
        }
        else
        {
            if (playerSeen)
            {
                currentTransform = GetClosestWanderPoint();
                MakeNoise();
            }
            playerSeen = false;
        }
        #endregion

        #region Handles Wandering to Points
        bool atTransform = MoveToTransform(currentTransform, playerFPSController.walkSpeed);

        if (atTransform)
        {
            if (timer < waitTimer)
                timer += Time.deltaTime;
            else
            {
                SetCollision(true);
                MakeNoise();
                currentTransform = GetNewWanderPoint();
                timer = 0;
            }
        }
        else
        {
            MakeFootStep(stepCadence);
        }
        #endregion
    }

    bool MoveToTransform(Transform transfromToMove, float speed)
    {
        Vector3 directionToTransform = (transfromToMove.position - transform.position).normalized;
        Vector2 flatPosition = new Vector2(transform.position.x, transform.position.z);
        Vector2 flatTransformToMove = new Vector2(transfromToMove.position.x, transfromToMove.position.z);
        float flatDistanceToTransform = Vector2.Distance(flatTransformToMove, flatPosition);

        if (flatDistanceToTransform > 0.1f)
        {
            if (stepTimer > stepCadence) Debug.Log(flatDistanceToTransform);
            Vector3 velocity = directionToTransform * speed;
            if (!screwGravity) velocity.y = -gravity * Time.deltaTime;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToTransform.x, 0, directionToTransform.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
            characterController.Move(velocity * Time.deltaTime);
            animator.SetBool("Walking", (directionToTransform.magnitude > 0));
            return false;
        }

        animator.SetBool("Walking", false);

        return true;
    }

    Transform GetNewWanderPoint()
    {
        Debug.Log("Getting new point.");
        currentPoint = currentPoint.GetComponent<AccessiblePoints>().getRandomAccessiblePoint();
        return currentPoint.transform;
    }

    Transform GetClosestWanderPoint()
    {
        GameObject[] allWanderPoints = GameObject.FindObjectsOfType<GameObject>().Where(obj => (wanderPointLayer.value & (1 << obj.layer)) != 0).ToArray();

        System.Array.Sort(allWanderPoints, (obj1, obj2) =>
        {
            float distanceToObj1 = Vector3.Distance(obj1.transform.position, transform.position);
            float distanceToObj2 = Vector3.Distance(obj2.transform.position, transform.position);

            return distanceToObj1.CompareTo(distanceToObj2);
        });

        foreach (GameObject wanderPoint in allWanderPoints)
        {
            float distanceToWanderPoint = Vector3.Distance(transform.position, wanderPoint.transform.position);
            Vector3 directionToWanderPoint = wanderPoint.transform.position - transform.position;

            RaycastHit hit;
            bool visible = !Physics.Raycast(transform.position, directionToWanderPoint.normalized, out hit, distanceToWanderPoint, obstacleLayer);

            if (visible)
            {
                Debug.Log(wanderPoint.name);
                currentPoint = wanderPoint;

                return wanderPoint.transform;
            }
        }

        Debug.LogWarning("No wander points visible! Defaulting to closest non-visible point.");
        SetCollision(false);
        return allWanderPoints[0].transform;
    }

    void SetCollision(bool doCollision)
    {
        Physics.IgnoreLayerCollision(GetLayerNumberFromMask(alienLayer), GetLayerNumberFromMask(obstacleLayer), !doCollision);
        screwGravity = !doCollision;
    }

    void MakeNoise()
    {
        Debug.Log("Making noise.");
        alienNoise.clip = alienNoises[Random.Range(0, alienNoises.Length)];
        alienNoise.PlayOneShot(alienNoise.clip);
    }

    void MakeFootStep(float cadence)
    {
        if (stepTimer < cadence)
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

    int GetLayerNumberFromMask(LayerMask layerMask)
    {
        int layerIndex = -1;

        for (int i = 0; i < 32; i++)
        {
            if ((layerMask.value & (1 << i)) != 0)
            {
                layerIndex = i;
                break;
            }
        }

        return layerIndex;
    }
}

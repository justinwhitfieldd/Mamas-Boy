using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Rendering;
using Unity.VisualScripting;

public class WanderingSystem : MonoBehaviour
{
    public GameObject player;
    public float runSpeed = 2.0f;
    public float jumpScareRadius = 2.0f;
    public float alienFOV = 25f;
    public float triggerRadius = 8.0f;
    public LayerMask obstacleLayer;
    public LayerMask alienLayer;
    public LayerMask wanderPointLayer;
    public AudioClip jumpScareSound;
    public GameObject ambienceSystem;
    public GameObject startingPoint;
    public float rotationSpeed = 5.0f;
    public float waitTimer = 2.5f;
    public float waitProb = 0.25f;
    public float gravity = 15f;
    public AudioClip[] alienNoises;
    public AudioClip alienStepL;
    public AudioClip alienStepR;
    public bool disableCollision = false;
    public GameObject alienHead;
    private CharacterController characterController;
    private Animator animator;
    private GameObject currentPoint;
    private Transform currentTransform;
    private float timer = 0;
    private AudioSource alienNoise;
    private bool jumpScared = false;
    private float jumpScareRotationSpeed = 25.0f;
    private FPSController playerFPSController;
    private bool playerSeen = false;
    private bool screwGravity = false;
    private int stepCounter = 0;
    private bool isStopping = true;
    public float jumpScareCameraMovementSpeed = 5.0f;
public bool isEating = false;
    void Start()
    {
        alienNoise = GetComponent<AudioSource>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerFPSController = player.GetComponent<FPSController>();
        currentPoint = startingPoint;
        currentTransform = currentPoint.transform;

        if (disableCollision)
        {
            Physics.IgnoreLayerCollision(GetLayerNumberFromMask(alienLayer), GetLayerNumberFromMask(obstacleLayer), true);
            screwGravity = true;
        }
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        Vector3 directionToPlayer = player.transform.position - transform.position;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        RaycastHit hit;
        bool visible = !Physics.Raycast(transform.position, directionToPlayer.normalized, out hit, distanceToPlayer, obstacleLayer);

  #region Handles Jump Scaring Player
        if ((visible && (distanceToPlayer < jumpScareRadius)) || isEating)
        {
            ambienceSystem.SetActive(false);
            animator.SetBool("takedown", true);

            playerFPSController.canMove = false;

            if (distanceToPlayer <= 1.8f || isEating)
            {
                isEating = true;
                // Smoothly move the player's camera towards the alien's head position and rotation
                player.GetComponentInChildren<Camera>().transform.position = Vector3.Lerp(
                    player.GetComponentInChildren<Camera>().transform.position,
                    alienHead.transform.position,
                    Time.deltaTime * jumpScareCameraMovementSpeed
                );

                player.GetComponentInChildren<Camera>().transform.rotation = Quaternion.Lerp(
                    player.GetComponentInChildren<Camera>().transform.rotation,
                    alienHead.transform.rotation,
                    Time.deltaTime * jumpScareCameraMovementSpeed
                );
            }

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
            bool atPlayer = MoveToTransform(player.transform, runSpeed);
            // if (!atPlayer) MakeFootStep(runCadence);

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

        bool atTransform = MoveToTransform(currentTransform, 1f);

        if (atTransform)
        {
            if (!isStopping)
            {
                currentTransform = GetNewWanderPoint();
                return;
            }

            if (timer < waitTimer)
                timer += Time.deltaTime;
            else
            {
                Debug.Log("Stopped to smell the roses.");
                MakeNoise();
                currentTransform = GetNewWanderPoint();
                timer = 0;
            }
        }
        else
        {
            // MakeFootStep(stepCadence);
        }
        #endregion
    }

    bool MoveToTransform(Transform transfromToMove, float speed)
    {
        Vector3 directionToTransform = (transfromToMove.position - transform.position).normalized;
        Vector2 flatPosition = new Vector2(transform.position.x, transform.position.z);
        Vector2 flatTransformToMove = new Vector2(transfromToMove.position.x, transfromToMove.position.z);
        float flatDistanceToTransform = Vector2.Distance(flatTransformToMove, flatPosition);

        if (flatDistanceToTransform > 0.5f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToTransform.x, 0, directionToTransform.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
            if (!screwGravity) characterController.Move(new Vector3(0, -gravity * Time.deltaTime, 0));
            animator.speed = speed;
            animator.SetBool("Walking", (directionToTransform.magnitude > 0));
            return false;
        }

        if (isStopping) animator.SetBool("Walking", false);

        return true;
    }

    Transform GetNewWanderPoint()
    {
        float willStop = Random.Range(0.0f, 1.0f);
        if (willStop < waitProb) isStopping = true;
        else isStopping = false;

        SetCollision(true);

        stepCounter++;
        currentPoint = currentPoint.GetComponent<AccessiblePoints>().getRandomAccessiblePoint(stepCounter);
        Debug.Log("Heading to " + currentPoint.name + ".");
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
                Debug.Log("Heading to " + wanderPoint.name + " after charging.");
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
        if (!disableCollision)
        {
            Physics.IgnoreLayerCollision(GetLayerNumberFromMask(alienLayer), GetLayerNumberFromMask(obstacleLayer), !doCollision);
            screwGravity = !doCollision;
        }
    }

    void MakeNoise()
    {
        alienNoise.clip = alienNoises[Random.Range(0, alienNoises.Length)];
        alienNoise.PlayOneShot(alienNoise.clip);
    }

    void MakeFootStep(int isLeft)
    {
        if (isLeft == 1)
            alienNoise.clip = alienStepL;
        else
            alienNoise.clip = alienStepR;
        alienNoise.PlayOneShot(alienNoise.clip);
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

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Rendering;
using Unity.VisualScripting;
//using UnityEditor.Experimental.GraphView;

public class WanderingSystem : MonoBehaviour
{
    public GameObject player;
    public float runSpeed = 2.25f;
    public float jumpPower = 7f;
    public float jumpScareRadius = 2.0f;
    public float alienFOV = 25f;
    public float triggerRadius = 8.0f;
    public LayerMask obstacleLayer;
    public LayerMask interactableLayer;
    public LayerMask alienLayer;
    public AudioClip jumpScareSound;
    public GameObject jumpScareAmbiance;
    public GameObject ambienceSystem;
    public GameObject startingPoint;
    public float rotationSpeed = 5.0f;
    public float waitTimer = 2.5f;
    public float waitProb = 0.25f;
    public float gravity = 10f;
    public AudioClip[] alienNoises;
    public AudioClip[] alienStepLSounds; // Array to store the left foot sound clips
    public AudioClip[] alienStepRSounds; // Array to store the right foot sound clips
    public bool disableCollision = false;
    public GameObject alienHead;
    public bool isEating = false;
    public float jumpScareCameraMovementSpeed = 5.0f;
    public GameObject currentPoint;
    public bool freeze = false;
    public bool canJumpScare = true;

    private PauseMenu menuManager;
    private CharacterController characterController;
    private Animator animator;
    private Transform currentTransform;
    private float timer = 0;
    private AudioSource alienNoise;
    private bool jumpScared = false;
    private FPSController playerFPSController;
    private bool playerSeen = false;
    private int stepCounter = 0;
    private bool isStopping = true;
    private float speedPerSec = 0.0f;
    private Vector3 oldPosition;
    private Vector3 moveDirection = Vector3.zero;
    private bool obstacleVisible = true;
    private bool interactableVisible = true;
    private float speedTimer = 0.0f;
    private AudioSource jumpScareNoise;

    private void Start()
    {
        GameObject pauseMenuObject = GameObject.Find("MenuManager");
        menuManager = pauseMenuObject.GetComponent<PauseMenu>();
        alienNoise = GetComponent<AudioSource>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerFPSController = player.GetComponent<FPSController>();
        currentPoint = startingPoint;
        currentTransform = currentPoint.transform;
        oldPosition = gameObject.transform.position;
        jumpScareNoise = jumpScareAmbiance.GetComponent<AudioSource>();

        if (disableCollision) Physics.IgnoreLayerCollision(GetLayerNumberFromMask(alienLayer), GetLayerNumberFromMask(obstacleLayer), true);
    }

    private void Update()
    {
        if (freeze)
        {
            animator.SetBool("Walking", false);
            return;
        }

        speedPerSec = Vector3.Distance(oldPosition, transform.position) / Time.deltaTime;
        oldPosition = transform.position;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        Vector3 directionToPlayer = player.transform.position - transform.position;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        bool visible = GetVisibility(directionToPlayer, distanceToPlayer);

        #region Handles Jump Scaring Player
        if (canJumpScare && ((visible && (distanceToPlayer < jumpScareRadius)) || isEating))
        {
            ambienceSystem.SetActive(false);
            animator.SetBool("Takedown", true);

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
                jumpScareNoise.enabled = false;
                jumpScared = true;
            }

            return;
        }
        #endregion

        #region Handles Charging to Player

        if (canJumpScare && (visible && ((angleToPlayer < alienFOV) || (distanceToPlayer < triggerRadius))))
        {
            bool atPlayer = MoveToTransform(player.transform, runSpeed);
            if ((speedPerSec < 1.0f) && characterController.isGrounded)
            {
                Debug.LogWarning("Object in way of charge! Jumping.");
                moveDirection.y = jumpPower;
            }

            if (!playerSeen)
            {
                MakeNoise();
                jumpScareNoise.enabled = true;
                Debug.Log("Player seen! Starting charge.");
            }
            playerSeen = true;

            return;
        }
        else
        {
            if (playerSeen)
            {
                currentTransform = GetClosestWanderPoint();
                MakeNoise();
                jumpScareNoise.enabled = false;
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
            if ((speedPerSec < 0.1f) && (speedTimer > 0.5f))
            {
                Debug.Log("Alien stopped moving. Disabling collision.");
                SetCollision(false);
            }
            speedTimer += Time.deltaTime;
        }
        #endregion
    }

    private bool MoveToTransform(Transform transfromToMove, float speed)
    {
        Vector3 directionToTransform = (transfromToMove.position - transform.position).normalized;
        Vector2 flatPosition = new Vector2(transform.position.x, transform.position.z);
        Vector2 flatTransformToMove = new Vector2(transfromToMove.position.x, transfromToMove.position.z);
        float flatDistanceToTransform = Vector2.Distance(flatTransformToMove, flatPosition);

        if (flatDistanceToTransform > 0.5f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToTransform.x, 0, directionToTransform.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
            if (!characterController.isGrounded) moveDirection.y -= gravity;
            characterController.Move(moveDirection);
            animator.speed = speed;
            animator.SetBool("Walking", (directionToTransform.magnitude > 0));
            return false;
        }

        if (isStopping) animator.SetBool("Walking", false);

        return true;
    }

    private Transform GetNewWanderPoint()
    {
        float willStop = Random.Range(0.0f, 1.0f);
        if (willStop < waitProb) isStopping = true;
        else isStopping = false;

        speedTimer = 0.0f;

        SetCollision(true);

        stepCounter++;
        currentPoint = currentPoint.GetComponent<AccessiblePoints>().GetRandomAccessiblePoint(stepCounter);
        Debug.Log("The alien is heading to " + currentPoint.name + ".");
        return currentPoint.transform;
    }

    private bool GetVisibility(Vector3 direction, float distance)
    {
        RaycastHit hit;

        obstacleVisible = !Physics.Raycast(transform.position, direction.normalized, out hit, distance, obstacleLayer);
        interactableVisible = !Physics.Raycast(transform.position, direction.normalized, out hit, distance, interactableLayer);

        return obstacleVisible && interactableVisible;
    }

    private Transform GetClosestWanderPoint()
    {
        GameObject[] allWanderPoints = GameObject.FindGameObjectsWithTag("Wander Point");

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

            bool visible = GetVisibility(directionToWanderPoint, distanceToWanderPoint);

            if (visible)
            {
                Debug.Log("Player lost! The alien is heading to " + wanderPoint.name + " after charging.");
                currentPoint = wanderPoint;

                return wanderPoint.transform;
            }
        }

        Debug.LogWarning("No wander points visible! Alien is defaulting to closest non-visible point.");
        SetCollision(false);
        return allWanderPoints[0].transform;
    }

    private void SetCollision(bool doCollision)
    {
        if (!disableCollision) Physics.IgnoreLayerCollision(GetLayerNumberFromMask(alienLayer), GetLayerNumberFromMask(obstacleLayer), !doCollision);
    }

    private void MakeNoise()
    {
        alienNoise.clip = alienNoises[Random.Range(0, alienNoises.Length)];
        alienNoise.PlayOneShot(alienNoise.clip);

    }

    public void MakeFootStep(int isLeft)
    {
        if (isLeft == 1)
        {
            // Randomly select a sound clip from the left foot sounds array
            int randomIndex = Random.Range(0, alienStepLSounds.Length);
            alienNoise.clip = alienStepLSounds[randomIndex];
        }
        else
        {
            // Randomly select a sound clip from the right foot sounds array
            int randomIndex = Random.Range(0, alienStepRSounds.Length);
            alienNoise.clip = alienStepRSounds[randomIndex];
        }
        
        alienNoise.PlayOneShot(alienNoise.clip);
    }
    public void loseGame()
    {
        menuManager.GameOverLose();
    }
    private int GetLayerNumberFromMask(LayerMask layerMask)
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

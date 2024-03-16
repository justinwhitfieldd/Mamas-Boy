using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Rendering;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;

public class TargetingSystem : MonoBehaviour
{
    public GameObject alien;
    public GameObject player;
    public bool poweredDown = true;
    public float speed = 10f;
    public float destroyRadius = 4.0f;
    public LayerMask obstacleLayer;
    public LayerMask robotLayer;
    public AudioClip destroySound;
    public GameObject ambienceSystem;
    public GameObject startingPoint;
    public float rotationSpeed = 5.0f;
    public float gravity = 10f;
    public AudioClip robotStepL;
    public AudioClip robotStepR;
    public bool disableCollision = false;

    private CharacterController characterController;
    private Animator animator;
    private WanderingSystem alienController;
    private FPSController playerFPSController;
    private GameObject currentPoint;
    private Transform currentTransform;
    private AudioSource robotNoise;
    private bool destroyed = false;
    private float speedPerSec = 0.0f;
    private Vector3 oldPosition;
    private Vector3 moveDirection = Vector3.zero;
    private float speedTimer = 0.0f;
    private float destroyRotationSpeed = 25.0f;

    private void Start()
    {
        robotNoise = GetComponent<AudioSource>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        alienController = alien.GetComponent<WanderingSystem>();
        playerFPSController = player.GetComponent<FPSController>();
        currentPoint = startingPoint;
        currentTransform = currentPoint.transform;
        oldPosition = gameObject.transform.position;

        if (disableCollision) Physics.IgnoreLayerCollision(GetLayerNumberFromMask(robotLayer), GetLayerNumberFromMask(obstacleLayer), true);
    }

    private void Update()
    {
        if (poweredDown) return;
        alienController.canJumpScare = false;
        playerFPSController.canMove = false;

        speedPerSec = Vector3.Distance(oldPosition, transform.position) / Time.deltaTime;
        oldPosition = transform.position;

        float distanceToAlien = Vector3.Distance(transform.position, alien.transform.position);

        #region Handles Destroying the Alien
        if (distanceToAlien < destroyRadius)
        {
            alienController.freeze = true;
            ambienceSystem.SetActive(false);
            animator.speed = 0.1f;
            animator.SetBool("Attack", true);

            Quaternion alienRotation = Quaternion.LookRotation(transform.position - alien.transform.position);
            alien.transform.rotation = Quaternion.Lerp(alien.transform.rotation, alienRotation, Time.deltaTime * destroyRotationSpeed);

            Quaternion robotRotation = Quaternion.LookRotation(alien.transform.position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, robotRotation, Time.deltaTime * destroyRotationSpeed);

            if (!destroyed)
            {
                robotNoise.clip = destroySound;
                robotNoise.PlayOneShot(robotNoise.clip);
                destroyed = true;
            }

            return;
        }
        #endregion

        #region Handles Wandering to Points

        bool atTransform = MoveToTransform(currentTransform);

        if (atTransform)
        {
            currentTransform = GetNewWanderPoint();
        }
        else
        {
            if ((speedPerSec < 0.1f) && (speedTimer > 0.5f))
            {
                Debug.Log("Robot stopped moving. Disabling collision.");
                SetCollision(false);
            }
            speedTimer += Time.deltaTime;
        }
        #endregion
    }

    private bool MoveToTransform(Transform transfromToMove)
    {
        Vector3 directionToTransform = (transfromToMove.position - transform.position).normalized;
        Vector2 flatPosition = new Vector2(transform.position.x, transform.position.z);
        Vector2 flatTransformToMove = new Vector2(transfromToMove.position.x, transfromToMove.position.z);
        float flatDistanceToTransform = Vector2.Distance(flatTransformToMove, flatPosition);

        if (flatDistanceToTransform > 0.5f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToTransform.x, 0, directionToTransform.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
            moveDirection = directionToTransform * speed;
            if (!characterController.isGrounded) moveDirection.y -= gravity;
            characterController.Move(moveDirection * Time.deltaTime);
            animator.SetBool("Walking", (directionToTransform.magnitude > 0));
            return false;
        }

        animator.SetBool("Walking", false);

        return true;
    }

    private Transform GetNewWanderPoint()
    {
        speedTimer = 0.0f;

        SetCollision(true);

        currentPoint = currentPoint.GetComponent<AccessiblePoints>().GetPointClosestTo(alienController.currentPoint);

        Debug.Log("The robot is heading to " + currentPoint.name + ".");
        return currentPoint.transform;
    }

    private void SetCollision(bool doCollision)
    {
        if (!disableCollision) Physics.IgnoreLayerCollision(GetLayerNumberFromMask(robotLayer), GetLayerNumberFromMask(obstacleLayer), !doCollision);
    }

    public void MakeFootStep(int isLeft)
    {
        if (isLeft == 1)
            robotNoise.clip = robotStepL;
        else
            robotNoise.clip = robotStepR;
        robotNoise.PlayOneShot(robotNoise.clip);
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

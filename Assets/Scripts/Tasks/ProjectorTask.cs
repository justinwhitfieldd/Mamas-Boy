using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectorTask : MonoBehaviour, IInteractable
{
    // Reference to the FPSMovement script
    public FPSController FPSController;
    private MultiSkillBarMove multiskillBarMove;
    private PlaceInCamera placeInCamera;
    private TaskCounter taskCounter;
    [SerializeField] private InteractionPromptUI _interactionPromptUI;
    public bool taskFailed = false;
    public bool interacting = false;
    [SerializeField] int numWins = 2;
    public int Wins = 0;
    [SerializeField] private bool canInteract = true; // Flag to control if interaction is allowed
    [SerializeField] private float maxInteractDistance = 4f; // Maximum distance for interaction
    [SerializeField] private string _interactionPrompt;

    public string InteractionPrompt
    {
        get => _interactionPrompt;
        set => _interactionPrompt = value;
    }

    private void Start()
    {
        // Find avatar and camera GameObjects by tag
        GameObject characterObject = GameObject.FindWithTag("Player");
        FPSController = characterObject.GetComponent<FPSController>();
        // Get a reference to the SkillBarMove component attached to a child object
        multiskillBarMove = GetComponentInChildren<MultiSkillBarMove>();
        placeInCamera = GetComponentInChildren<PlaceInCamera>();
        // get player intteract UI
        GameObject canvasObject = GameObject.FindWithTag("MainCanvas");
        _interactionPromptUI = canvasObject.GetComponentInChildren<InteractionPromptUI>();
        // get task counter
        GameObject taskSystem = GameObject.FindWithTag("TaskSystem");
        taskCounter = taskSystem.GetComponentInChildren<TaskCounter>();
    }

    public bool Interact(Interactor inspector)
    {

        if (!canInteract)
        {
            return false; // Interaction not allowed
        }

        Wins = 0;

        // Stop player
        StopFPSmovement();
        _interactionPromptUI.Close();
        interacting = true;

        // Spawn background
        placeInCamera.SpawnforPlayer();

        // Make skill bar move and wait for it to finish
        StartCoroutine(StartAndFinishSkillBarMove());

        return true;
    }

    private IEnumerator StartAndFinishSkillBarMove()
    {

        // While num of wins you need is less than current wins
        while (numWins > Wins)
        {

            // Start the skill bar movement
            multiskillBarMove.StartAppearAndDisappear();

            // Wait for the skill bar movement to finish
            yield return StartCoroutine(multiskillBarMove.WaitForCompletion());

            // Get the result of the skill bar movement
            taskFailed = multiskillBarMove.taskFailed;

            if (taskFailed)
            {
                break;
            }
            else
            {
                Wins += 1;
            }
        }

        if (taskFailed)
        {
            _interactionPromptUI.SetUp("Stunned");
            // Once skill bar movement is done, despawn the background
            placeInCamera.DespawnforPlayer();
            yield return new WaitForSeconds(2.5f);
            _interactionPromptUI.SetUp("Reboot Oxygen Scrubber (Press E)");
            canInteract = true;
        }

        else
        {
            Debug.Log(" You completed the task");
            _interactionPromptUI.SetUp("Finished!");
            InteractionPrompt = "Finished!";
            taskCounter.IncrementCounter("oxygen"); // add 1 to tasks done
            canInteract = false;
            // Once skill bar movement is done, despawn the background
            placeInCamera.DespawnforPlayer();
        }

        // Restart player movement
        StartFPSmovement();
    }

    public void StopFPSmovement()
    {
        // Disable movement controls for the avatar and camera
        if (FPSController != null)
            FPSController.enabled = false;
    }

    public void StartFPSmovement()
    {
        // Enable movement controls for the avatar and camera
        if (FPSController != null)
            FPSController.enabled = true;
    }
    private void Update()
    {
        if (Vector3.Distance(FPSController.transform.position, transform.position) < maxInteractDistance && !interacting)
        {
            if (canInteract)
            {
                // Hide the UI panel
                _interactionPromptUI.SetUp("Reboot Oxygen Scrubber (Press E)");
                interacting = true;
            }
            else
            {
                _interactionPromptUI.SetUp("Finished!");
            }
        }
        // Check if player is too far away
        if (Vector3.Distance(FPSController.transform.position, transform.position) > maxInteractDistance && interacting)
        {
            // Hide the UI panel
            _interactionPromptUI.Close();
            interacting = false;
        }
    }       

}

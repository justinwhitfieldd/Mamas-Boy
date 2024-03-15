using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenTask : MonoBehaviour, IInteractable
{
    // Reference to the FPSMovement script
    public FPSController FPSController;
    private BarFill BarFill;
    private PlaceInCamera placeInCamera;
    private TaskCounter taskCounter;
    [SerializeField] GameObject scrollBar;
    [SerializeField] private InteractionPromptUI _interactionPromptUI;
    public bool taskFailed = false;
    [SerializeField] private bool canInteract = true; // Flag to control if interaction is allowed
    [SerializeField] private AudioSource loseSound;
    [SerializeField] private AudioSource winSound;
    [SerializeField] private string _interactionPrompt;
    public bool interacting = false;
    [SerializeField] private float maxInteractDistance = 4f;

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
        // Get a reference to the Barfill component attached to a child object
        BarFill = GetComponentInChildren<BarFill>();
        placeInCamera = GetComponentInChildren<PlaceInCamera>();
        // get player intteract UI
        GameObject canvasObject = GameObject.FindWithTag("MainCanvas");
        _interactionPromptUI = canvasObject.GetComponentInChildren<InteractionPromptUI>();
        // get task counter
        GameObject taskSystem = GameObject.FindWithTag("TaskSystem");
        taskCounter = taskSystem.GetComponentInChildren<TaskCounter>();
        scrollBar.SetActive(false);
    }

    public bool Interact(Interactor inspector)
    {

        if (!canInteract)
        {
            return false; // Interaction not allowed
        }

        // Stop player
        StopFPSmovement();

        // Spawn text
        placeInCamera.SpawnforPlayer();

        // Make progress bar move and wait for it to finish
        StartCoroutine(StartAndFinishProgressBarMove());

        return true;
    }

    private IEnumerator StartAndFinishProgressBarMove()
    {

        scrollBar.SetActive(true);

        // Start the skill bar movement
        BarFill.StartAppearAndDisappear();

        // Wait for the skill bar movement to finish
        yield return StartCoroutine(BarFill.WaitForCompletion());

        // Get the result of the skill bar movement
        taskFailed = BarFill.taskFailed;

        if (taskFailed)
        {
            loseSound.Play();
            _interactionPromptUI.SetUp("Stunned");
            // Once skill bar movement is done, despawn the background
            placeInCamera.DespawnforPlayer();
            yield return new WaitForSeconds(2.5f);
            _interactionPromptUI.SetUp("Restart Backup Power (E)");
            canInteract = true;
        }

        else
        {
            winSound.Play();
            Debug.Log(" You completed the task");
            _interactionPromptUI.SetUp("Finished!");
            InteractionPrompt = "Finished!";
            taskCounter.IncrementCounter("backup_generator"); // add 1 to tasks done
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
                _interactionPromptUI.SetUp("Restart Backup Generator (E)");
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

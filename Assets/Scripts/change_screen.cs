using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class change_screen : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;
    [SerializeField] public GameObject next_screen; // Manually assiGn this in the Inspector
    [SerializeField] public GameObject current_screen_text;
    [SerializeField] public GameObject thisPressECanvas;
    [SerializeField] public DoorOpenClose doorToBlock;
    [SerializeField] public GameObject doorPressECanvas;
    [SerializeField] private string _interactionPrompt;
    [SerializeField] private AudioSource alarm;
    public void Start(){
        alarm.Play();
        doorToBlock.canOpen = false;
    }
    public string InteractionPrompt
    {
        get => _interactionPrompt;
        set => _interactionPrompt = value;
    }

    public bool Interact(Interactor interactor)
    {
        alarm.Stop();
        Debug.Log("Switching to new screen");
        doorToBlock.canOpen = true;
        current_screen_text.gameObject.SetActive(false);
        thisPressECanvas.gameObject.SetActive(false);
        doorPressECanvas.gameObject.SetActive(true);
        next_screen.gameObject.SetActive(true);
        return true;
    }
}

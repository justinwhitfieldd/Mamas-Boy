using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class change_screen : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;
    [SerializeField] public GameObject next_screen; // Manually assiGn this in the Inspector
    [SerializeField] public GameObject current_screen_text;
    [SerializeField] private string _interactionPrompt;

    public string InteractionPrompt
    {
        get => _interactionPrompt;
        set => _interactionPrompt = value;
    }

    public bool Interact(Interactor interactor)
    {
        Debug.Log("Switching to new screen");
        current_screen_text.gameObject.SetActive(false);
        next_screen.gameObject.SetActive(true);
        return true;
    }
}

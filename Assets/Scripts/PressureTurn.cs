using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressureTurn : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;

    public string InteractionPrompt => _prompt;

    public bool Interact(Interactor inspector)
    {


        Debug.Log(message:"Working!");
        return true;
    }
}

using UnityEngine;
//stolen from https://www.youtube.com/watch?v=THmW4YolDok
public class DoorController : MonoBehaviour, IInteractable
{
    public Animator animator;

    [SerializeField] private string _interactionPrompt;

    public string InteractionPrompt
    {
        get => _interactionPrompt;
        set => _interactionPrompt = value;
    }

    public bool isDoorOpen = false;
    public bool Interact(Interactor interactor)
    {
        isDoorOpen = !isDoorOpen;
        animator.SetBool("character_nearby",isDoorOpen);
        Debug.Log(message:"Opening door");
        return true;
    }
}
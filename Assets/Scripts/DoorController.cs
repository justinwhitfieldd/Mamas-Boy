using UnityEngine;
//stolen from https://www.youtube.com/watch?v=THmW4YolDok
public class DoorController : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;
    public Animator animator;
    public string InteractionPrompt => _prompt;
    public bool isDoorOpen = false;
    public bool Interact(Interactor interactor)
    {
        isDoorOpen = !isDoorOpen;
        animator.SetBool("character_nearby",isDoorOpen);
        Debug.Log(message:"Opening door");
        return true;
    }
}
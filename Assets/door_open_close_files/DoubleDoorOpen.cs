using UnityEngine;
//stolen from https://www.youtube.com/watch?v=THmW4YolDok
public class doubleDoorController : MonoBehaviour, IInteractable
{
    public DoorOpenClose thedoor;
    public DoorOpenClose thedoor2;
    [SerializeField] private string _interactionPrompt;

    public string InteractionPrompt
    {
        get => _interactionPrompt;
        set => _interactionPrompt = value;
    }

    public bool isDoorOpen = false;
    public bool Interact(Interactor interactor)
    {
        thedoor.toggie();
        if (thedoor.isDoorOpen != thedoor2.isDoorOpen) thedoor2.toggie();
        return true;
    }
}
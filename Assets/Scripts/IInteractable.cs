using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//stolen from https://www.youtube.com/watch?v=THmW4YolDok

public interface IInteractable 
{
    public string InteractionPrompt { get;}
    public bool Interact(Interactor inspector);
}


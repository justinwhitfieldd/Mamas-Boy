using UnityEngine;
using UnityEngine.UI; // Ensure you're using the UI namespace

public class DoorStatusDisplay : MonoBehaviour
{
    public Image statusImage; // Assign in the inspector
    public Sprite openSprite; // Assign in the inspector
    public Sprite closedSprite; // Assign in the inspector
    public DoorController door; // Reference to your door script or component that knows the door's state

    void Update()
    {
        if (door.isDoorOpen) // Assuming your door script has an isOpen boolean
        {
            statusImage.sprite = openSprite;
        }
        else
        {
            statusImage.sprite = closedSprite;
        }
    }
}

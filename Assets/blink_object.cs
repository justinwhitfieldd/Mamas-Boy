using UnityEngine;
using UnityEngine.UI;

public class BlinkingUIElement : MonoBehaviour
{
    public bool isBlinking = false; // Public variable to control blinking

    private Image imageComponent; // Reference to the Image component
    private float blinkInterval = 1f; // Time in seconds for one blink cycle
    private float blinkTimer; // Timer to track the blink cycle

    void Start()
    {
        imageComponent = GetComponent<Image>(); // Get the Image component
    }

    void Update()
    {
        if (isBlinking)
        {
            BlinkUIElement();
        }
    }

    void BlinkUIElement()
    {
        blinkTimer += Time.deltaTime; // Increment the blink timer

        if (blinkTimer >= blinkInterval) // Check if it's time to switch visibility
        {
            imageComponent.enabled = !imageComponent.enabled; // Toggle visibility
            blinkTimer = 0f; // Reset the blink timer
        }
    }
}
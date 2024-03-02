using UnityEngine;
using UnityEngine.UIElements;

public class CursorController : MonoBehaviour
{
    private VisualElement cursorElement;
    public float mouseSensitivityX = 5.0f;
    public float mouseSensitivityY = 5.0f;

    // Assuming these dimensions match your UI view or screen.
    // Adjust these values based on your actual UI size.
    private float uiWidth = 1920; // Width of the UI
    private float uiHeight = 1080; // Height of the UI

    void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        cursorElement = root.Q<VisualElement>("mouse");
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivityX;
        float mouseY = -Input.GetAxis("Mouse Y") * mouseSensitivityY; // Invert Y-axis

        if(cursorElement != null)
        {
            float newLeft = Mathf.Clamp(cursorElement.style.left.value.value + mouseX, 0, uiWidth - cursorElement.layout.width);
            float newTop = Mathf.Clamp(cursorElement.style.top.value.value + mouseY, 0, uiHeight - cursorElement.layout.height);

            cursorElement.style.left = new StyleLength(newLeft);
            cursorElement.style.top = new StyleLength(newTop);
        }
    }
}

// using UnityEngine;
// using UnityEngine.UIElements;

// public class CursorController : MonoBehaviour
// {
//     private VisualElement cursorElement;
//     public float mouseSensitivityX = 5.0f;
//     public float mouseSensitivityY = 5.0f;

//     private float uiWidth = 1920; // Width of the UI
//     private float uiHeight = 1080; // Height of the UI

//     private VisualElement root;
//     private VisualElement previousHoveredElement;
//     private VisualElement[] maps;
//     private bool canClick = true; // For debouncing



//     void Start()
//     {
//         root = GetComponent<UIDocument>().rootVisualElement;
//         cursorElement = root.Q<VisualElement>("mouse");
//         cursorElement.pickingMode = PickingMode.Ignore;
//         // Initialize maps array for easier management
//         maps = new VisualElement[]
//         {
//             root.Q<VisualElement>("map1"),
//             root.Q<VisualElement>("map2"),
//             root.Q<VisualElement>("map3"),
//             root.Q<VisualElement>("map4"),
//             root.Q<VisualElement>("map5"),
//             root.Q<VisualElement>("map6")
//         };
//     }

//     void Update()
//     {
//         float mouseX = Input.GetAxis("Mouse X") * mouseSensitivityX;
//         float mouseY = -Input.GetAxis("Mouse Y") * mouseSensitivityY; // Invert Y-axis

//         if (cursorElement != null)
//         {
//             float newLeft = Mathf.Clamp(cursorElement.style.left.value.value + mouseX, 0, uiWidth - cursorElement.layout.width);
//             float newTop = Mathf.Clamp(cursorElement.style.top.value.value + mouseY, 0, uiHeight - cursorElement.layout.height);

//             cursorElement.style.left = new StyleLength(newLeft);
//             cursorElement.style.top = new StyleLength(newTop);

//             // After updating the cursor position, check for hover
//             CheckForHover();

//             // Check if the mouse button is pressed to simulate a click
//             if (Input.GetMouseButtonDown(0) && canClick)
//             {
//                 TriggerClickAction();
//                 StartCoroutine(DebounceClick());
//             }
//         }
//     }

//     void CheckForHover()
//     {
//         Vector2 cursorPosition = new Vector2(cursorElement.worldBound.center.x, cursorElement.worldBound.center.y);
//         VisualElement hoveredElement = root.panel.Pick(cursorPosition);

//         if (hoveredElement != null && hoveredElement != root)
//         {
//             if (hoveredElement != previousHoveredElement)
//             {
//                 if (previousHoveredElement != null)
//                 {
//                     previousHoveredElement.EnableInClassList("hover", false);
//                 }
//                 hoveredElement.EnableInClassList("hover", true);
//                 previousHoveredElement = hoveredElement;

//             }
//         }
//         else
//         {
//             if (previousHoveredElement != null)
//             {
//                 previousHoveredElement.EnableInClassList("hover", false);
//                 previousHoveredElement = null;
//             }
//         }
//     }

//     void TriggerClickAction()
//     {
//         Vector2 cursorPosition = new Vector2(cursorElement.worldBound.center.x, cursorElement.worldBound.center.y);
//         VisualElement clickedElement = root.panel.Pick(cursorPosition);

//         if (clickedElement != null && clickedElement != root)
//         {
//             // Check if the clicked element can receive focus
//             if (clickedElement is Focusable focusableElement)
//             {
//                 // Set focus on the focusable element
//                 focusableElement.Focus();
//                 if (clickedElement.name == "item1")
//                 {
//                     map1.style.display = DisplayStyle.Flex;
//                     map2.style.display = DisplayStyle.None;
//                     map3.style.display = DisplayStyle.None;
//                     map4.style.display = DisplayStyle.None;
//                     map5.style.display = DisplayStyle.None;
//                     map6.style.display = DisplayStyle.None;
//                 }
//                  else if (clickedElement.name == "item2")
//                 {
//                     map1.style.display = DisplayStyle.None;
//                     map2.style.display = DisplayStyle.Flex;
//                     map3.style.display = DisplayStyle.None;
//                     map4.style.display = DisplayStyle.None;
//                     map5.style.display = DisplayStyle.None;
//                     map6.style.display = DisplayStyle.None;
//                 }
//                 else if (clickedElement.name == "item3")
//                 {
//                     map1.style.display = DisplayStyle.None;
//                     map2.style.display = DisplayStyle.None;
//                     map3.style.display = DisplayStyle.Flex;
//                     map4.style.display = DisplayStyle.None;
//                     map5.style.display = DisplayStyle.None;
//                     map6.style.display = DisplayStyle.None;
//                 }
//                 else if (clickedElement.name == "item4")
//                 {
//                     map1.style.display = DisplayStyle.None;
//                     map2.style.display = DisplayStyle.None;
//                     map3.style.display = DisplayStyle.None;
//                     map4.style.display = DisplayStyle.Flex;
//                     map5.style.display = DisplayStyle.None;
//                     map6.style.display = DisplayStyle.None;
//                 }
//                 else if (clickedElement.name == "item5")
//                 {
//                     map1.style.display = DisplayStyle.None;
//                     map2.style.display = DisplayStyle.None;
//                     map3.style.display = DisplayStyle.None;
//                     map4.style.display = DisplayStyle.None;
//                     map5.style.display = DisplayStyle.Flex;
//                     map6.style.display = DisplayStyle.None;
//                 }
//                 else if (clickedElement.name == "item6")
//                 {
//                     map1.style.display = DisplayStyle.None;
//                     map2.style.display = DisplayStyle.None;
//                     map3.style.display = DisplayStyle.None;
//                     map4.style.display = DisplayStyle.None;
//                     map5.style.display = DisplayStyle.None;
//                     map6.style.display = DisplayStyle.Flex;
//                 }
//                 // Optionally, remove focus from the previously focused element if it's different
//                 if (previousHoveredElement != null && previousHoveredElement != focusableElement)
//                 {
//                     previousHoveredElement.EnableInClassList("focused", false); // Example of removing a "focused" class
//                 }

//                 // Indicate visually that the element is now focused
//                 clickedElement.EnableInClassList("focused", true); // Assuming you have a "focused" style class

//                 // Update the reference to the currently focused element
//                 previousHoveredElement = clickedElement;
//             }

//             // Log the action or the element name for debugging
//             Debug.Log($"Clicked and focused element: {clickedElement.name}");
//         }
//     }

// }

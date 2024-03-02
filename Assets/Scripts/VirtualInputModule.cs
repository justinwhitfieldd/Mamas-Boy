using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

[AddComponentMenu("Event/Virtual Input Module")]
public class VirtualInputModule : PointerInputModule
{

    public RectTransform VirtualCursor;
    public Camera CanvasCamera;

    public string HorizontalAxis = "Mouse X";

    /// <summary>
    /// Name of the vertical axis for movement (if axis events are used).
    /// </summary>
    public string VerticalAxis = "Mouse Y";

    /// <summary>
    /// Name of the submit button.
    /// </summary>
    public string SubmitButton = "Submit";

    /// <summary>
    /// Name of the cancel button.
    /// </summary>
    public string CancelButton = "Cancel";

    public float InputActionsPerSecond = 10;
    public float RepeatDelay = 0.5f;

    private float _prevActionTime;
    private Vector2 _lastMoveVector;
    private int _consecutiveMoveCount = 0;

    private Vector2 _lastMousePosition;
    private Vector2 _mousePosition;

    private GameObject _currentFocusedGameObject;

    protected VirtualInputModule()
    {
    }

    [Obsolete("Mode is no longer needed on input module as it handles both mouse and keyboard simultaneously.", false)]
    public enum InputMode
    {
        Mouse,
        Buttons
    }

    [Obsolete("Mode is no longer needed on input module as it handles both mouse and keyboard simultaneously.", false)]
    public InputMode inputMode
    {
        get { return InputMode.Mouse; }
    }

    [SerializeField]
    [FormerlySerializedAs("m_AllowActivationOnMobileDevice")]
    private bool m_ForceModuleActive;

    [Obsolete("allowActivationOnMobileDevice has been deprecated. Use forceModuleActive instead (UnityUpgradable) -> forceModuleActive")]
    public bool allowActivationOnMobileDevice
    {
        get { return m_ForceModuleActive; }
        set { m_ForceModuleActive = value; }
    }

    public bool forceModuleActive
    {
        get { return m_ForceModuleActive; }
        set { m_ForceModuleActive = value; }
    }

    public float inputActionsPerSecond
    {
        get { return InputActionsPerSecond; }
        set { InputActionsPerSecond = value; }
    }

    public float repeatDelay
    {
        get { return RepeatDelay; }
        set { RepeatDelay = value; }
    }

    /// <summary>
    /// Name of the horizontal axis for movement (if axis events are used).
    /// </summary>
    public string horizontalAxis
    {
        get { return HorizontalAxis; }
        set { HorizontalAxis = value; }
    }

    /// <summary>
    /// Name of the vertical axis for movement (if axis events are used).
    /// </summary>
    public string verticalAxis
    {
        get { return VerticalAxis; }
        set { VerticalAxis = value; }
    }

    public string submitButton
    {
        get { return SubmitButton; }
        set { SubmitButton = value; }
    }

    public string cancelButton
    {
        get { return CancelButton; }
        set { CancelButton = value; }
    }

    public override void UpdateModule()
    {
        _lastMousePosition = _mousePosition;
        _mousePosition = VirtualCursor.anchoredPosition;
    }

    public override bool IsModuleSupported()
    {
        return m_ForceModuleActive || input.mousePresent || input.touchSupported;
    }

    public override bool ShouldActivateModule()
    {
        if (!base.ShouldActivateModule())
            return false;

        var shouldActivate = m_ForceModuleActive;
        shouldActivate |= input.GetButtonDown(SubmitButton);
        shouldActivate |= input.GetButtonDown(CancelButton);
        shouldActivate |= !Mathf.Approximately(input.GetAxisRaw(HorizontalAxis), 0.0f);
        shouldActivate |= !Mathf.Approximately(input.GetAxisRaw(VerticalAxis), 0.0f);
        shouldActivate |= (_mousePosition - _lastMousePosition).sqrMagnitude > 0.0f;
        shouldActivate |= input.GetMouseButtonDown(0);

        if (input.touchCount > 0)
            shouldActivate = true;

        return shouldActivate;
    }

    public override void ActivateModule()
    {
        base.ActivateModule();
        _mousePosition = VirtualCursor.anchoredPosition;
        _lastMousePosition = VirtualCursor.anchoredPosition;

        var toSelect = eventSystem.currentSelectedGameObject ?? eventSystem.firstSelectedGameObject;
        eventSystem.SetSelectedGameObject(toSelect, GetBaseEventData());
    }

    public override void DeactivateModule()
    {
        base.DeactivateModule();
        ClearSelection();
    }

    public override void Process()
    {
        bool usedEvent = SendUpdateEventToSelectedObject();

        if (eventSystem.sendNavigationEvents)
        {
            if (!usedEvent)
                usedEvent |= SendMoveEventToSelectedObject();

            if (!usedEvent)
                SendSubmitEventToSelectedObject();
        }

        // touch needs to take precedence because of the mouse emulation layer
        if (!ProcessTouchEvents() && input.mousePresent)
            ProcessMouseEvent();
    }

    private bool ProcessTouchEvents()
    {
        for (int i = 0; i < input.touchCount; ++i)
        {
            Touch touch = input.GetTouch(i);

            if (touch.type == TouchType.Indirect)
                continue;

            bool released;
            bool pressed;
            var pointer = GetTouchPointerEventData(touch, out pressed, out released);

            ProcessTouchPress(pointer, pressed, released);

            if (!released)
            {
                ProcessMove(pointer);
                ProcessDrag(pointer);
            }
            else
                RemovePointerData(pointer);
        }
        return input.touchCount > 0;
    }

    protected void ProcessTouchPress(PointerEventData pointerEvent, bool pressed, bool released)
    {
        var currentOverGo = pointerEvent.pointerCurrentRaycast.gameObject;

        // PointerDown notification
        if (pressed)
        {
            pointerEvent.eligibleForClick = true;
            pointerEvent.delta = Vector2.zero;
            pointerEvent.dragging = false;
            pointerEvent.useDragThreshold = true;
            pointerEvent.pressPosition = pointerEvent.position;
            pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;

            DeselectIfSelectionChanged(currentOverGo, pointerEvent);

            if (pointerEvent.pointerEnter != currentOverGo)
            {
                // send a pointer enter to the touched element if it isn't the one to select...
                HandlePointerExitAndEnter(pointerEvent, currentOverGo);
                pointerEvent.pointerEnter = currentOverGo;
            }

            // search for the control that will receive the press
            // if we can't find a press handler set the press
            // handler to be what would receive a click.
            var newPressed = ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.pointerDownHandler);

            // didnt find a press handler... search for a click handler
            if (newPressed == null)
                newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

            // Debug.Log("Pressed: " + newPressed);

            float time = Time.unscaledTime;

            if (newPressed == pointerEvent.lastPress)
            {
                var diffTime = time - pointerEvent.clickTime;
                if (diffTime < 0.3f)
                    ++pointerEvent.clickCount;
                else
                    pointerEvent.clickCount = 1;

                pointerEvent.clickTime = time;
            }
            else
            {
                pointerEvent.clickCount = 1;
            }

            pointerEvent.pointerPress = newPressed;
            pointerEvent.rawPointerPress = currentOverGo;

            pointerEvent.clickTime = time;

            // Save the drag handler as well
            pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentOverGo);

            if (pointerEvent.pointerDrag != null)
                ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);
        }

        // PointerUp notification
        if (released)
        {
            // Debug.Log("Executing pressup on: " + pointer.pointerPress);
            ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);

            // Debug.Log("KeyCode: " + pointer.eventData.keyCode);

            // see if we mouse up on the same element that we clicked on...
            var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

            // PointerClick and Drop events
            if (pointerEvent.pointerPress == pointerUpHandler && pointerEvent.eligibleForClick)
            {
                ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
            }
            else if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
            {
                ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.dropHandler);
            }

            pointerEvent.eligibleForClick = false;
            pointerEvent.pointerPress = null;
            pointerEvent.rawPointerPress = null;

            if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
                ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);

            pointerEvent.dragging = false;
            pointerEvent.pointerDrag = null;

            if (pointerEvent.pointerDrag != null)
                ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);

            pointerEvent.pointerDrag = null;

            // send exit events as we need to simulate this on touch up on touch device
            ExecuteEvents.ExecuteHierarchy(pointerEvent.pointerEnter, pointerEvent, ExecuteEvents.pointerExitHandler);
            pointerEvent.pointerEnter = null;
        }
    }

    /// <summary>
    /// Process submit keys.
    /// </summary>
    protected bool SendSubmitEventToSelectedObject()
    {
        if (eventSystem.currentSelectedGameObject == null)
            return false;

        var data = GetBaseEventData();
        if (input.GetButtonDown(SubmitButton))
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.submitHandler);

        if (input.GetButtonDown(CancelButton))
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.cancelHandler);
        return data.used;
    }

    private Vector2 GetRawMoveVector()
    {
        Vector2 move = Vector2.zero;
        move.x = input.GetAxisRaw(HorizontalAxis);
        move.y = input.GetAxisRaw(VerticalAxis);

        if (input.GetButtonDown(HorizontalAxis))
        {
            if (move.x < 0)
                move.x = -1f;
            if (move.x > 0)
                move.x = 1f;
        }
        if (input.GetButtonDown(VerticalAxis))
        {
            if (move.y < 0)
                move.y = -1f;
            if (move.y > 0)
                move.y = 1f;
        }
        return move;
    }

    /// <summary>
    /// Process keyboard events.
    /// </summary>
    protected bool SendMoveEventToSelectedObject()
    {
        float time = Time.unscaledTime;

        Vector2 movement = GetRawMoveVector();
        if (Mathf.Approximately(movement.x, 0f) && Mathf.Approximately(movement.y, 0f))
        {
            _consecutiveMoveCount = 0;
            return false;
        }

        // If user pressed key again, always allow event
        bool allow = input.GetButtonDown(HorizontalAxis) || input.GetButtonDown(VerticalAxis);
        bool similarDir = (Vector2.Dot(movement, _lastMoveVector) > 0);
        if (!allow)
        {
            // Otherwise, user held down key or axis.
            // If direction didn't change at least 90 degrees, wait for delay before allowing consequtive event.
            if (similarDir && _consecutiveMoveCount == 1)
                allow = (time > _prevActionTime + RepeatDelay);
            // If direction changed at least 90 degree, or we already had the delay, repeat at repeat rate.
            else
                allow = (time > _prevActionTime + 1f / InputActionsPerSecond);
        }
        if (!allow)
            return false;

        // Debug.Log(m_ProcessingEvent.rawType + " axis:" + m_AllowAxisEvents + " value:" + "(" + x + "," + y + ")");
        var axisEventData = GetAxisEventData(movement.x, movement.y, 0.6f);

        if (axisEventData.moveDir != MoveDirection.None)
        {
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler);
            if (!similarDir)
                _consecutiveMoveCount = 0;
            _consecutiveMoveCount++;
            _prevActionTime = time;
            _lastMoveVector = movement;
        }
        else
        {
            _consecutiveMoveCount = 0;
        }

        return axisEventData.used;
    }

    protected void ProcessMouseEvent()
    {
        ProcessMouseEvent(0);
    }

    [Obsolete("This method is no longer checked, overriding it with return true does nothing!")]
    protected virtual bool ForceAutoSelect()
    {
        return false;
    }

    /// <summary>
    /// Process all mouse events.
    /// </summary>
    protected void ProcessMouseEvent(int id)
    {
        var mouseData = GetMousePointerEventData(id);
        var leftButtonData = mouseData.GetButtonState(PointerEventData.InputButton.Left).eventData;

        _currentFocusedGameObject = leftButtonData.buttonData.pointerCurrentRaycast.gameObject;

        // Process the first mouse button fully
        ProcessMousePress(leftButtonData);
        ProcessMove(leftButtonData.buttonData);
        ProcessDrag(leftButtonData.buttonData);

        // Now process right / middle clicks
        ProcessMousePress(mouseData.GetButtonState(PointerEventData.InputButton.Right).eventData);
        ProcessDrag(mouseData.GetButtonState(PointerEventData.InputButton.Right).eventData.buttonData);
        ProcessMousePress(mouseData.GetButtonState(PointerEventData.InputButton.Middle).eventData);
        ProcessDrag(mouseData.GetButtonState(PointerEventData.InputButton.Middle).eventData.buttonData);

        if (!Mathf.Approximately(leftButtonData.buttonData.scrollDelta.sqrMagnitude, 0.0f))
        {
            var scrollHandler = ExecuteEvents.GetEventHandler<IScrollHandler>(leftButtonData.buttonData.pointerCurrentRaycast.gameObject);
            ExecuteEvents.ExecuteHierarchy(scrollHandler, leftButtonData.buttonData, ExecuteEvents.scrollHandler);
        }
    }

    protected bool SendUpdateEventToSelectedObject()
    {
        if (eventSystem.currentSelectedGameObject == null)
            return false;

        var data = GetBaseEventData();
        ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.updateSelectedHandler);
        return data.used;
    }

    /// <summary>
    /// Process the current mouse press.
    /// </summary>
    protected void ProcessMousePress(MouseButtonEventData data)
    {
        var pointerEvent = data.buttonData;
        var currentOverGo = pointerEvent.pointerCurrentRaycast.gameObject;

        // PointerDown notification
        if (data.PressedThisFrame())
        {
            pointerEvent.eligibleForClick = true;
            pointerEvent.delta = Vector2.zero;
            pointerEvent.dragging = false;
            pointerEvent.useDragThreshold = true;
            pointerEvent.pressPosition = pointerEvent.position;
            pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;
            pointerEvent.position = VirtualCursor.anchoredPosition;

            DeselectIfSelectionChanged(currentOverGo, pointerEvent);

            // search for the control that will receive the press
            // if we can't find a press handler set the press
            // handler to be what would receive a click.
            var newPressed = ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.pointerDownHandler);

            // didnt find a press handler... search for a click handler
            if (newPressed == null)
                newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

            // Debug.Log("Pressed: " + newPressed);

            float time = Time.unscaledTime;

            if (newPressed == pointerEvent.lastPress)
            {
                var diffTime = time - pointerEvent.clickTime;
                if (diffTime < 0.3f)
                    ++pointerEvent.clickCount;
                else
                    pointerEvent.clickCount = 1;

                pointerEvent.clickTime = time;
            }
            else
            {
                pointerEvent.clickCount = 1;
            }

            pointerEvent.pointerPress = newPressed;
            pointerEvent.rawPointerPress = currentOverGo;

            pointerEvent.clickTime = time;

            // Save the drag handler as well
            pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentOverGo);

            if (pointerEvent.pointerDrag != null)
                ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);
        }

        // PointerUp notification
        if (data.ReleasedThisFrame())
        {
            // Debug.Log("Executing pressup on: " + pointer.pointerPress);
            ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);

            // Debug.Log("KeyCode: " + pointer.eventData.keyCode);

            // see if we mouse up on the same element that we clicked on...
            var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

            // PointerClick and Drop events
            if (pointerEvent.pointerPress == pointerUpHandler && pointerEvent.eligibleForClick)
            {
                ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
            }
            else if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
            {
                ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.dropHandler);
            }

            pointerEvent.eligibleForClick = false;
            pointerEvent.pointerPress = null;
            pointerEvent.rawPointerPress = null;

            if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
                ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);

            pointerEvent.dragging = false;
            pointerEvent.pointerDrag = null;

            // redo pointer enter / exit to refresh state
            // so that if we moused over somethign that ignored it before
            // due to having pressed on something else
            // it now gets it.
            if (currentOverGo != pointerEvent.pointerEnter)
            {
                HandlePointerExitAndEnter(pointerEvent, null);
                HandlePointerExitAndEnter(pointerEvent, currentOverGo);
            }
        }
    }

    protected GameObject GetCurrentFocusedGameObject()
    {
        return _currentFocusedGameObject;
    }

    private readonly MouseState m_MouseState = new MouseState();

    protected override MouseState GetMousePointerEventData(int id)
    {
        // Populate the left button...
        PointerEventData leftData;
        var created = GetPointerData(kMouseLeftId, out leftData, true);

        leftData.Reset();

        var screenSpaceCursorPosition = RectTransformUtility.WorldToScreenPoint(CanvasCamera,
            VirtualCursor.position);

        if (created)
            leftData.position = screenSpaceCursorPosition;

        Vector2 pos = screenSpaceCursorPosition;
        leftData.delta = pos - leftData.position;
        leftData.position = pos;
        leftData.scrollDelta = Input.mouseScrollDelta;
        leftData.button = PointerEventData.InputButton.Left;
        eventSystem.RaycastAll(leftData, m_RaycastResultCache);
        var raycast = FindFirstRaycast(m_RaycastResultCache);
        leftData.pointerCurrentRaycast = raycast;
        m_RaycastResultCache.Clear();

        // copy the apropriate data into right and middle slots
        PointerEventData rightData;
        GetPointerData(kMouseRightId, out rightData, true);
        CopyFromTo(leftData, rightData);
        rightData.button = PointerEventData.InputButton.Right;

        PointerEventData middleData;
        GetPointerData(kMouseMiddleId, out middleData, true);
        CopyFromTo(leftData, middleData);
        middleData.button = PointerEventData.InputButton.Middle;

        m_MouseState.SetButtonState(PointerEventData.InputButton.Left, StateForMouseButton(0), leftData);
        m_MouseState.SetButtonState(PointerEventData.InputButton.Right, StateForMouseButton(1), rightData);
        m_MouseState.SetButtonState(PointerEventData.InputButton.Middle, StateForMouseButton(2), middleData);

        return m_MouseState;
    }
}
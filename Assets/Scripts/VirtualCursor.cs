using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualCursor : UIBehaviour
{
    public RectTransform Canvas;

    public string HorizontalAxis = "Mouse X";
    public string VerticalAxis = "Mouse Y";

    public float Sensitivity;
    public bool EnableOnStart;

    private Rect _canvasRect;
    private RectTransform _rectTransform;

    public bool Enabled { get; private set; }

    protected override void Awake()
    {
        _canvasRect = Canvas.rect;
        _rectTransform = GetComponent<RectTransform>();
    }

    protected override void Start()
    {
        if (EnableOnStart)
        {
            Enable();
        }
    }

    public void Enable()
    {
        LockCursor();
        Enabled = true;
    }

    public void Disable()
    {
        UnlockCursor();
        Enabled = false;
    }

    void LockCursor()
    {
        if (Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void UnlockCursor()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void Update()
    {
        if (Enabled)
        {
            MovePointer();
        }
    }

    private void MovePointer()
    {
        if (_rectTransform == null)
        {
            return;
        }

        var movement = new Vector2(Input.GetAxis(HorizontalAxis), Input.GetAxis(VerticalAxis)) * Sensitivity;
        _rectTransform.anchoredPosition = Clamp(_rectTransform.anchoredPosition + movement);
    }

    private Vector2 Clamp(Vector2 pos)
    {
        pos.x = Mathf.Clamp(pos.x, -1 * _canvasRect.width / 2, _canvasRect.width / 2);
        pos.y = Mathf.Clamp(pos.y, -1 * _canvasRect.height / 2, _canvasRect.height / 2);
        return pos;
    }
}
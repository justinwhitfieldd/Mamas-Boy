using UnityEngine;
using UnityEngine.EventSystems;

public enum PhoneInputMode
{
    Virtual,
    Standalone
}

public class InputManager : UIBehaviour
{
    public VirtualInputModule VirtualInputModule;
    public VirtualCursor VirtualCursor;
    public StandaloneInputModule StandaloneInputModule;

    public PhoneInputMode CurrentInputMode { get; private set; }

    protected override void Awake()
    {
        CurrentInputMode = PhoneInputMode.Standalone;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            SetInputMode(CurrentInputMode == PhoneInputMode.Standalone
                ? PhoneInputMode.Virtual
                : PhoneInputMode.Standalone);
        }
    }

    public void SetInputMode(PhoneInputMode mode)
    {
        if (mode == PhoneInputMode.Virtual)
        {
            StandaloneInputModule.gameObject.SetActive(false);
            StandaloneInputModule.DeactivateModule();

            VirtualInputModule.gameObject.SetActive(true);
            VirtualInputModule.ActivateModule();
            VirtualCursor.Enable();
        }
        else if (mode == PhoneInputMode.Standalone)
        {
            VirtualCursor.Disable();
            VirtualInputModule.gameObject.SetActive(false);
            VirtualInputModule.DeactivateModule();

            StandaloneInputModule.gameObject.SetActive(true);
            StandaloneInputModule.ActivateModule();
        }
        CurrentInputMode = mode;
    }
}
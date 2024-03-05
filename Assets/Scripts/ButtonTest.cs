using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonTest : UIBehaviour
{
    public void OnClick()
    {
        Debug.Log("Click: " + transform.name);
    }
}

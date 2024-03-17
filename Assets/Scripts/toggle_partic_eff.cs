using UnityEngine;

public class ParentToggle : MonoBehaviour
{
    public GameObject fireStream;
    public void ToggleParent(int state)
    {
        if (transform.parent != null)
        {
            fireStream.SetActive(true);
            Debug.Log("hello i am an animation event");
            transform.parent.gameObject.SetActive(state == 1);
        }
    }
}
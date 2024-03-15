using UnityEngine;
//stolen from https://www.youtube.com/watch?v=THmW4YolDok

public class Interactor : MonoBehaviour
{
    [SerializeField] private Transform _interactionPoint;
    [SerializeField] private float _interactionPointRadius = 0.5f;
    [SerializeField] private LayerMask _interactableMask;
    //[SerializeField] private InteractionPromptUI _interactionPromptUI;

    private readonly Collider[] _colliders = new Collider[3];
    [SerializeField] private int _numFound;

    private IInteractable _interactable;

    private void Update()
    {
        _numFound = Physics.OverlapSphereNonAlloc(_interactionPoint.position,_interactionPointRadius,_colliders,_interactableMask);

        if(_numFound > 0)
        {
            var _interactable = _colliders[0].GetComponent<IInteractable>();

            if(_interactable != null)
            {

                //if (!_interactionPromptUI.IsDisplayed) _interactionPromptUI.SetUp(_interactable.InteractionPrompt);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    _interactable.Interact(this);
                //    _interactionPromptUI.Close(); //This does not work, gotta figure our how to remove when E is pressed
                }
                
            }
        }

        else
        {
            if (_interactable != null) _interactable = null;
        //    if (_interactionPromptUI.IsDisplayed) _interactionPromptUI.Close();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_interactionPoint.position, _interactionPointRadius);
    }
}


using UnityEngine;
using UnityEngine.InputSystem;

public class InteractDetector : MonoBehaviour
{
    private IInteractable interactableRange = null;
    public GameObject interecionIcon;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        interecionIcon.SetActive(false);    
    }
    void Update()
    {
        if (interactableRange != null && Input.GetKeyDown(KeyCode.B))
        {
            interactableRange.Interact();
        }
    }

    //public void OnInteract(InputAction.CallbackContext context)
    //{
    //    if (context.performed)
    //    {
    //        interactableRange?.Interact();
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
        {
            interactableRange = interactable;
            interecionIcon.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable == interactableRange)
        {
            interactableRange = null;
            interecionIcon.SetActive(false);
        }
    }
}

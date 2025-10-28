using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    public InteractableI InteractableInRange = null;
    public GameObject InteractionIcon;

    // Start is called before the first frame update
    void Start()
    {
        InteractionIcon.SetActive(false);

    }

    public void Interact(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            InteractableInRange?.Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out InteractableI Interactable) && Interactable.canInteract())
        {
            InteractableInRange = Interactable;
            InteractionIcon.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out InteractableI Interactable) && Interactable == InteractableInRange)
        {
            InteractableInRange = null;
            InteractionIcon.SetActive(false);
        }
    }
}

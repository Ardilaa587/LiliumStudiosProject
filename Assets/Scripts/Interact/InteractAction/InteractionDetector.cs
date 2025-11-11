using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditorInternal;
#endif
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

    void Update()
    {
        if (InteractableInRange == null)
        {
            if (InteractionIcon.activeSelf)
            {
                InteractionIcon.SetActive(false);
            }
            return;
        }

        bool canCurrentlyInteract = InteractableInRange.canInteract();

        if (canCurrentlyInteract && !InteractionIcon.activeSelf)
        {
            InteractionIcon.SetActive(true);
        }
        else if (!canCurrentlyInteract && InteractionIcon.activeSelf)
        {
            InteractionIcon.SetActive(false);
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            if (InteractableInRange != null && InteractableInRange.canInteract())
            {
                InteractableInRange?.Interact(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out InteractableI Interactable) && Interactable.canInteract())
        {
            InteractableInRange = Interactable;
            if (Interactable.canInteract())
            {
                InteractionIcon.SetActive(true);
            }
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

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
        // 1. Si NO hay un objeto en rango, no hacer nada.
        if (InteractableInRange == null)
        {
            // Asegurarse de que el icono está apagado si no hay nada en rango.
            if (InteractionIcon.activeSelf)
            {
                InteractionIcon.SetActive(false);
            }
            return;
        }

        // 2. Si HAY un objeto en rango, comprobar si actualmente se puede interactuar.
        bool canCurrentlyInteract = InteractableInRange.canInteract();

        // 3. Sincronizar el ícono con el estado actual del objeto.
        if (canCurrentlyInteract && !InteractionIcon.activeSelf)
        {
            // Si SÍ se puede interactuar y el ícono está apagado, ENCIÉNDELO.
            InteractionIcon.SetActive(true);
        }
        else if (!canCurrentlyInteract && InteractionIcon.activeSelf)
        {
            // Si NO se puede interactuar y el ícono está encendido, APÁGALO.
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

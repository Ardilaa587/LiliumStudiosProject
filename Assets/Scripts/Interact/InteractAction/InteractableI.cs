using UnityEngine;

public interface InteractableI
{
    void Interact(GameObject user);
    bool canInteract(); 
}

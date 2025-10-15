using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUps : MonoBehaviour, InteractableI
{
    [SerializeField] private GameObject pickUp;
    

    private bool isPickedUp = false;

    void Start()
    {
        
    }

    public bool canInteract()
    {
        return !isPickedUp;
    }

    public void Interact()
    {
        CameraEffects cameraEffects = Camera.main.GetComponent<CameraEffects>();

        if (isPickedUp) return;

        if (pickUp != null)
        {
            pickUp.SetActive(false);
            cameraEffects.effectActive = false;
        }

       

        isPickedUp = true;
        gameObject.SetActive(false);
    }
}

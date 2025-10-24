using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUps : MonoBehaviour, InteractableI
{
    
    public bool isPickedUp = false;

    [SerializeField] private string itemName;
    [SerializeField] private Sprite itemSprite;

    private PickUpUI pickUpUI;

    

    void Start()
    {
        pickUpUI = FindObjectOfType<PickUpUI>();
    }

    public bool canInteract()
    {
        return !isPickedUp;
    }

    public void Interact()
    {
        CameraEffects cameraEffects = Camera.main.GetComponent<CameraEffects>();

        if (isPickedUp) return;

        if (pickUpUI != null)
        {
            cameraEffects.effectActive = false;

            pickUpUI.ShowPickUp(itemSprite, itemName);

            
        }       

        isPickedUp = true;
        gameObject.SetActive(false);

        
    }
}

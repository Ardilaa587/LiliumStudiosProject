using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUps : MonoBehaviour, InteractableI
{
    
    public bool isPickedUp = false;

    [SerializeField] private string itemName;
    [SerializeField] private Sprite itemSprite;
    [SerializeField] private Sprite secondSprite; 
    [SerializeField] private string secondText;  
    [SerializeField] private AudioClip pickUpSound;

    private PickUpUI pickUpUI;

    void Start()
    {
        pickUpUI = FindObjectOfType<PickUpUI>();

        if (RespawnManager.instance != null && RespawnManager.instance.effectRemovalItemCollected)
        {

            isPickedUp = true;

            gameObject.SetActive(false);
        }
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

            if (RespawnManager.instance != null)
            {
                RespawnManager.instance.CollectEffectRemovalItem();
            }

            pickUpUI.StartSequence(itemSprite, itemName, pickUpSound,
            secondSprite, secondText);

        }       

        isPickedUp = true;
        gameObject.SetActive(false);

        
    }
}

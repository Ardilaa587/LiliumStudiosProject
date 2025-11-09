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
    private Inventory playerInventory;

    private GrayScaleController grayscaleController;

    void Start()
    {
        pickUpUI = FindObjectOfType<PickUpUI>();
        grayscaleController = FindObjectOfType<GrayScaleController>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerInventory = player.GetComponent<Inventory>();
        }

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

    public void Interact(GameObject user)
    {
        

        if (isPickedUp) return;

        if (pickUpUI != null)
        {
            

            if (RespawnManager.instance != null)
            {
                RespawnManager.instance.CollectEffectRemovalItem();
            }

            pickUpUI.StartSequence(itemSprite, itemName, pickUpSound,
            secondSprite, secondText);

            if (grayscaleController != null)
            {
                grayscaleController.SetEffectActive(false); // Desactiva B/N (vuelve a color)
            }
        }       

        isPickedUp = true;
        gameObject.SetActive(false);

        if (playerInventory != null)
        {
            playerInventory.SetInventoryUIVisibility(true);
        }


    }
}

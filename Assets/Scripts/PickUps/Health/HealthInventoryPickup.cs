using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthInventoryPickup : MonoBehaviour
{
    [Header("Item Data")]
    [SerializeField] private string itemName = "Poción de Salud +2";
    [SerializeField] private string itemExplanation = "Restaura una pequeña cantidad de vida al consumirla desde el inventario.";
    [SerializeField] private Sprite itemSprite;
    [SerializeField] private ItemEffect itemEffect; 
    [SerializeField] private bool isConsumable = true;

    [Header("Pickup Components")]
    [SerializeField] private AudioSource pickUpSound;
    [SerializeField] private GameObject pickUpEffect;

    [Header("Inventory Setup")]
    [SerializeField] private GameObject itemButtonPrefab; 

    private Inventory inventory;
    [SerializeField] private ItemDisplayPanel displayPanel;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            inventory = player.GetComponent<Inventory>();
        }
        displayPanel = FindObjectOfType<ItemDisplayPanel>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && inventory != null)
        {
            if (pickUpSound != null)
            {
                AudioSource.PlayClipAtPoint(pickUpSound.clip, transform.position);
            }
            if (pickUpEffect != null)
            {
                Instantiate(pickUpEffect, transform.position, Quaternion.identity);
            }

            SaveToInventory();
        }
    }

    private void SaveToInventory()
    {
        for (int i = 0; i < inventory.slots.Length; i++)
        {
            if (inventory.isFull[i] == false)
            {
                inventory.isFull[i] = true;

                GameObject newButton = Instantiate(itemButtonPrefab, inventory.slots[i].transform, false);

                InventoryItem itemScript = newButton.GetComponent<InventoryItem>();

                if (itemScript != null)
                {
                    itemScript.itemName = itemName;
                    itemScript.itemExplanation = itemExplanation;
                    itemScript.itemSprite = itemSprite;

                    itemScript.itemEffect = itemEffect;
                    itemScript.isConsumable = isConsumable;
                    itemScript.slotIndex = i;
                }

                Destroy(gameObject);
                return; 
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthInventoryPickup : MonoBehaviour
{
    [Header("Item Data")]
    [SerializeField] private string itemName = "Poción de Salud +2";
    [SerializeField] private string itemExplanation = "Restaura una pequeña cantidad de vida al consumirla desde el inventario.";
    [SerializeField] private Sprite itemSprite;
    [SerializeField] private ItemEffect itemEffect; // El ScriptableObject de HealthEffect.cs
    [SerializeField] private bool isConsumable = true; // Siempre true para pociones

    // --- COMPONENTES DEL MUNDO/PICKUP ---
    [Header("Pickup Components")]
    [SerializeField] private AudioSource pickUpSound;
    [SerializeField] private GameObject pickUpEffect;

    // --- CONFIGURACIÓN DE INVENTARIO ---
    [Header("Inventory Setup")]
    [SerializeField] private GameObject itemButtonPrefab; // Prefab del botón de UI (con InventoryItem.cs)

    private Inventory inventory;
    [SerializeField] private ItemDisplayPanel displayPanel;

    private void Start()
    {
        // Obtener referencias esenciales
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
            // Ejecuta efectos visuales y de sonido (opcional)
            if (pickUpSound != null)
            {
                AudioSource.PlayClipAtPoint(pickUpSound.clip, transform.position);
            }
            if (pickUpEffect != null)
            {
                Instantiate(pickUpEffect, transform.position, Quaternion.identity);
            }

            // Inicia la lógica de guardado en inventario
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

                // 1. Instancia el botón en el slot
                GameObject newButton = Instantiate(itemButtonPrefab, inventory.slots[i].transform, false);

                // 2. Transfiere los datos y la lógica al script del botón
                InventoryItem itemScript = newButton.GetComponent<InventoryItem>();

                if (itemScript != null)
                {
                    // Transferencia de datos de UI
                    itemScript.itemName = itemName;
                    itemScript.itemExplanation = itemExplanation;
                    itemScript.itemSprite = itemSprite;

                    // Transferencia de lógica de uso
                    itemScript.itemEffect = itemEffect;
                    itemScript.isConsumable = isConsumable;
                    itemScript.slotIndex = i;
                }

                // 3. Destruye el objeto del mundo
                Destroy(gameObject);
                return; // Sale de la función después de guardar
            }
        }

        // Opcional: Si el inventario está lleno, aquí podrías mostrar un mensaje de error.
        Debug.Log("Inventario lleno. No se pudo recoger el ítem.");
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CarrotPickUp : MonoBehaviour
{
    [SerializeField] private AudioSource carrotSound;

    [SerializeField] private string carrotName;
    [SerializeField] private string carrotExplanation;
    [SerializeField] private Sprite carrotSprite;

    // --- COMPONENTES DEL INVENTARIO ---
    [Header("Configuración de Inventario")]
    [SerializeField] private GameObject itemButtonPrefab;

    private Inventory inventory;
    private ItemDisplayPanel displayPanel;

    [Header("Lógica de Uso")]
    [SerializeField] private ItemEffect missionEffect;

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
        if (collision.CompareTag("Player") && inventory != null && displayPanel != null)
        {
            // Verificar si hay espacio ANTES de iniciar la animación y el sonido
            bool isInventoryFull = true;
            for (int i = 0; i < inventory.slots.Length; i++)
            {
                if (inventory.isFull[i] == false)
                {
                    isInventoryFull = false;
                    break;
                }
            }

            if (!isInventoryFull)
            {
                carrotSound.Play();
                displayPanel.ShowPanel(carrotName, carrotExplanation, carrotSprite);
                StartCoroutine(WaitAndSaveToInventory());
            }
        }
    }

    private IEnumerator WaitAndSaveToInventory()
    {
        float totalDisplayTime = displayPanel.displayDuration * 2f + 3f;

        yield return new WaitForSeconds(totalDisplayTime);

        for (int i = 0; i < inventory.slots.Length; i++)
        {
            if (inventory.isFull[i] == false)
            {
                inventory.isFull[i] = true;

                GameObject newButton = Instantiate(itemButtonPrefab, inventory.slots[i].transform, false);

                InventoryItem itemScript = newButton.GetComponent<InventoryItem>();

                if (itemScript != null)
                {
                    itemScript.itemName = carrotName;
                    itemScript.itemExplanation = carrotExplanation;
                    itemScript.itemSprite = carrotSprite;
                    itemScript.slotIndex = i;
                    itemScript.itemEffect = missionEffect;
                    itemScript.isConsumable = false;
                }

                Destroy(gameObject);
                break;
            }
        }

        displayPanel.HidePanel();
    }
}

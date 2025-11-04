using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    [HideInInspector] public string itemName;
    [HideInInspector] public string itemExplanation;
    [HideInInspector] public Sprite itemSprite;

    // --- Lógica de Efecto (EL PUNTO CLAVE) ---
    [HideInInspector] public ItemEffect itemEffect;
    [HideInInspector] public bool isConsumable; // Indica si debe destruirse después de usarse

    // --- Lógica de Inventario ---
    [HideInInspector] public int slotIndex;
    private Inventory inventory;
    private ItemDisplayPanel displayPanel;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnItemClicked);
        displayPanel = FindObjectOfType<ItemDisplayPanel>();
        inventory = FindObjectOfType<Inventory>();
    }

    private void OnItemClicked()
    {
        // 🚨 1. LÓGICA DE CLIC: Decide si se usa o si solo muestra el panel

        // Si no es consumible O si ya está en modo victoria (Zanahoria)
        if (!isConsumable)
        {
            // Muestra el panel (Zanahoria)
            ShowItemInfo();
        }
        else
        {
            // Es consumible (Poción de Vida), se usa inmediatamente
            UseItemAndConsume();
        }
    }

    private void ShowItemInfo()
    {
        if (displayPanel != null)
        {
            displayPanel.ShowPanel(itemName, itemExplanation, itemSprite);
        }
    }

    private void UseItemAndConsume()
    {
        // 2. Ejecutar el efecto
        if (itemEffect != null)
        {
            // El usuario es el Jugador, que es el padre del Canvas de Inventario
            itemEffect.ExecuteEffect(inventory.gameObject);
        }

        // 3. Si es consumible, se elimina del inventario (NO se elimina si es la Zanahoria)
        if (isConsumable)
        {
            RemoveItemFromInventory();
        }
        // Nota: Si el efecto de la Zanahoria es exitoso, su efecto debería llamar a RemoveItemFromInventory()
        // Opcional: Modifica CarrotMissionEffect para que devuelva un bool.
    }

    public void RemoveItemFromInventory()
    {
        if (inventory != null)
        {
            inventory.isFull[slotIndex] = false;
        }
        Destroy(gameObject);
    }
}

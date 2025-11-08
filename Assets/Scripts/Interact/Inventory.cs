using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public bool[] isFull;
    public GameObject[] slots;

    public GameObject inventoryUI;

    void Start()
    {
        if (inventoryUI != null)
        {
            inventoryUI.SetActive(false);
        }
    }

    public void SetInventoryUIVisibility(bool isVisible)
    {
        if (inventoryUI != null)
        {
            inventoryUI.SetActive(isVisible);
        }
    }

    public InventoryItem selectedMissionItem = null;

    // Función llamada desde InventoryItem.cs
    public void SetSelectedMissionItem(InventoryItem item)
    {
        if (selectedMissionItem == item)
        {
            // Deseleccionar si se hace clic dos veces
            selectedMissionItem = null;
            // Ocultar cualquier indicador de "usar" en el mundo.
        }
        else
        {
            // Seleccionar el nuevo ítem
            selectedMissionItem = item;
            // Mostrar un indicador visual de que este ítem está listo para usarse.
        }
    }
}

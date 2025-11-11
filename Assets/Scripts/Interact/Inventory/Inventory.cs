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

    public void SetSelectedMissionItem(InventoryItem item)
    {
        if (selectedMissionItem == item)
        {
            selectedMissionItem = null;
        }
        else
        {
            selectedMissionItem = item;
        }
    }
}

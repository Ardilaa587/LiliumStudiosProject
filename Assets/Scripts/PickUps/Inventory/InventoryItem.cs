using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    [HideInInspector] public string itemName;
    [HideInInspector] public string itemExplanation;
    [HideInInspector] public Sprite itemSprite;

    [HideInInspector] public ItemEffect itemEffect;
    [HideInInspector] public bool isConsumable; 

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
        if (!isConsumable)
        {
            inventory.SetSelectedMissionItem(this);
        }
        else
        {
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
        if (itemEffect != null)
        {
            itemEffect.ExecuteEffect(inventory.gameObject);
        }
        if (isConsumable)
        {
            RemoveItemFromInventory();
        }
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

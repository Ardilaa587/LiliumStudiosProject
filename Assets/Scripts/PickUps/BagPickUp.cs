using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class BagPickUp : MonoBehaviour, InteractableI
{
    public bool isOpened { get; private set; }
   // public bool isInteracted;

    [SerializeField]private GameObject itemPrefab;

    void Start()
    { 
        isOpened = false;
        //isInteracted = false;

        if (RespawnManager.instance != null && RespawnManager.instance.itemAInteracted)
        {
            //isInteracted = true;
            isOpened = true;
        }
    }

    public void Interact()
    {
        if (!canInteract()) return;
        OpenBag();

        if(!isOpened)
        {
            if (RespawnManager.instance != null)
            {
                RespawnManager.instance.SetItemAInteracted();
            }
            isOpened = true;
        }

    }

    public bool canInteract()
    {
        return !isOpened;
        
    }

    private void OpenBag()
    {
        isOpened = true;

        if (itemPrefab)
        {
            GameObject droppedItem = Instantiate(itemPrefab, transform.position + Vector3.left * 2, Quaternion.identity);

        }
    }
}

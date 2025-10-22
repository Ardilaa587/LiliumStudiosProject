using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagPickUp : MonoBehaviour, InteractableI
{
    public bool isOpened { get; private set; }

    [SerializeField]private GameObject itemPrefab;


    public void Interact()
    {
        if (!canInteract()) return;
        OpenBag();
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

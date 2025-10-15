using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagPickUp : MonoBehaviour, InteractableI
{
    public bool isOpened { get; private set; }

    public GameObject itemPrefab;

    // Start is called before the first frame update
    void Start()
    {

    }


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
        SetOpened(true);

        if (itemPrefab)
        {
            GameObject droppedItem = Instantiate(itemPrefab, transform.position + Vector3.right * 2, Quaternion.identity);
            
        }
    }

    public void SetOpened(bool opened)
    {
        if (isOpened == opened)
        {
            gameObject.SetActive(false);
        }
    }
}

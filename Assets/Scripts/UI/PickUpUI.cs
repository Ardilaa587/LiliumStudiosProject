using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PickUpUI : MonoBehaviour
{
    [SerializeField] private GameObject pickUpPanel;
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private float displayTime = 3.5f;

    private Coroutine displayCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        pickUpPanel.SetActive(false);
    }

    public void ShowPickUp(Sprite itemSprite, string itemName)
    {
        if(displayCoroutine != null)
        {
            StopCoroutine(displayCoroutine);
        }

        itemImage.sprite = itemSprite;
        itemNameText.text = itemName;
        pickUpPanel.SetActive(true);
        displayCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayTime);
        pickUpPanel.SetActive(false);
    }

}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDisplayPanel : MonoBehaviour
{
    [SerializeField] private GameObject panelObject;
    [SerializeField] private Image displayImage;
    [SerializeField] private TMP_Text displayText;
    [SerializeField] private float typingDelay = 0.05f;
    [SerializeField] public float displayDuration = 3f; // 💡 Lo hacemos público para calcular el tiempo

    private void Start()
    {
        panelObject.SetActive(false);
    }

    public void ShowPanel(string itemName, string itemExplanation, Sprite itemSprite)
    {
        panelObject.SetActive(true);
        displayImage.sprite = itemSprite;

        StopAllCoroutines();
        StartCoroutine(TypeItemText(itemName, itemExplanation));
    }

    public void HidePanel()
    {
        panelObject.SetActive(false);
    }

    private IEnumerator TypeItemText(string itemName, string itemExplanation)
    {
        // ... (Tu lógica de tecleo de nombre) ...
        displayText.text = "";
        for (int i = 0; i < itemName.Length; i++)
        {
            displayText.text += itemName[i];
            yield return new WaitForSeconds(typingDelay);
        }

        yield return new WaitForSeconds(displayDuration);

        // ... (Tu lógica de tecleo de explicación) ...
        displayText.text = "";
        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < itemExplanation.Length; i++)
        {
            displayText.text += itemExplanation[i];
            yield return new WaitForSeconds(typingDelay);
        }

        yield return new WaitForSeconds(displayDuration);

        HidePanel();
    }
}

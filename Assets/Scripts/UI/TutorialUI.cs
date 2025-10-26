using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private Image image;
    [SerializeField] private Sprite tutorialSprites;
    [SerializeField] TMP_Text tutorialText;
    [SerializeField] private string tutorialMessages;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            tutorialPanel.SetActive(true);
            
            image.sprite = tutorialSprites;
            tutorialText.text = tutorialMessages;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            tutorialPanel.SetActive(false);
        }
    }
}

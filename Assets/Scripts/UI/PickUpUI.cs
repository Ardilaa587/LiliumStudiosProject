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
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private float displayTime = 3.5f;
    private Coroutine displayCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        pickUpPanel.SetActive(false);
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    public void StartSequence(Sprite firstSprite, string firstText, AudioClip pickUpSound,
            Sprite secondSprite, string secondText)
    {
        if (displayCoroutine != null)
        {
            StopCoroutine(displayCoroutine);
        }

        displayCoroutine = StartCoroutine(RunPickUpSequence(firstSprite, firstText, pickUpSound,
            secondSprite, secondText));
    }

    private IEnumerator RunPickUpSequence(Sprite firstSprite, string firstText, AudioClip pickUpSound,
        Sprite secondSprite, string secondText)
    {
        ;

        pickUpPanel.SetActive(true);

        itemImage.sprite = firstSprite;
        itemNameText.text = firstText;

        if (audioSource != null && pickUpSound != null)
        {
            audioSource.PlayOneShot(pickUpSound);
        }

        yield return new WaitForSeconds(displayTime);

        if (secondSprite != null)
        {
            itemImage.sprite = secondSprite;
        }
        itemNameText.text = secondText;

        yield return new WaitForSeconds(displayTime);

        pickUpPanel.SetActive(false);
        displayCoroutine = null;

    }
}

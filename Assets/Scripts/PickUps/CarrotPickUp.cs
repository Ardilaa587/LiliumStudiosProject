using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CarrotPickUp : MonoBehaviour
{
    [SerializeField] GameObject pickUpPanel;
    [SerializeField] private Image pickUpImage;
    [SerializeField] private TMP_Text carrotText;
    [SerializeField] private AudioSource carrotSound;

    [SerializeField] private float typingDelay = 0.05f;
    [SerializeField] private float displayDuration = 3f;

    [SerializeField] private string carrotName;
    [SerializeField] private string carrotExplanation;
    [SerializeField] private Sprite carrotSprite;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            carrotSound.Play();
            pickUpPanel.SetActive(true);

            pickUpImage.sprite = carrotSprite;
            
            StartCoroutine(TypeCarrotText());

        }
    }

    private IEnumerator TypeCarrotText()
    {
        

        carrotText.text = "";
        for( int i = 0; i < carrotName.Length; i++)
        {
            carrotText.text += carrotName[i];
            yield return new WaitForSeconds(typingDelay);
        }

        yield return new WaitForSeconds(displayDuration);

        carrotText.text = "";
        yield return new WaitForSeconds(0.2f);

        for( int i = 0; i < carrotExplanation.Length; i++)
        {
            carrotText.text += carrotExplanation[i];
            yield return new WaitForSeconds(typingDelay);
        }

        yield return new WaitForSeconds(displayDuration);

        Destroy(gameObject);
        pickUpPanel.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

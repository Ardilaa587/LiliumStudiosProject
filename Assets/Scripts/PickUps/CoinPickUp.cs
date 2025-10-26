using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickUp : MonoBehaviour
{
    [SerializeField] private int coinValue = 1;
    private CoinCounter coinCounter;
    [SerializeField] private AudioSource coinSound;

    private void Start()
    {
        coinCounter = FindObjectOfType<CoinCounter>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (coinSound != null)
            {
                AudioSource.PlayClipAtPoint(coinSound.clip, transform.position);
            }
            coinCounter.AddCoins(coinValue);
            Destroy(gameObject);
        }
    }


}

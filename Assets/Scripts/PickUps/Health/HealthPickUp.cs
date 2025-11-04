using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickUp : MonoBehaviour
{
    [SerializeField] private float healthAmount = 1f;
    [SerializeField] private AudioSource pickUpSound;
    [SerializeField] private GameObject pickUpEffect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null)
        {
            player.AddHealth(healthAmount);
            if (pickUpSound != null)
            {
                AudioSource.PlayClipAtPoint(pickUpSound.clip, transform.position);
            }
            if (pickUpEffect != null)
            {
                Instantiate(pickUpEffect, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }
}

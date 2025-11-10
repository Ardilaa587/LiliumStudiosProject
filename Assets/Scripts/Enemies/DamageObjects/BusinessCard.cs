using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
#endif
using UnityEngine;

public class BusinessCard : MonoBehaviour
{
    //public GameObject target;
    [SerializeField] public float speed;
    [SerializeField] private Rigidbody2D rb;
    private Vector2 initialDirection;
    [SerializeField] private float damage;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 0;

        Destroy(gameObject, 2f);
    }

    public void SetDirection(Vector2 direction)
    {
        initialDirection = direction.normalized;
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        if (rb != null)
        {
            rb.velocity = initialDirection * speed;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        PlayerController playerController = collision.GetComponent<PlayerController>();

        if (playerController != null)
        {
            playerController.TakeDamage(damage);
        }

        // Si queremos que la tarjeta desaparezca al tocar al jugador o cualquier cosa.
        // Podrías añadir una comprobación si quieres que solo se destruya al tocar al jugador.
        if (playerController != null)
        {
            Destroy(gameObject);
        }
        // Si quieres que se destruya al tocar cualquier cosa (pared, suelo, etc.):
        // Destroy(gameObject); 
    }
}

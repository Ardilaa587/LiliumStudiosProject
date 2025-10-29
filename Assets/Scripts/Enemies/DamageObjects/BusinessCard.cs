using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BusinessCard : MonoBehaviour
{
    //public GameObject target;
    [SerializeField] public float speed;
    [SerializeField] private Rigidbody2D rb;
    private Vector2 initialDirection;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 0;
        rb.velocity = initialDirection * speed;

        Destroy(gameObject, 5f);
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
        Destroy(gameObject);
    }
}

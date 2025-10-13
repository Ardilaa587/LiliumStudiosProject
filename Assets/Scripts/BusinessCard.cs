using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusinessCard : MonoBehaviour
{
    public GameObject target;
    public float speed;
    public Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        target = GameObject.FindGameObjectWithTag("Player");
        

        
        rb.gravityScale = 0;

        Destroy(gameObject, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            // Recalcular dirección cada frame
            Vector2 direction = (target.transform.position - transform.position).normalized;

            // Limitar el ángulo vertical para que no apunte demasiado arriba/abajo
            direction.y = Mathf.Clamp(direction.y, -0.3f, 0.3f);

            // Mover la tarjeta
            rb.velocity = direction * speed;
        }
    }
}

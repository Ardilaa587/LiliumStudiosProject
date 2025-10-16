using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniEnemiesController : MonoBehaviour
{
    public Transform[] enemyMovementPoints;
    [SerializeField] private Transform actualObjective;
    [SerializeField] private Rigidbody2D rb;

    public float enemySpeed;
    public float detectionRadius = 0.5f;

    Vector2 movement;

    public float enemyDamage;                  // Daño que causa al jugador.
    public float enemyHitForceX;               // Fuerza horizontal del golpe que recibe el jugador.
    public float enemyHitForceY;
    public float enemyHitTime;

    // Start is called before the first frame update
    void Start()
    {
        actualObjective = enemyMovementPoints[0];
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float distanceToObjective = Vector2.Distance(transform.position, actualObjective.position);

        if (distanceToObjective < detectionRadius)
        {
            if (actualObjective == enemyMovementPoints[0]) // Llegue al punto A
            {
                actualObjective = enemyMovementPoints[1];
                
            }
            else if (actualObjective == enemyMovementPoints[1]) // Lelgue al punto B
            {
                actualObjective = enemyMovementPoints[0];
                
            }
        }

        Vector2 direction = (actualObjective.position - transform.position).normalized;

        rb.MovePosition(rb.position + movement * enemySpeed * Time.fixedDeltaTime);
        if (direction.x != 0)
            transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Obtenemos el script del jugador (debe tener ClassPlayerMovement).
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();

            // Aplicamos daño al jugador.
            player.TakeDamage(enemyDamage);

            // Configuramos valores del golpe (knockback).
            player.hitTime = enemyHitTime;
            player.hitForceX = enemyHitForceX;
            player.hitForceY = enemyHitForceY;

            // Revisamos desde qué lado golpeó el enemigo al jugador.
            if (collision.transform.position.x <= transform.position.x)
            {
                // El jugador está a la izquierda del enemigo.
                player.hitFromRight = true;
            }
            else if (collision.transform.position.x > transform.position.x)
            {
                // El jugador está a la derecha del enemigo.
                player.hitFromRight = false;
            }
        }
    }
}

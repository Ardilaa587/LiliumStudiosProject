using Cinemachine;
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

    public float enemyDamage;
    public float enemyHitForceX;  
    public float enemyHitForceY;
    public float enemyHitTime;

    [SerializeField] private Animator miniEnemiesAnimator;

    // Start is called before the first frame update
    void Start()
    {
        miniEnemiesAnimator = GetComponent<Animator>();
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
                Flip();
            }
            else if (actualObjective == enemyMovementPoints[1]) // Llegue al punto B
            {
                actualObjective = enemyMovementPoints[0];
                Flip();
            }
        }

        Vector2 direction = (actualObjective.position - transform.position).normalized;

        int roundedDirection = Mathf.RoundToInt(direction.x);

        movement = new Vector2(roundedDirection, 0);

        miniEnemiesAnimator.SetFloat("Direction", roundedDirection);
        rb.MovePosition(rb.position + movement * enemySpeed * Time.fixedDeltaTime);

    }

    private void Flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
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
            miniEnemiesAnimator.SetTrigger("Attack");
        }
    }
}

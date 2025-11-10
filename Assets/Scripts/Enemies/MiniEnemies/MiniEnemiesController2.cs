using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniEnemiesController2 : MonoBehaviour
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

    // 🌟🌟 NUEVAS VARIABLES PARA EL ATAQUE DE TARJETA 🌟🌟
    [Header("Ataque de Tarjeta")]
    [SerializeField] private GameObject businessCardPrefab; // El prefab del BusinessCard
    [SerializeField] private Transform attackPoint;         // Punto desde donde se dispara la tarjeta (objeto hijo)
    [SerializeField] private float fireRate = 2f;           // Tiempo entre disparos
    private float nextFireTime;

    // Start is called before the first frame update
    void Start()
    {
        miniEnemiesAnimator = GetComponent<Animator>();
        actualObjective = enemyMovementPoints[0];
        rb = GetComponent<Rigidbody2D>();

        // Inicializa el tiempo para el primer disparo
        nextFireTime = Time.time;
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

        // 🌟🌟 LÓGICA DE DISPARO 🌟🌟
        // Comprueba si es hora de disparar
        if (Time.time >= nextFireTime)
        {
            ShootBusinessCard();
            // Calcula el tiempo del próximo disparo
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    // 🌟🌟 MÉTODO DE DISPARO 🌟🌟
    public void ShootBusinessCard()
    {
        if (businessCardPrefab == null || attackPoint == null)
        {
            Debug.LogError("BusinessCard Prefab o Attack Point no asignado en MiniEnemiesController.");
            return;
        }

        // Determinar la dirección basada en la escala (hacia donde mira el enemigo)
        // Si localScale.x > 0, dispara a la derecha. Si es < 0, dispara a la izquierda.
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        // 1. Instanciar la tarjeta.
        GameObject cardObject = Instantiate(businessCardPrefab, attackPoint.position, Quaternion.identity);

        // 2. Obtener el script BusinessCard y configurarlo.
        BusinessCard card = cardObject.GetComponent<BusinessCard>();

        if (card != null)
        {
            card.SetDirection(direction);
        }

        // Opcional: Puedes añadir aquí un Trigger para la animación de ataque a distancia.
        // miniEnemiesAnimator.SetTrigger("RangedAttack");
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

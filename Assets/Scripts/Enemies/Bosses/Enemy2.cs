using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Enemy2 : MonoBehaviour
{
    public Transform[] EnemyMovementPoints;
    [SerializeField] private Transform actualObjective;
    [SerializeField] private Rigidbody2D enemyRb;

    [SerializeField] private float speed;

    Vector2 movement;

    //Business Card Shoot variables
    [SerializeField] private GameObject businessCardPrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float shootInterval;
    public float shootTimer;

    private Transform playerTarget;
    private Rigidbody2D playerRb;

    [Header("Jump Stomp Logic")]
    [SerializeField] private int requiredJumpsToDefeat = 3; // Cuántos saltos se necesitan
    private int jumpStompCounter = 0; // Contador de saltos
    private bool isDefeated = false; // Estado de derrota

    // >>> REFERENCIA AL PANEL DE VICTORIA <<<
    [Header("Victory Panel")]
    [SerializeField] private GameObject victoryPanel; // El GameObject del Panel UI
    [SerializeField] private TMP_Text victoryText; // El componente de texto dentro del panel
    [SerializeField] private string victoryMessage;
    [SerializeField] private Image victoryImageComponent;
    [SerializeField] private Sprite victoryImage;

    // Start is called before the first frame update
    void Start()
    {
        actualObjective = EnemyMovementPoints[0];
        enemyRb = GetComponent<Rigidbody2D>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTarget = player.transform;
            playerRb = player.GetComponent<Rigidbody2D>(); // OBTENER EL RB DEL JUGADOR
        }

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isDefeated)
        {
            enemyRb.velocity = Vector2.zero; // Asegura que no se mueva
            return;
        }

        float distanceToObjective = Vector2.Distance(transform.position, actualObjective.position);

        if (distanceToObjective < 0.5f)
        {
            if (actualObjective == EnemyMovementPoints[0])
            {
                actualObjective = EnemyMovementPoints[1];
                
            }
            else if (actualObjective == EnemyMovementPoints[1])
            {
                actualObjective = EnemyMovementPoints[0];
            }
        }

        Vector2 direction = (actualObjective.position - transform.position).normalized;

        int roundedDirection = Mathf.RoundToInt(direction.x);

        movement = new Vector2(roundedDirection, 0);

        enemyRb.MovePosition(enemyRb.position + movement * speed * Time.fixedDeltaTime);

        shootTimer += Time.deltaTime;
        if (shootTimer >= shootInterval)
        {
            ShootCardAtTarget();
            shootTimer = 0f;
        }
    }

    private void ShootCardAtTarget()
    {
        Vector2 targetPosition = playerTarget.position;
        float projectileSpeed = businessCardPrefab.GetComponent<BusinessCard>().speed;

        Vector2 direction = (targetPosition - (Vector2)shootPoint.position).normalized;

        float maxAngleComponentY = 0.5f;
        direction.y = Mathf.Clamp(direction.y, -maxAngleComponentY, maxAngleComponentY);

        direction = direction.normalized;

        GameObject cardGO = Instantiate(businessCardPrefab, shootPoint.position, Quaternion.identity);

        BusinessCard cardScript = cardGO.GetComponent<BusinessCard>();
        if (cardScript != null)
        {
            cardScript.SetDirection(direction);
        }
    }

        private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDefeated) return;

        // 1. Verificar si es el jugador y si viene desde arriba
        if (collision.gameObject.CompareTag("Player"))
        {
            // Vector que va del centro del enemigo al centro del punto de contacto
            Vector2 contactNormal = collision.contacts[0].normal;

            // Si el vector normal apunta hacia arriba (significa que el jugador golpeó desde arriba)
            if (contactNormal.y < -0.9f) // El valor -1 indica golpe directo desde arriba
            {
                jumpStompCounter++;
                Debug.Log("¡Salto detectado! Contador: " + jumpStompCounter);

                // Rebotar al jugador
                if (playerRb != null)
                {
                    // Usa una fuerza o velocidad para rebotar al jugador
                    // Esto es solo un ejemplo, ajusta la fuerza
                    playerRb.velocity = new Vector2(playerRb.velocity.x, 0);
                    playerRb.AddForce(Vector2.up * 500f);
                }

                if (jumpStompCounter >= requiredJumpsToDefeat)
                {
                    DefeatEnemy();
                }
            }
        }
    }

    // Nuevo método para manejar la derrota del enemigo y el panel de victoria
    private void DefeatEnemy()
    {
        isDefeated = true;
        // Detener el movimiento inmediatamente
        if (enemyRb != null)
        {
            enemyRb.velocity = Vector2.zero;
            enemyRb.isKinematic = true; // Opcional: para que no le afecten más fuerzas
        }

        // Llamar al panel de victoria
        ShowVictoryPanel();

        // Opcional: Destruir el enemigo después de un tiempo o cambiar su sprite
        // Destroy(gameObject, 5f);
    }

    private void ShowVictoryPanel()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);

            // Establecer el texto y el mensaje de victoria
            if (victoryText != null)
            {
                victoryText.text = victoryMessage;
            }

            if (victoryImageComponent != null && victoryImage != null)
            {
                victoryImageComponent.sprite = victoryImage;
                
            }

            // Opcional: Mostrar un sprite de victoria en el panel si tienes un Image
            // Image victoryImage = victoryPanel.GetComponentInChildren<Image>(); 
            // if (victoryImage != null) { victoryImage.sprite = tuSpriteDeVictoria; }
        }
    }
}
   


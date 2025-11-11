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

    [SerializeField] private GameObject businessCardPrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float shootInterval;
    public float shootTimer;

    private Transform playerTarget;
    private Rigidbody2D playerRb;

    [Header("Jump Stomp Logic")]
    [SerializeField] private int requiredJumpsToDefeat = 3; 
    private int jumpStompCounter = 0; 
    private bool isDefeated = false; 

    [Header("Victory Panel")]
    [SerializeField] private GameObject victoryPanel; 
    [SerializeField] private TMP_Text victoryText; 
    [SerializeField] private string victoryMessage;
    [SerializeField] private Image victoryImageComponent;
    [SerializeField] private Sprite victoryImage;

    [SerializeField] private Animator boss2Animator;

    // Start is called before the first frame update
    void Start()
    {
        actualObjective = EnemyMovementPoints[0];
        enemyRb = GetComponent<Rigidbody2D>();

        boss2Animator = GetComponent<Animator>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTarget = player.transform;
            playerRb = player.GetComponent<Rigidbody2D>(); 
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
            enemyRb.velocity = Vector2.zero; 
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

        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 contactNormal = collision.contacts[0].normal;

            if (contactNormal.y < -0.9f) 
            {
                jumpStompCounter++;
                Debug.Log("¡Salto detectado! Contador: " + jumpStompCounter);

                boss2Animator.SetTrigger("Hit");

                if (playerRb != null)
                {
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

    private void DefeatEnemy()
    {
        isDefeated = true;

        if (enemyRb != null)
        {
            enemyRb.velocity = Vector2.zero;
            enemyRb.isKinematic = true; 
        }

        ShowVictoryPanel();
    }

    private void ShowVictoryPanel()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);

            if (victoryText != null)
            {
                victoryText.text = victoryMessage;
            }

            if (victoryImageComponent != null && victoryImage != null)
            {
                victoryImageComponent.sprite = victoryImage;
                
            }

        }
    }
}
   


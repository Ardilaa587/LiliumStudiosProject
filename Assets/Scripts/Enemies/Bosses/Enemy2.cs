using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    // Update is called once per frame
    void Update()
    {
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
   
}

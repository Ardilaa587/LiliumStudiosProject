using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2 : MonoBehaviour
{
    public Transform[] EnemyMovementPoints;
    [SerializeField] private Transform actualObjective;
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private float speed;

    Vector2 movement;

    //Business Card Shoot variables
    [SerializeField] private GameObject businessCardPrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float shootInterval;
    public float shootTimer;

    // Start is called before the first frame update
    void Start()
    {
        actualObjective = EnemyMovementPoints[0];
        rb = GetComponent<Rigidbody2D>();
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

        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);

        shootTimer += Time.deltaTime;
        if (shootTimer >= shootInterval)
        {
            Instantiate(businessCardPrefab, shootPoint.position, Quaternion.identity);
            shootTimer = 0f;
        }
    }

    
   
}

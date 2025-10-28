using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Boss1 : MonoBehaviour
{
    [SerializeField] private float detectionRange = 5f; 
    [SerializeField] private float moveSpeed = 3f;      

    [SerializeField] private float jumpAdvantageHeight = 0.5f;
    [SerializeField] private Transform playerTransform;

    private Rigidbody2D rb;
    private bool isPursuing = false;

    [SerializeField] private int bossDamage = 2;
    [SerializeField] private float bossHitTime;
    [SerializeField] private float bossHitForceY;
    [SerializeField] private float bossHitForceX;

    [Header("Configuración de Combate")]
    [SerializeField] private int maxHits = 3; // Total de golpes necesarios
    public int currentHits = 0;             // Contador actual
    private bool isDefeated = false;

    [Header("Escena de Victoria")]
    [SerializeField]
    private string nextSceneName = "Level2";

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (playerTransform == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
            }
        }
    }

    void Update()
    {
        if (playerTransform == null || isDefeated) return;

        float distanceToPlayer = Mathf.Abs(transform.position.x - playerTransform.position.x);

        if (distanceToPlayer <= detectionRange)
        {
            PursuePlayer();
        }
        else
        {
            StopMovement();
            isPursuing = false; 
        }
    }
    public void TakeHit()
    {
        if (isDefeated) return;

        currentHits++;
        Debug.Log("Jefe golpeado. Golpes: " + currentHits);

        if (currentHits >= maxHits)
        {
            DefeatBoss();
        }
    }

    private void DefeatBoss()
    {
        isDefeated = true;
        StopMovement();
        GetComponent<Collider2D>().enabled = false;

        StartCoroutine(LoadNextSceneAfterDelay(2f));
    }

    private IEnumerator LoadNextSceneAfterDelay(float delay)
    {
        // Pausa la ejecución por el tiempo especificado
        yield return new WaitForSeconds(delay);

        // Carga la escena, verificando que el nombre no esté vacío
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 contactNormal = collision.GetContact(0).normal;

            if (contactNormal.y > 0.8f)
            {
                return;
            }
            if (!isDefeated)
            {
                PlayerController player = collision.gameObject.GetComponent<PlayerController>();

                if(player != null)
                {
                    player.TakeDamage(bossDamage);

                    player.hitTime = bossHitTime;
                    player.hitForceX = bossHitForceX;
                    player.hitForceY = bossHitForceY;
                }
            }
        }
    }

    private void PursuePlayer()
    {
        if (isPursuing == false)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            isPursuing = true;
        }

        float verticalDifference = playerTransform.position.y - transform.position.y;

        if (verticalDifference > jumpAdvantageHeight)
        {
            StopMovement();
            return;
        }

        float targetX = playerTransform.position.x;
        float currentX = rb.position.x;

        float moveDirection = Mathf.Sign(targetX - currentX);

        rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);
    }

    private void StopMovement()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

}

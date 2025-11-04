using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [Header("Configuración de Docilidad")]
    [SerializeField] private BarBoss1 docilityBar;

    [Header("Configuración de Combate")]
    [SerializeField] private int maxHits = 3;
    public int currentHits = 0;
    private bool isFullyDocile = false;

    [Header("Escena de Victoria")]
    [SerializeField]
    private string nextSceneName = "Level2";

    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private float victoryDisplayTime = 10f;
    [SerializeField] private AudioClip victorySound;
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private Image victoryImageComponent;
    [SerializeField] private Sprite victorySprite;

    [SerializeField] private TMP_Text victoryTextComponent;
    [SerializeField] private string victoryMessage;


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

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (playerTransform == null || isFullyDocile)
        {
            StopMovement();
            return;
        }

        float distanceToPlayer = Mathf.Abs(transform.position.x - playerTransform.position.x);

        if (distanceToPlayer <= detectionRange)
        {
            PursuePlayer();
            if (docilityBar != null && currentHits > 0)
            {
                docilityBar.ShowBar();
            }
        }
        else
        {
            StopMovement();
            isPursuing = false;
            if (docilityBar != null)
            {
                docilityBar.HideBar();
            }
        }
    }

    public void RegisterHit()
    {
        if (isFullyDocile) return;

        currentHits++;

        if (docilityBar != null)
        {
            docilityBar.UpdateDocility(currentHits, maxHits);
        }

        if (currentHits >= maxHits)
        {
            ActivateDocileMode();
        }
    }

    private void ActivateDocileMode()
    {
        isFullyDocile = true;
        StopMovement();

        if (docilityBar != null)
        {
            docilityBar.UpdateDocility(maxHits, maxHits);
        }

        StartCoroutine(HandleVictorySequence());
    }

    private IEnumerator HandleVictorySequence()
    {
        float victoryDisplayDelay = 2f; // Puedes usar tu campo victoryDisplayTime si lo prefieres

        // Asegura que la barra de docilidad permanezca visible si estaba en uso
        if (docilityBar != null)
        {
            // Espera el tiempo de docilidad MÁXIMA que pusiste en DocilityBar
            float docileTime = docilityBar.displayTimeWhenMaxDocile;
            yield return new WaitForSeconds(docileTime);
        }

        // Lógica visual de Victoria (Panel, Sonido, Texto)
        if (victoryTextComponent != null)
        {
            victoryTextComponent.text = victoryMessage;
        }

        if (victoryImageComponent != null && victorySprite != null)
        {
            victoryImageComponent.sprite = victorySprite;
            victoryImageComponent.enabled = true;
        }

        if (victorySound != null && audioSource != null)
        {
            audioSource.PlayOneShot(victorySound);
        }

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }

        // Espera final antes de la transición
        yield return new WaitForSeconds(victoryDisplayDelay);

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

            if (contactNormal.y < -0.8f)
            {
                RegisterHit();
                return;
            }

            if (!isFullyDocile)
            {
                PlayerController player = collision.gameObject.GetComponent<PlayerController>();

                if (player != null)
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

    public void ActivateVictoryState()
    {
        // 💡 Lógica para asegurarse de que el jefe se detenga y muestre el panel de victoria
        isFullyDocile = true;
        StopMovement();

        // Si la barra de docilidad es visible, ocúltala o déjala verde.
        if (docilityBar != null)
        {
            docilityBar.UpdateDocility(maxHits, maxHits);
        }

        // Llama a la corrutina para iniciar la secuencia de victoria
        StartCoroutine(HandleVictorySequence());
    }

}

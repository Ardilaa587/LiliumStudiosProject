using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Boss1 : MonoBehaviour, InteractableI
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
    [SerializeField] public int maxHits = 3;
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

    private bool isFacingRight;

    [Header("Secuencia Final de Zanahoria")]
    [SerializeField] private Transform destinationPoint; 
    [SerializeField] private float travelSpeed = 2f;
    

    private bool isSequenceRunning = false;
    private Transform playerMountPoint;

    [Header("Punto de Montaje")]
    [SerializeField] private Transform bossMountPoint; 

    private Inventory playerInventory;
    private InteractionDetector playerDetector;

    [Header("Animación Boss1")]
    [SerializeField] private string hitBoolName = "IsHitting";
    [SerializeField] private Animator bossAnimator;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bossAnimator = GetComponent<Animator>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
            // Obtener referencias clave del jugador
            playerInventory = playerObject.GetComponent<Inventory>();
            playerDetector = playerObject.GetComponent<InteractionDetector>();
        }

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }
        

        SetBossAnimation(true);
    }

    private void SetBossAnimation(bool isHitting)
    {
        if (bossAnimator != null)
        {
            bossAnimator.SetBool(hitBoolName, isHitting);
        }
    }

    void Update()
    {
        if (playerTransform == null || isFullyDocile || isSequenceRunning)
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
        if (isFullyDocile || isSequenceRunning) return;

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

        SetBossAnimation(false);
        if (docilityBar != null)
        {
            docilityBar.UpdateDocility(maxHits, maxHits);
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
                SetBossAnimation(true);

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

        if (moveDirection > 0 && isFacingRight) Flip();
        else if (moveDirection < 0 && !isFacingRight) Flip();

        rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);
    }

    private void StopMovement()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    public bool canInteract()
    {
        return isFullyDocile;
    }

    public void Interact(GameObject user)
    {
        PlayerController playerController = user.GetComponent<PlayerController>();

        if (playerController == null)
        {
            playerController = user.GetComponentInParent<PlayerController>();
        }

        if (playerController != null && playerInventory != null && playerInventory.selectedMissionItem != null && isFullyDocile)
        {
            InventoryItem carrotItem = playerInventory.selectedMissionItem;

            playerController.DisableMovement(true);

            StartCarrotVictorySequence(playerController.transform); 

            carrotItem.RemoveItemFromInventory();
            playerInventory.SetSelectedMissionItem(null);

            playerController.DisableMovement(true);

            if (playerDetector != null)
            {
                playerDetector.InteractionIcon.SetActive(false);
            }

        }


    }

    public void StartCarrotVictorySequence(Transform playerTransformRef)
    {
        if (isSequenceRunning) return;

        isFullyDocile = true;
        isSequenceRunning = true;
        StopMovement();

        playerMountPoint = playerTransformRef;

        if (playerDetector != null && playerDetector.InteractionIcon != null)
        {
            playerDetector.InteractionIcon.SetActive(false);

            playerDetector.InteractionIcon.transform.SetParent(null);
        }

        if (bossMountPoint != null)
        {

            playerMountPoint.position = bossMountPoint.position;

            playerMountPoint.SetParent(bossMountPoint);
        }

        if (docilityBar != null) docilityBar.HideBar();

        StartCoroutine(ShowVictoryPanelAndPrepareMove());
    }

    private IEnumerator ShowVictoryPanelAndPrepareMove()
    {
        float panelDisplayDelay = 0.5f;

        yield return new WaitForSeconds(panelDisplayDelay);

        if (victoryTextComponent != null) victoryTextComponent.text = victoryMessage;

        if (victorySound != null && audioSource != null)
        {
            audioSource.PlayOneShot(victorySound);
        }

        if (victoryPanel != null) victoryPanel.SetActive(true);

        yield return new WaitForSeconds(victoryDisplayTime);

        if (victoryPanel != null) victoryPanel.SetActive(false);

        StartCoroutine(MoveBossToDestination());
    }

    private IEnumerator MoveBossToDestination()
    {

        if (isFacingRight)
        {
            Flip();
        }

        while (Vector2.Distance(transform.position, destinationPoint.position) > 0.1f)
        {
            Vector2 newPosition = Vector2.MoveTowards(transform.position, destinationPoint.position, travelSpeed * Time.deltaTime);
            rb.MovePosition(newPosition);
            yield return null;

            
        }

        StopMovement();

        if (playerMountPoint != null)
        {
            playerMountPoint.SetParent(null);
        }

        PlayerController playerController = playerMountPoint.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.DisableMovement(false); 
        }

        if (playerDetector != null && playerDetector.InteractionIcon != null)
        {
            playerDetector.InteractionIcon.transform.SetParent(playerMountPoint.transform);
        }

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

   
}

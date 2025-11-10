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
    private bool isFullyDocile = false; // El estado clave

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
    [SerializeField] private Transform destinationPoint; // Punto de destino del movimiento
    [SerializeField] private float travelSpeed = 2f;
    

    private bool isSequenceRunning = false;
    private Transform playerMountPoint;

    [Header("Punto de Montaje")]
    [SerializeField] private Transform bossMountPoint; // Empty Child para montar al jugador

    // Nueva Referencia para la Interacción
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
        // Detiene el movimiento normal si está dócil o en cinemática
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


    // --- Lógica de Combate y Docilidad ---

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

    // ... (Colisión, PursuePlayer, StopMovement y Flip) ...

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
        // Lógica de persecución. Revisa que el verticalDifference no detenga el movimiento.
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


    // --- IMPLEMENTACIÓN DE INTERACTABLEI ---

    public bool canInteract()
    {
        return isFullyDocile;
    }

    public void Interact(GameObject user)
    {
        // El Boss solo interactúa si el jugador tiene la zanahoria seleccionada
        PlayerController playerController = user.GetComponent<PlayerController>();

        if (playerController == null)
        {
            playerController = user.GetComponentInParent<PlayerController>();
        }

        if (playerController != null && playerInventory != null && playerInventory.selectedMissionItem != null && isFullyDocile)
        {
            // 1. Obtener referencia del ítem
            InventoryItem carrotItem = playerInventory.selectedMissionItem;

            // 2. Desactivar el control del jugador usando la referencia de la raíz
            playerController.DisableMovement(true);

            // 3. Iniciar la secuencia cinemática, pasando el TRANSFORM DE LA RAÍZ DEL JUGADOR
            // ----------------------------------------------------------------------
            StartCarrotVictorySequence(playerController.transform); // <-- ¡LA CLAVE!
                                                                    // ----------------------------------------------------------------------

            // 4. Consumir el ítem y deseleccionarlo (termina la misión)
            carrotItem.RemoveItemFromInventory();
            playerInventory.SetSelectedMissionItem(null);

            playerController.DisableMovement(true);

            // 5. Ocultar el ícono de interacción (sigue usando el playerDetector)
            if (playerDetector != null)
            {
                playerDetector.InteractionIcon.SetActive(false);
            }
            else if (isFullyDocile)
            {
                Debug.Log("Jefe: ¡Necesito que selecciones la Zanahoria en tu inventario primero!");
            }
        }


    }


    // --- SECUENCIA CINEMÁTICA DE ZANAHORIA (NO DESAPARECE AL INICIO) ---

    public void StartCarrotVictorySequence(Transform playerTransformRef)
    {
        if (isSequenceRunning) return;

        isFullyDocile = true;
        isSequenceRunning = true;
        StopMovement();

        // 1. Almacena la referencia del jugador
        playerMountPoint = playerTransformRef;

        if (playerDetector != null && playerDetector.InteractionIcon != null)
        {
            // Desactivar el ícono, si estaba activo (Interact ya lo desactiva, pero es seguro).
            playerDetector.InteractionIcon.SetActive(false);

            // Quitar el ícono de la jerarquía del jugador para que no se mueva con el jefe.
            // Lo dejamos flotando en el mundo (SetParent(null)).
            playerDetector.InteractionIcon.transform.SetParent(null);
        }

        // 2. Posicionar el jugador al punto de montaje
        if (bossMountPoint != null)
        {
            // Mueve al jugador al mount point
            playerMountPoint.position = bossMountPoint.position;
            // Lo hace hijo del mount point
            playerMountPoint.SetParent(bossMountPoint);
        }

        // ❌ ¡CORRECCIÓN CLAVE! EL JEFE NO SE DESACTIVA AQUÍ.

        if (docilityBar != null) docilityBar.HideBar();

        // Inicia la primera fase de la secuencia (Panel de Victoria)
        StartCoroutine(ShowVictoryPanelAndPrepareMove());
    }

    // Corrutina 1: Muestra Panel de Victoria
    private IEnumerator ShowVictoryPanelAndPrepareMove()
    {
        float panelDisplayDelay = 0.5f;

        yield return new WaitForSeconds(panelDisplayDelay);

        // Lógica visual de Victoria (Panel, Sonido, Texto, etc.)
        if (victoryTextComponent != null) victoryTextComponent.text = victoryMessage;

        if (victorySound != null && audioSource != null)
        {
            audioSource.PlayOneShot(victorySound);
        }

        if (victoryPanel != null) victoryPanel.SetActive(true);

        // Espera el tiempo del panel de victoria
        yield return new WaitForSeconds(victoryDisplayTime);

        if (victoryPanel != null) victoryPanel.SetActive(false);

        // Comienza el movimiento
        StartCoroutine(MoveBossToDestination());
    }

    // Corrutina 2: Mueve el Boss con el Jugador
    private IEnumerator MoveBossToDestination()
    {
        // ❌ CORRECCIÓN CLAVE: Eliminadas las líneas que reactivaban el jefe, ya que nunca se desactivó
        // ❌ gameObject.SetActive(true);
        // ❌ GetComponent<Collider2D>().enabled = true;

        if (isFacingRight)
        {
            Flip();
        }

        // Movimiento hacia el destino
        while (Vector2.Distance(transform.position, destinationPoint.position) > 0.1f)
        {
            Vector2 newPosition = Vector2.MoveTowards(transform.position, destinationPoint.position, travelSpeed * Time.deltaTime);
            rb.MovePosition(newPosition);
            yield return null;

            
        }

        // Boss llegó al destino
        StopMovement();

        // El jugador deja de ser hijo y se mantiene en su última posición
        if (playerMountPoint != null)
        {
            playerMountPoint.SetParent(null);
        }

        // Inicia el panel final
        PlayerController playerController = playerMountPoint.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.DisableMovement(false); // Reactivar movimiento
        }

        // Restaurar el ícono de interacción a la jerarquía del jugador
        if (playerDetector != null && playerDetector.InteractionIcon != null)
        {
            playerDetector.InteractionIcon.transform.SetParent(playerMountPoint.transform);
        }

        // Carga la siguiente escena inmediatamente
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

   
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class BossHands : MonoBehaviour
{
    [Header("Referencias de las manos")]
    public Transform leftHand;
    public Transform rightHand;
    public Transform leftRestPos;
    public Transform rightRestPos;

    [Header("Velocidad y ataques")]
    public float moveSpeed = 5f;
    public float attackDelay = 2f;
    public float minAttackDelay = 0.5f;
    public float speedUpRate = 0.1f;
    public int maxAttacks = 100;

    [Header("Velas")]
    public LayerMask candleLayer; // ahora usamos layer
    public List<CandleInteractable> candles = new List<CandleInteractable>();

    [Header("Jugador (para ignorar colisiones)")]
    public Collider playerCollider; // 🔹 <-- ESTA ERA LA VARIABLE FALTANTE

    [Header("Debug")]
    public bool startOnAwake = false;

    private int totalAttacks = 0;
    private bool isRunning = false;
    private List<CandleInteractable> candlesInUse = new List<CandleInteractable>();

    [Header("Animación")]
    public Animator bossAnimator; // 🆕 Referencia al Animator del jefe
    [SerializeField] private  string ANIM_IDLE = "Idle";
    [SerializeField] private  string ANIM_HIT = "Hit";
    [SerializeField] private string ANIM_EXPLOSION = "Explosion";

    [Header("UI de Derrota")]
    [SerializeField] private GameObject defeatPanel; // Panel que contendrá el texto y la imagen.
    [SerializeField] private TMP_Text defeatTextMessage; // Referencia al componente de texto
    [SerializeField] private Image defeatImageDisplay; // 🌟🌟 NUEVO: Referencia al componente Image 🌟🌟
    [SerializeField] private Sprite defeatSprite; // 🌟🌟 NUEVO: El sprite de la imagen de victoria 🌟🌟
    [SerializeField] private float textDisplayTime = 3f;

    [TextArea(3, 5)]
    [SerializeField] private string textPhase1 = "";
    [TextArea(3, 5)]
    [SerializeField] private string textPhase2 = "";

    void Start()
    {
        if (leftHand == null || rightHand == null)
        {
            Debug.LogError("⚠️ Las manos no están asignadas.", this);
            enabled = false;
            return;
        }

        if (leftRestPos == null || rightRestPos == null)
        {
            Debug.LogError("⚠️ Las posiciones de descanso no están asignadas.", this);
            enabled = false;
            return;
        }

        if (bossAnimator == null)
        {
            Debug.LogWarning("⚠️ El Animator del jefe no está asignado. Las animaciones no funcionarán.", this);
        }

        // Buscar velas automáticamente según layer
        BuscarVelasPorLayer();

        // Ignorar colisiones entre manos y jugador
        Collider leftCol = leftHand.GetComponent<Collider>();
        Collider rightCol = rightHand.GetComponent<Collider>();

        if (leftCol && rightCol)
            Physics.IgnoreCollision(leftCol, rightCol);

        if (playerCollider)
        {
            if (leftCol) Physics.IgnoreCollision(leftCol, playerCollider);
            if (rightCol) Physics.IgnoreCollision(rightCol, playerCollider);
        }

        // Ignorar colisiones con layer “CANDLE” (para que no se atasquen)
        int candleLayerIndex = Mathf.RoundToInt(Mathf.Log(candleLayer.value, 2));
        Physics.IgnoreLayerCollision(gameObject.layer, candleLayerIndex, true);

        SetAnimation(ANIM_IDLE);

        //if (startOnAwake)
            //StartCoroutine(BossRoutine());
    }

    void SetAnimation(string animName)
    {
        if (bossAnimator != null)
        {
            bossAnimator.Play(animName);
        }
    }

    public void ActivateBoss()
    {
        if (!isRunning)
        {
            Debug.Log("📢 BOSS ACTIVADO POR EL JUGADOR.");
            StartCoroutine(BossRoutine());
        }
    }

    IEnumerator BossRoutine()
    {
        SetAnimation(ANIM_IDLE);

        isRunning = true;
        while (isRunning)
        {
            if (AllCandlesOut())
            {
                Debug.Log("🔥 TODAS LAS VELAS APAGADAS. Boss derrotado!");
                SelfDestruct(true);
                yield break;
            }

            if (totalAttacks >= maxAttacks)
            {
                Debug.Log("💀 Boss se autodestruye: no logró apagar las 100 velas!");
                SelfDestruct(false);
                yield break;
            }

            // Ataque aleatorio
            if (Random.value > 0.5f)
                StartCoroutine(AttackRandomCandle(leftHand, leftRestPos));
            else
                StartCoroutine(AttackRandomCandle(rightHand, rightRestPos));

            totalAttacks++;
            attackDelay = Mathf.Max(minAttackDelay, attackDelay - speedUpRate);
            yield return new WaitForSeconds(attackDelay);
        }
    }

    IEnumerator AttackRandomCandle(Transform hand, Transform restPos)
    {
        CandleInteractable target = PickRandomLitCandleNotInUse();
        if (target == null) yield break;
        candlesInUse.Add(target);

        SetAnimation(ANIM_HIT);

        Vector3 targetPos = target.transform.position;
        float timer = 0f; // seguridad de tiempo

        // Desactivar física temporalmente
        Rigidbody rb = hand.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;
        Collider col = hand.GetComponent<Collider>();
        if (col) col.enabled = false;

        // Movimiento hacia la vela con timeout
        while (target != null && target.IsLit && Vector3.Distance(hand.position, targetPos) > 0.1f)
        {
            hand.position = Vector3.MoveTowards(hand.position, targetPos, moveSpeed * Time.deltaTime);
            timer += Time.deltaTime;

            // Si pasa demasiado tiempo, aborta
            if (timer > 5f)
            {
                Debug.LogWarning($"⏱️ {hand.name} no pudo llegar a la vela, aborta ataque.");
                break;
            }

            yield return null;
        }

        // Apagar la vela si llegó
        if (target != null && target.IsLit && Vector3.Distance(hand.position, targetPos) < 0.2f)
        {
            target.Extinguish();
            yield return new WaitForSeconds(0.3f);
        }

        // Volver a la posición de descanso
        while (Vector3.Distance(hand.position, restPos.position) > 0.05f)
        {
            hand.position = Vector3.MoveTowards(hand.position, restPos.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        SetAnimation(ANIM_IDLE);

        // Reactivar collider
        if (col) col.enabled = true;
        candlesInUse.Remove(target);
    }

    CandleInteractable PickRandomLitCandleNotInUse()
    {
        var lit = candles.Where(c => c != null && c.IsLit && !candlesInUse.Contains(c)).ToList();
        if (lit.Count == 0) return null;
        return lit[Random.Range(0, lit.Count)];
    }

    bool AllCandlesOut()
    {
        return candles.All(c => c == null || !c.IsLit);
    }

    void BuscarVelasPorLayer()
    {
        candles.Clear();
        var allCandles = FindObjectsOfType<CandleInteractable>();
        foreach (var c in allCandles)
        {
            if (((1 << c.gameObject.layer) & candleLayer) != 0)
                candles.Add(c);
        }
        Debug.Log($"🕯️ Se encontraron {candles.Count} velas en el layer CANDLE.");
    }

    void SelfDestruct(bool defeated)
    {
        isRunning = false;

        // 🆕 Si fue derrotado, reproducir animación de explosión
        if (defeated)
        {
            SetAnimation(ANIM_EXPLOSION);
            // Dejar la destrucción hasta que termine la animación, o con un retraso fijo
            Destroy(leftHand.gameObject, 0.1f);
            Destroy(rightHand.gameObject, 0.1f);
            Destroy(gameObject, 2f); // Destruir el jefe 2 segundos después (ajusta el tiempo a la duración de la animación)
        }
        else
        {
            // Autodestrucción normal si no fue por derrota
            if (leftHand) Destroy(leftHand.gameObject);
            if (rightHand) Destroy(rightHand.gameObject);
            Destroy(gameObject, 0.5f);
        }
    }

    IEnumerator DisplayDefeatMessage()
    {
        // 1. Activar el panel principal de derrota
        if (defeatPanel != null)
        {
            defeatPanel.SetActive(true);
        }

        // 🌟🌟 NUEVO: Configurar la imagen al inicio del mensaje de derrota 🌟🌟
        if (defeatImageDisplay != null && defeatSprite != null)
        {
            defeatImageDisplay.sprite = defeatSprite;
            defeatImageDisplay.enabled = true; // Asegurarse de que la imagen esté visible
        }

        // 2. Mostrar la Fase 1 del texto
        if (defeatTextMessage != null)
        {
            defeatTextMessage.text = textPhase1;
        }

        yield return new WaitForSeconds(textDisplayTime); // Esperar el tiempo configurado

        // 3. Mostrar la Fase 2 del texto
        if (defeatTextMessage != null)
        {
            defeatTextMessage.text = textPhase2;
        }

        yield return new WaitForSeconds(textDisplayTime); // Esperar el tiempo configurado nuevamente

        // 4. Opcional: Mostrar un mensaje final o limpiar la UI
        if (defeatTextMessage != null)
        {
            defeatTextMessage.text = "Pulsa A para continuar...";
        }
    }
    }



using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
    public List<Candle> candles = new List<Candle>();

    [Header("Jugador (para ignorar colisiones)")]
    public Collider playerCollider; // 🔹 <-- ESTA ERA LA VARIABLE FALTANTE

    [Header("Debug")]
    public bool startOnAwake = true;

    private int totalAttacks = 0;
    private bool isRunning = false;
    private List<Candle> candlesInUse = new List<Candle>();

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

        if (startOnAwake)
            StartCoroutine(BossRoutine());
    }

    IEnumerator BossRoutine()
    {
        isRunning = true;
        while (isRunning)
        {
            if (AllCandlesOut())
            {
                Debug.Log("🔥 TODAS LAS VELAS APAGADAS. Boss derrotado!");
                SelfDestruct();
                yield break;
            }

            if (totalAttacks >= maxAttacks)
            {
                Debug.Log("💀 Boss se autodestruye: no logró apagar las 100 velas!");
                SelfDestruct();
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
        Candle target = PickRandomLitCandleNotInUse();
        if (target == null) yield break;
        candlesInUse.Add(target);

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

        // Reactivar collider
        if (col) col.enabled = true;
        candlesInUse.Remove(target);
    }

    Candle PickRandomLitCandleNotInUse()
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
        var allCandles = FindObjectsOfType<Candle>();
        foreach (var c in allCandles)
        {
            if (((1 << c.gameObject.layer) & candleLayer) != 0)
                candles.Add(c);
        }
        Debug.Log($"🕯️ Se encontraron {candles.Count} velas en el layer CANDLE.");
    }

    void SelfDestruct()
    {
        isRunning = false;
        if (leftHand) Destroy(leftHand.gameObject);
        if (rightHand) Destroy(rightHand.gameObject);
        Destroy(gameObject, 0.5f);
    }
}


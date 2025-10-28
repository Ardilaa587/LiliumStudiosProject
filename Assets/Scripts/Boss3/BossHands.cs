using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BossHands : MonoBehaviour
{
    [Header("Referencias de las manos")]
    public Transform leftHand;      // mano izquierda
    public Transform rightHand;     // mano derecha
    public Transform leftRestPos;   // posición inicial o “reposo”
    public Transform rightRestPos;

    [Header("Velocidad y ataques")]
    public float moveSpeed = 5f;           // velocidad de movimiento
    public float attackDelay = 2f;         // tiempo entre ataques
    public float minAttackDelay = 0.5f;    // límite inferior
    public float speedUpRate = 0.1f;       // cuánto reduce el delay por ataque
    public int maxAttacks = 100;           // si llega aquí sin apagar todas las velas, se autodestruye

    [Header("Velas")]
    public List<Candle> candles = new List<Candle>();

    [Header("Debug")]
    public bool startOnAwake = true;

    private int totalAttacks = 0;
    private bool isRunning = false;

    void Start()
    {
        if (startOnAwake)
            StartCoroutine(BossRoutine());
    }

    IEnumerator BossRoutine()
    {
        isRunning = true;
        while (isRunning)
        {
            // Verificar si todas están apagadas
            if (AllCandlesOut())
            {
                Debug.Log("🔥 TODAS LAS VELAS APAGADAS. Boss derrotado!");
                SelfDestruct();
                yield break;
            }

            // verificar si alcanzó su límite
            if (totalAttacks >= maxAttacks)
            {
                Debug.Log("💀 Boss se autodestruye: no logró apagar las 100 velas!");
                SelfDestruct();
                yield break;
            }

            // manos atacan de forma alternada o simultánea
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
        Candle target = PickRandomLitCandle();
        if (target == null) yield break;

        Vector3 startPos = hand.position;
        Vector3 targetPos = target.transform.position;

        // mover hacia la vela
        while (Vector3.Distance(hand.position, targetPos) > 0.1f)
        {
            hand.position = Vector3.MoveTowards(hand.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // “ataque”: apaga la vela
        target.Extinguish();
        yield return new WaitForSeconds(0.3f);

        // volver a su posición de descanso
        while (Vector3.Distance(hand.position, restPos.position) > 0.05f)
        {
            hand.position = Vector3.MoveTowards(hand.position, restPos.position, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    Candle PickRandomLitCandle()
    {
        var lit = candles.Where(c => c != null && c.IsLit).ToList();
        if (lit.Count == 0) return null;
        return lit[Random.Range(0, lit.Count)];
    }

    bool AllCandlesOut()
    {
        return candles.All(c => c == null || !c.IsLit);
    }

    void SelfDestruct()
    {
        isRunning = false;

        // Destruye las manos
        if (leftHand != null) Destroy(leftHand.gameObject);
        if (rightHand != null) Destroy(rightHand.gameObject);

        // Opcional: destruir el objeto del boss
        Destroy(gameObject, 0.5f);
    }

#if UNITY_EDITOR
    [ContextMenu("Buscar velas en escena")]
    void BuscarVelas()
    {
        candles = FindObjectsOfType<Candle>().ToList();
        Debug.Log($"Se encontraron {candles.Count} velas.");
    }
#endif
}


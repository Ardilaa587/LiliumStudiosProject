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
    public LayerMask candleLayer; 
    public List<CandleInteractable> candles = new List<CandleInteractable>();

    [Header("Jugador (para ignorar colisiones)")]
    public Collider playerCollider; 

    [Header("Debug")]
    public bool startOnAwake = false;

    private int totalAttacks = 0;
    private bool isRunning = false;
    private List<CandleInteractable> candlesInUse = new List<CandleInteractable>();

    [Header("Animación")]
    public Animator bossAnimator; 
    [SerializeField] private  string ANIM_IDLE = "Idle";
    [SerializeField] private  string ANIM_HIT = "Hit";
    [SerializeField] private string ANIM_EXPLOSION = "Explosion";

    [Header("UI de Derrota")]
    [SerializeField] private GameObject defeatPanel; 
    [SerializeField] private TMP_Text defeatTextMessage; 
    [SerializeField] private Image defeatImageDisplay; 
    [SerializeField] private Sprite defeatSprite; 
    [SerializeField] private float textDisplayTime = 3f;

    [TextArea(3, 5)]
    [SerializeField] private string textPhase1 = "";
    [TextArea(3, 5)]
    [SerializeField] private string textPhase2 = "";

    void Start()
    {
        if (leftHand == null || rightHand == null)
        {
            enabled = false;
            return;
        }

        if (leftRestPos == null || rightRestPos == null)
        {
            enabled = false;
            return;
        }

        BuscarVelasPorLayer();

        Collider leftCol = leftHand.GetComponent<Collider>();
        Collider rightCol = rightHand.GetComponent<Collider>();

        if (leftCol && rightCol)
            Physics.IgnoreCollision(leftCol, rightCol);

        if (playerCollider)
        {
            if (leftCol) Physics.IgnoreCollision(leftCol, playerCollider);
            if (rightCol) Physics.IgnoreCollision(rightCol, playerCollider);
        }

        int candleLayerIndex = Mathf.RoundToInt(Mathf.Log(candleLayer.value, 2));
        Physics.IgnoreLayerCollision(gameObject.layer, candleLayerIndex, true);

        SetAnimation(ANIM_IDLE);
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
                SelfDestruct(true);
                yield break;
            }

            if (totalAttacks >= maxAttacks)
            {
                SelfDestruct(false);
                yield break;
            }

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
        float timer = 0f; 

        Rigidbody rb = hand.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;
        Collider col = hand.GetComponent<Collider>();
        if (col) col.enabled = false;

        while (target != null && target.IsLit && Vector3.Distance(hand.position, targetPos) > 0.1f)
        {
            hand.position = Vector3.MoveTowards(hand.position, targetPos, moveSpeed * Time.deltaTime);
            timer += Time.deltaTime;

            yield return null;
        }

        if (target != null && target.IsLit && Vector3.Distance(hand.position, targetPos) < 0.2f)
        {
            target.Extinguish();
            yield return new WaitForSeconds(0.3f);
        }

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

    }

    void SelfDestruct(bool defeated)
    {
        isRunning = false;

        if (defeated)
        {
            SetAnimation(ANIM_EXPLOSION);

            Destroy(leftHand.gameObject, 0.1f);
            Destroy(rightHand.gameObject, 0.1f);
            Destroy(gameObject, 2f);

            DisplayDefeatMessage();
        }
        else
        {

            if (leftHand) Destroy(leftHand.gameObject);
            if (rightHand) Destroy(rightHand.gameObject);
            Destroy(gameObject, 0.5f);
        }
    }

    IEnumerator DisplayDefeatMessage()
    {
        if (defeatPanel != null)
        {
            defeatPanel.SetActive(true);
        }

        if (defeatImageDisplay != null && defeatSprite != null)
        {
            defeatImageDisplay.sprite = defeatSprite;
            defeatImageDisplay.enabled = true; 
        }

        if (defeatTextMessage != null)
        {
            defeatTextMessage.text = textPhase1;
        }

        yield return new WaitForSeconds(textDisplayTime); 

        if (defeatTextMessage != null)
        {
            defeatTextMessage.text = textPhase2;
        }

        yield return new WaitForSeconds(textDisplayTime); 

        if (defeatTextMessage != null)
        {
            defeatTextMessage.text = "Pulsa A para continuar...";
        }
    }
    }



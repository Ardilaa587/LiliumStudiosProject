using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossController : MonoBehaviour
{
    [Header("Hands")]
    public HandController leftHand;
    public HandController rightHand;

    [Header("Targeting")]
    public float targetScanInterval = 0.5f; // con qué frecuencia busca velas
    private float scanTimer = 0f;

    [Header("Difficulty / Speed curve")]
    public float initialSpeedMultiplier = 1f;
    public float maxSpeedMultiplier = 3f;
    public float timeToMax = 120f; // segundos hasta que se alcanza max
    public AnimationCurve speedCurve = AnimationCurve.EaseInOut(0,1,1,3); // opcional

    private float elapsed = 0f;

    void Start()
    {
        ApplySpeed( initialSpeedMultiplier );
    }

    void Update()
    {
        elapsed += Time.deltaTime;
        UpdateSpeedOverTime();

        scanTimer += Time.deltaTime;
        if (scanTimer >= targetScanInterval)
        {
            scanTimer = 0f;
            TryAssignTargets();
        }
    }

    void UpdateSpeedOverTime()
    {
        float t = Mathf.Clamp01(elapsed / timeToMax);
        float multiplier;
        if (speedCurve != null && speedCurve.length > 0)
            multiplier = speedCurve.Evaluate(t);
        else
            multiplier = Mathf.Lerp(initialSpeedMultiplier, maxSpeedMultiplier, t);

        ApplySpeed(multiplier);
    }

    void ApplySpeed(float multiplier)
    {
        if (leftHand != null) leftHand.SetSpeedMultiplier(multiplier);
        if (rightHand != null) rightHand.SetSpeedMultiplier(multiplier);
    }

    void TryAssignTargets()
    {
        // Buscar todas las velas en escena (puedes mejorar con manager)
        Candle[] candles = GameObject.FindObjectsOfType<Candle>();
        if (candles == null || candles.Length == 0) return;

        // Asignar a cada mano la vela más cercana que no esté siendo atacada
        AssignClosestToHand(leftHand, candles);
        AssignClosestToHand(rightHand, candles);
    }

    void AssignClosestToHand(HandController hand, Candle[] candles)
    {
        if (hand == null || hand.IsBusy()) return;

        Candle best = null;
        float bestDistance = float.MaxValue;

        foreach (var c in candles)
        {
            if (!c.isLit) continue; // no atacar velas ya apagadas
            // opcional: ignorar si otra mano ya asignó esta vela (podrías trackear)
            float d = Vector2.Distance(hand.transform.position, c.transform.position);
            if (d < bestDistance)
            {
                bestDistance = d;
                best = c;
            }
        }

        if (best != null)
        {
            hand.AssignTarget(best);
        }
    }
}

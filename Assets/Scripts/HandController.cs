using UnityEngine;
using System.Collections;

public class HandController : MonoBehaviour
{
    [Header("Movement")]
    public float baseSpeed = 2f;
    [HideInInspector] public float currentSpeed = 2f;
    public float reachRange = 0.5f; // distancia para empezar a soplar

    [Header("Extinguish")]
    public float extinguishInterval = 0.1f; // con qué frecuencia aplica daño a la vela (segundos)
    public float extinguishDuration = 2f;   // tiempo mínimo que intenta soplar en una vela (puede romperse si el objetivo se apaga)
    public Animator animator; // asigna Animator para cambiar estados

    private Candle targetCandle;
    private bool isSoplando = false;
    private float soploTimer = 0f;
    private float extinguishTimer = 0f;

    void Start()
    {
        currentSpeed = baseSpeed;
    }

    void Update()
    {
        if (targetCandle != null)
        {
            MoveTowardsTarget();
        }
        else
        {
            // idle: puedes añadir movimiento suave o una posición home
        }

        if (isSoplando)
        {
            extinguishTimer += Time.deltaTime;
            soploTimer += Time.deltaTime;
            if (soploTimer >= extinguishInterval)
            {
                // aplicar extinción a la vela
                targetCandle.ApplyExtinguish(soploTimer);
                soploTimer = 0f;
            }

            // si la vela ya está apagada, terminar
            if (!targetCandle.isLit || extinguishTimer >= extinguishDuration)
            {
                StopSoplar();
            }
        }
    }

    void MoveTowardsTarget()
    {
        Vector3 targetPos = targetCandle.transform.position;
        // Mano se posiciona ligeramente por encima/lateral según diseño
        Vector3 desired = new Vector3(targetPos.x, targetPos.y + 0.5f, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, desired, currentSpeed * Time.deltaTime);

        float dist = Vector3.Distance(transform.position, desired);
        if (dist <= reachRange && !isSoplando)
        {
            StartSoplar();
        }
    }

    public void AssignTarget(Candle candle)
    {
        targetCandle = candle;
        // activar animación de movimiento si corresponde
        if (animator != null) animator.SetBool("isMoving", true);
    }

    void StartSoplar()
    {
        if (targetCandle == null) return;
        isSoplando = true;
        soploTimer = 0f;
        extinguishTimer = 0f;
        if (animator != null)
        {
            animator.SetBool("isMoving", false);
            animator.SetTrigger("Soplar"); // anima soplo
        }
    }

    void StopSoplar()
    {
        isSoplando = false;
        targetCandle = null;
        if (animator != null) animator.SetBool("isMoving", false);
    }

    public bool IsBusy()
    {
        return targetCandle != null || isSoplando;
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        currentSpeed = baseSpeed * multiplier;
    }
}


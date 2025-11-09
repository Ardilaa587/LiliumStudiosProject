using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CandleInteractable : MonoBehaviour, InteractableI
{
    [Header("Configuración de la vela")]
    [SerializeField]
    private bool isLit = false;       // Estado actual
    public ParticleSystem flameEffect;       // Partícula de fuego
    public Light2D candleLight;             // Luz opcional (si usas 3D)
    public AudioSource igniteSound;         // Sonido al encender

    [Tooltip("Evita encenderla si el boss está activo, por ejemplo.")]
    public bool canBeInteracted = true;

    // --- Control de Estado y Lógica ---

    void Update()
    {
        // 1. Detecta si la Light2D está encendida para actualizar isLit (LA LÓGICA DE DETECCIÓN SE MANTIENE)
        if (candleLight != null)
        {
            isLit = candleLight.enabled;
        }
    }

    // Implementación de la interfaz
    public void Interact(GameObject user)
    {
        if (!canBeInteracted) return;

        // 🚨 CAMBIO CLAVE AQUÍ: Solo permite LightUp() si está apagada (!isLit).
        if (isLit)
        {
            // Si ya está encendida, solo se informa y se detiene la interacción.
            Debug.Log($"{name}: 🕯️ La vela ya está encendida. No se puede interactuar.");
            return;
        }
        else
        {
            // Si está apagada, la encendemos.
            LightUp();
        }
    }

    public bool canInteract()
    {
        // 2. Controla si la interacción es posible: Solo si está apagada (!isLit)
        return canBeInteracted && !isLit;
    }

    // Encender la vela
    void LightUp()
    {
        if (flameEffect != null)
        {
            flameEffect.Play();
        }

        if (candleLight != null)
        {
            candleLight.enabled = true; // Activa la luz, lo que actualiza isLit en Update
        }

        if (igniteSound != null)
        {
            igniteSound.Play();
        }

        Debug.Log($"{name}: 🔥 Vela encendida por el jugador.");
    }

    // Apagar la vela (usado por otros scripts, pero no por la interacción del jugador)
    public void Extinguish()
    {
        if (flameEffect != null)
        {
            flameEffect.Stop();
        }

        if (candleLight != null)
        {
            candleLight.enabled = false; // Desactiva la luz, lo que actualiza isLit en Update
        }

        Debug.Log($"{name}: 💨 Vela apagada.");
    }

    // Acceso desde otros scripts
    public bool IsLit => isLit;
}


using UnityEngine;

public class CandleInteractable : MonoBehaviour, InteractableI
{
    [Header("Configuración de la vela")]
    public bool isLit = false;                // Estado actual
    public ParticleSystem flameEffect;        // Partícula de fuego
    public Light candleLight;                 // Luz opcional (si usas 3D)
    public AudioSource igniteSound;           // Sonido al encender

    [Tooltip("Evita encenderla si el boss está activo, por ejemplo.")]
    public bool canBeInteracted = true;

    // Implementación de la interfaz
    public void Interact()
    {
        if (!canBeInteracted) return;
        if (!isLit)
        {
            LightUp();
        }
        else
        {
            Debug.Log($"{name}: ya está encendida.");
        }
    }

    public bool canInteract()
    {
        return canBeInteracted && !isLit;
    }

    // Encender la vela
    void LightUp()
    {
        isLit = true;
        if (flameEffect != null)
        {
            flameEffect.Play();
        }

        if (candleLight != null)
        {
            candleLight.enabled = true;
        }

        if (igniteSound != null)
        {
            igniteSound.Play();
        }

        Debug.Log($"{name}: 🔥 Vela encendida por el jugador.");
    }

    // Apagar la vela (si el boss la apaga, por ejemplo)
    public void Extinguish()
    {
        isLit = false;

        if (flameEffect != null)
        {
            flameEffect.Stop();
        }

        if (candleLight != null)
        {
            candleLight.enabled = false;
        }

        Debug.Log($"{name}: 💨 Vela apagada.");
    }

    // Acceso desde otros scripts (como el boss)
    public bool IsLit => isLit;
}


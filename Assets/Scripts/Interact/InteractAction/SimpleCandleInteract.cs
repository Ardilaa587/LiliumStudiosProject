using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SimpleCandleInteract : MonoBehaviour
{
    [SerializeField] private Light2D candleLight;
    [SerializeField] private ParticleSystem flameEffect;
    [SerializeField] private AudioSource igniteSound;

    // Estado interno: controlamos la luz directamente, no es necesario en el Inspector.
    private bool isLit = false;

    void Awake()
    {
        // 1. Aseguramos que los componentes están apagados al inicio.
        if (candleLight != null)
        {
            candleLight.enabled = false;
        }
        if (flameEffect != null)
        {
            flameEffect.Stop();
        }
    }

    // --- Implementación de la Interfaz InteractableI ---

    public bool canInteract()
    {
        // Solo es interactuable si NO está encendida.
        return !isLit;
    }

    public void Interact(GameObject user)
    {
        // Si ya está encendida, ignoramos la interacción.
        if (isLit) return;

        // 2. Si llegamos aquí, está apagada y la encendemos.
        LightUp();
    }

    // --- Lógica de Encendido ---

    void LightUp()
    {
        isLit = true;

        if (flameEffect != null)
        {
            flameEffect.Play();
        }

        if (candleLight != null)
        {
            candleLight.enabled = true; // Enciende la luz
        }

        if (igniteSound != null)
        {
            // Nota: Se recomienda PlayOneShot() para evitar cortar sonidos largos.
            igniteSound.PlayOneShot(igniteSound.clip);
        }

        Debug.Log($"{name}: 🔥 Vela encendida.");

        // Opcional: Deshabilitar el script después de encender para ahorrar rendimiento.
        // enabled = false;
    }
}

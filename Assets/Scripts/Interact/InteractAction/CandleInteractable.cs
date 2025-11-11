using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CandleInteractable : MonoBehaviour, InteractableI
{
    [Header("Configuración de la vela")]
    [SerializeField]
    private bool isLit = false; 
    public ParticleSystem flameEffect; 
    public Light2D candleLight; 
    public AudioSource igniteSound;

    public bool canBeInteracted = true;

    void Update()
    {
        if (candleLight != null)
        {
            isLit = candleLight.enabled;
        }
    }

    public void Interact(GameObject user)
    {
        if (!canBeInteracted) return;

        if (isLit)
        {
            return;
        }
        else
        {
            LightUp();
        }
    }

    public bool canInteract()
    {
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
            candleLight.enabled = true; 
        }

        if (igniteSound != null)
        {
            igniteSound.Play();
        }

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

    }

    // Acceso desde otros scripts
    public bool IsLit => isLit;
}


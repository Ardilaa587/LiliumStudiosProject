using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SimpleCandleInteract : MonoBehaviour
{
    [SerializeField] private Light2D candleLight;
    [SerializeField] private ParticleSystem flameEffect;
    [SerializeField] private AudioSource igniteSound;

    private bool isLit = false;

    void Awake()
    {
        if (candleLight != null)
        {
            candleLight.enabled = false;
        }
        if (flameEffect != null)
        {
            flameEffect.Stop();
        }
    }

    public bool canInteract()
    {
        return !isLit;
    }

    public void Interact(GameObject user)
    {
        if (isLit) return;

        LightUp();
    }

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
            igniteSound.PlayOneShot(igniteSound.clip);
        }
    }
}

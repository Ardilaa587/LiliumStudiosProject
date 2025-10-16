using UnityEngine;
using System.Collections;

public class Candle : MonoBehaviour
{
    [Header("Visuals")]
    public SpriteRenderer flameRenderer;     // asigna el sprite de la llama
    public Transform flameTransform;
    public ParticleSystem smokeParticles;

    [Header("Candle state")]
    public bool isLit = true;
    [Range(0f,1f)] public float intensity = 1f; // 1 = full, 0 = off

    [Header("Extinguish settings")]
    public float extinguishSpeed = 0.5f; // cuánto reduce la intensidad por segundo cuando soplan
    public float relightSpeed = 1f;      // cuánto aumenta la intensidad por segundo cuando el player enciende
    public AudioClip extinguishSound;
    public AudioClip relightSound;
    private AudioSource audioSource;

    void Awake()
    {
        if (flameRenderer == null) Debug.LogWarning("Asigna flameRenderer en Candle");
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        UpdateVisual();
    }

    void UpdateVisual()
    {
        // Simple: ajusta alfa o escala de la llama según intensidad
        Color c = flameRenderer.color;
        c.a = Mathf.Clamp01(intensity);
        flameRenderer.color = c;

        // opcional: variar scale de la llama
        flameTransform.localScale = Vector3.one * (0.5f + intensity * 0.75f);

        // partículas de humo aparecen cuando intensidad < 0.3
        if (smokeParticles)
        {
            var em = smokeParticles.emission;
            em.rateOverTime = (intensity < 0.25f) ? 15f : 0f;
        }

        isLit = intensity > 0.05f;
    }

    // Called by a Hand when it's "soplando"
    public void ApplyExtinguish(float delta)
    {
        if (intensity <= 0f) return;
        intensity -= extinguishSpeed * delta;
        intensity = Mathf.Clamp01(intensity);
        if (intensity <= 0.01f) OnExtinguished();
        UpdateVisual();
    }

    public void ApplyRelight(float delta)
    {
        intensity += relightSpeed * delta;
        intensity = Mathf.Clamp01(intensity);
        UpdateVisual();
    }

    public void IgniteInstant()
    {
        intensity = 1f;
        UpdateVisual();
        PlaySound(relightSound);
    }

    private void OnExtinguished()
    {
        intensity = 0f;
        UpdateVisual();
        PlaySound(extinguishSound);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null) return;
        audioSource.PlayOneShot(clip);
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    // El nombre del parámetro de volumen dentro del Audio Mixer (ej: "MasterVolume")
    [SerializeField] private string volumeParameter = "MasterVolume";

    private Slider volumeSlider;

    private void Awake()
    {
        volumeSlider = GetComponent<Slider>();
    }

    void Start()
    {
        // 2. Cargar el valor guardado (si existe) o establecer un valor predeterminado (1.0)
        float savedVolume = PlayerPrefs.GetFloat("GlobalVolume", 1.0f);
        volumeSlider.value = savedVolume;

        // Aplicar el volumen al inicio
        SetVolume(savedVolume);
    }

    /// <summary>
    /// Llamada por el evento OnValueChanged del Slider.
    /// </summary>
    public void SetVolume(float volume)
    {
        // El AudioMixer usa escala logarítmica (dB) para el volumen.
        // Convertimos el valor lineal (0 a 1) del slider a dB.

        // Fórmulas de conversión:
        // Si volume > 0.0001, lo convertimos a dB.
        // Si volume es ~0, lo establecemos en -80 dB (silencio).

        if (volume > 0.0001f)
        {
            float dB = 20f * Mathf.Log10(volume);
            audioMixer.SetFloat(volumeParameter, dB);
        }
        else
        {
            // Valor de silencio absoluto
            audioMixer.SetFloat(volumeParameter, -80f);
        }

        // 3. Guardar el valor para la próxima sesión
        PlayerPrefs.SetFloat("GlobalVolume", volume);
    }
}

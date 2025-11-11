using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private string volumeParameter = "MasterVolume";

    private Slider volumeSlider;

    private void Awake()
    {
        volumeSlider = GetComponent<Slider>();
    }

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("GlobalVolume", 1.0f);
        volumeSlider.value = savedVolume;

        SetVolume(savedVolume);
    }

    /// <summary>
    /// Llamada por el evento OnValueChanged del Slider.
    /// </summary>
    public void SetVolume(float volume)
    {

        if (volume > 0.0001f)
        {
            float dB = 20f * Mathf.Log10(volume);
            audioMixer.SetFloat(volumeParameter, dB);
        }
        else
        {
            audioMixer.SetFloat(volumeParameter, -80f);
        }

        PlayerPrefs.SetFloat("GlobalVolume", volume);
    }
}

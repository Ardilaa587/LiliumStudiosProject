using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GrayScaleController : MonoBehaviour
{
    private Volume globalVolume;
    private ColorAdjustments colorAdjustments;

    [Header("Control de Efecto")]
    public bool effectActive = true;

    [Header("Ajustes de Transición")]
    public float transitionDuration = 0.5f; 

    void Start()
    {
        globalVolume = GetComponent<Volume>();

        if (!globalVolume.profile.TryGet(out colorAdjustments))
        {
            colorAdjustments = globalVolume.profile.Add<ColorAdjustments>(true);
        }

        colorAdjustments.saturation.value = 0f;

        SetInitialState();
    }

    private void SetInitialState()
    {
        if (RespawnManager.instance != null)
        {
            effectActive = RespawnManager.instance.isCameraEffectActive;
        }

        colorAdjustments.saturation.value = effectActive ? -100f : 0f;

        globalVolume.weight = 1f;
    }

    public void SetEffectActive(bool enable)
    {
        effectActive = enable;

        if (!enable)
        {
            StartCoroutine(TransitionSaturation(0f)); 
        }
        else
        {
            StartCoroutine(TransitionSaturation(-100f)); 
        }
    }

    private IEnumerator TransitionSaturation(float targetSaturation)
    {
        float startSaturation = colorAdjustments.saturation.value;
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            colorAdjustments.saturation.value = Mathf.Lerp(startSaturation, targetSaturation, elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        colorAdjustments.saturation.value = targetSaturation;
    } 
}

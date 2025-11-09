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
    // Simula la variable 'effectActive' de tu script antiguo
    public bool effectActive = true;

    [Header("Ajustes de Transición")]
    public float transitionDuration = 0.5f; // Duración de la transición

    // Necesitas el RespawnManager para inicializar el estado
    // [SerializeField] private RespawnManager respawnManager; // Asegúrate de asignar esto

    void Start()
    {
        globalVolume = GetComponent<Volume>();

        // Intenta obtener la configuración de Ajustes de Color. 
        // Si no existe en el perfil, lo añade y lo activa.
        if (!globalVolume.profile.TryGet(out colorAdjustments))
        {
            colorAdjustments = globalVolume.profile.Add<ColorAdjustments>(true);
        }

        // 1. Inicializa el estado de saturación (color normal)
        colorAdjustments.saturation.value = 0f;

        // 2. Sobrescribe y configura el estado inicial del efecto (B/N)
        SetInitialState();
    }

    private void SetInitialState()
    {
        // Lógica de RespawnManager original
        if (RespawnManager.instance != null)
        {
            effectActive = RespawnManager.instance.isCameraEffectActive;
        }

        // Aplica el estado inicial (blanco y negro si está activo)
        // No usamos transición para el inicio, lo ponemos directo.
        colorAdjustments.saturation.value = effectActive ? -100f : 0f;

        // El peso del Volume debe estar en 1 para que funcione
        globalVolume.weight = 1f;
    }

    // Método llamado por PickUps para desactivar el B/N
    public void SetEffectActive(bool enable)
    {
        effectActive = enable;

        // Si desactivamos (el objeto se recogió), iniciamos la transición a color.
        if (!enable)
        {
            StartCoroutine(TransitionSaturation(0f)); // 0f es color normal
        }
        else
        {
            // Si activamos (ej. en un punto de guardado), iniciamos la transición a B/N.
            StartCoroutine(TransitionSaturation(-100f)); // -100f es blanco y negro
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
        // Asegura el valor final
        colorAdjustments.saturation.value = targetSaturation;
    } 
}

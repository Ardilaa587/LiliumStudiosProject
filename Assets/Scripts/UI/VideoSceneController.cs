using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoSceneController : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;

    private void Start()
    {
        // 1. Asignar el listener para el evento de fin de video
        if (videoPlayer != null)
        {
            // Suscribe la función OnVideoFinished al evento loopPointReached
            videoPlayer.loopPointReached += OnVideoFinished;

            // Aseguramos que el video comience a reproducirse al inicio
            videoPlayer.Play();
        }
        else
        {
            Debug.LogError("FATAL: El VideoPlayer no está asignado. Cargando Nivel1 inmediatamente.");
            // Si el video no se encuentra, cargamos el nivel por seguridad
            SceneManager.LoadScene("Nivel1");
        }
    }

    // Esta función se llama automáticamente cuando el video termina
    private void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("Video terminado. Cargando Nivel 1.");

        // Desuscribir para evitar errores si el script se mantiene
        vp.loopPointReached -= OnVideoFinished;

        // 🌟 Redirige al nivel 1
        SceneManager.LoadScene("Nivel1");
    }
}

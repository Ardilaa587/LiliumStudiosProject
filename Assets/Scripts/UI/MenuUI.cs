using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    [Header("Menu Principal")]
    [SerializeField] public GameObject mainMenuPanel; // **NUEVO: Renombrado/Añadido**

    [Header("Animación de Intro")]
    [SerializeField] private Animator introAnimator; // **NUEVO: El Animator del libro**
    [SerializeField] private GameObject introPanel;
    [SerializeField] private AudioClip bookOpenSound;

    [SerializeField] public GameObject settingsPanel;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buttonClickSound;
    
    [Header("Punto de Reinicio Fijo")]
    public Transform fixedStartPosition;

    void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void Start()
    {
        // 🌟 LÓGICA DE INICIO:
        // 1. Aseguramos que el menú principal está oculto al inicio.
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
        }

        // 2. Si hay una animación de introducción, la mostramos.
        // Asegúrate de que el estado inicial del Animator es la animación de "abrir libro".
        if (introPanel != null)
        {
            introPanel.SetActive(true);
        }

        if (audioSource != null && bookOpenSound != null)
        {
            audioSource.PlayOneShot(bookOpenSound);
        }
    }


    public void OnBookAnimationEnd()
    {
        Debug.Log("Animación del libro terminada. Mostrando menú principal.");

        if (introAnimator != null)
        {
            introAnimator.enabled = false;
            // Opcional: Si el Animator está en el introPanel, podrías usar:
            // introPanel.GetComponent<Animator>().enabled = false;
        }

        // 3. Activar el panel del menú principal (los botones).
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }
    }

    private void PlaySound()
    {
        if (audioSource != null)
        {
            // Si tenemos un clip específico para el clic, lo asignamos y reproducimos.
            if (buttonClickSound != null)
            {
                audioSource.PlayOneShot(buttonClickSound);
            }
            // Si no, reproducimos el clip que esté asignado por defecto al AudioSource.
            else if (audioSource.clip != null)
            {
                audioSource.Play();
            }
        }
    }

    private IEnumerator PlaySoundAndLoadScene(string sceneName)
    {
        float delay = 0f;

        if (audioSource != null)
        {
            // 1. Asignar y reproducir el sonido
            if (buttonClickSound != null)
            {
                audioSource.PlayOneShot(buttonClickSound);
                delay = buttonClickSound.length; // Usar la duración del clip
            }
            else if (audioSource.clip != null)
            {
                audioSource.Play();
                delay = audioSource.clip.length; // Usar la duración del clip por defecto
            }
        }

        // 2. Esperar la duración del sonido para que no se corte
        // Solo esperamos si hay un sonido y su duración es mayor a 0
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }

        // 3. Cambiar de escena
        SceneManager.LoadScene(sceneName);
    }
    public void OnStart()
    {
        StartCoroutine(PlaySoundAndLoadScene("Nivel1"));
    }

    public void Level2()
    {
        StartCoroutine(PlaySoundAndLoadScene("Level2"));
    }

    public void Level3()
    {
        StartCoroutine(PlaySoundAndLoadScene("Level3"));
    }
 
    public void OnSettings()
    {
        PlaySound();
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    public void OnSettingsBack()
    {
        PlaySound();
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    public void OnMenu()
    {
        StartCoroutine(PlaySoundAndLoadScene("Menu"));
    }

    public void OnReStart()
    {
        PlaySound();

        // 2. Encontrar al Jugador
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        // 3. Verificar referencias
        if (RespawnManager.instance != null && playerObject != null && fixedStartPosition != null)
        {
            // A. SOBREESCRIBIR la última posición de respawn con la posición del objeto fijo.
            // Convertimos la posición de Transform (Vector3) a Vector2 para tu RespawnManager 2D.
            Vector2 fixedPos = fixedStartPosition.position;

            RespawnManager.instance.SetCheckpoint(fixedPos);

            // B. Mover al jugador usando la función del manager
            // Usamos SoftRespawn si tenemos el PlayerController, ya que también cura y limpia la velocidad.
            PlayerController playerController = playerObject.GetComponent<PlayerController>();

            if (playerController != null)
            {
                RespawnManager.instance.SoftRespawn(playerController);
            }
            else
            {
                RespawnManager.instance.RespawnPlayer(playerObject);
            }
        }
     }
}

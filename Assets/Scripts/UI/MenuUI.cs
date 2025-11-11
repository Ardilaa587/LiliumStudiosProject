using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    [Header("Menu Principal")]
    [SerializeField] public GameObject mainMenuPanel; 

    [Header("Animación de Intro")]
    [SerializeField] private Animator introAnimator; 
    [SerializeField] private GameObject introPanel;
    [SerializeField] private AudioClip bookOpenSound;

    [SerializeField] public GameObject settingsPanel;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buttonClickSound;

    [Header("Punto de Reinicio Fijo")]
    public Transform fixedStartPosition;

    [SerializeField] private GameOverUI gameOverUI;
    [Header("Posiciones de Inicio por Nivel")]
    public Transform startPosNivel1;
    public Transform startPosNivel2;
    public Transform startPosNivel3;
    void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void Start()
    {
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
        }

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
        }

        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }
    }

    private void PlaySound()
    {
        if (audioSource != null)
        {
            if (buttonClickSound != null)
            {
                audioSource.PlayOneShot(buttonClickSound);
            }

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
            if (buttonClickSound != null)
            {
                audioSource.PlayOneShot(buttonClickSound);
                delay = buttonClickSound.length; 
            }
            else if (audioSource.clip != null)
            {
                audioSource.Play();
                delay = audioSource.clip.length; 
        }
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }

        // 3. Cambiar de escena
        SceneManager.LoadScene(sceneName);
        
    }
    public void OnStart()
    {
       
        StartCoroutine(PlaySoundAndLoadScene("Video"));
    }

    public void Level1()
    {
        if (RespawnManager.instance != null && startPosNivel1 != null)
        {
            RespawnManager.instance.SetCheckpoint(startPosNivel1.position);
        }

        StartCoroutine(PlaySoundAndLoadScene("Nivel1"));
    }

    public void Level2()
    {
        if (RespawnManager.instance != null && startPosNivel2 != null)
        {
            RespawnManager.instance.SetCheckpoint(startPosNivel2.position);
        }

        StartCoroutine(PlaySoundAndLoadScene("Level2"));
    }

    public void Level3()
    {
        if (RespawnManager.instance != null && startPosNivel3 != null)
        {
            RespawnManager.instance.SetCheckpoint(startPosNivel3.position);
        }

        // 2. Cargar la escena
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

    // Dentro de MenuUI.cs

    public void OnReStart()
    {
        PlaySound();

        Time.timeScale = 1f;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null && fixedStartPosition != null)
        {
            Vector2 fixedPos = fixedStartPosition.position;
            PlayerController playerController = playerObject.GetComponent<PlayerController>();

            if (playerController != null)
            {
                playerController.SoftRespawn(fixedPos);
            }
        }

        if (gameOverUI != null)
        {
            gameOverUI.gameObject.SetActive(false);
        }
    }

    public void OnExit()
    {
        Application.Quit();
    }
}

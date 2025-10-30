using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public PlayerController playerController;
    public AudioSource musicSourceToStop;

    private void OnEnable()
    {
        if(musicSourceToStop != null)
        {
            musicSourceToStop.Pause();
        }
    }

    private void OnDisable()
    {
        if (musicSourceToStop != null)
        {
            musicSourceToStop.UnPause();
        }
    }

    public void Retry()
    {
        Time.timeScale = 1f;

        if(RespawnManager.instance != null && playerController != null)
        {
            RespawnManager.instance.SoftRespawn(playerController);
            gameObject.SetActive(false);
        }
    }

    public void Quit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
        Destroy(RespawnManager.instance.gameObject);
    }
}

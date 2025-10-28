using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public void Retry()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
        Destroy(RespawnManager.instance.gameObject);
    }
}

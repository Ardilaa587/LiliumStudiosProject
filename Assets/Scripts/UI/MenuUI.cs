using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    [SerializeField] public GameObject settingsPanel;

    public void OnStart()
    {
        SceneManager.LoadScene("Nivel1");
    }

    public void OnSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    public void OnSettingsBack()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    public void Level2()
    {
        SceneManager.LoadScene("Level2");
    }
    public void Level3()
    {
        SceneManager.LoadScene("Level3");
    }
}

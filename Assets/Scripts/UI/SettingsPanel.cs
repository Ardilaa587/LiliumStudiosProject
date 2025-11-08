using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    

    public void ShowSettingsPanel()
    {
        settingsPanel.SetActive(true);
    }

}

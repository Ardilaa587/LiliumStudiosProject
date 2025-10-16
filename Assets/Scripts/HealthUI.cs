using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private Image[] hearts;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;
    [SerializeField] private GameObject diePanel;

    // Update is called once per frame
    void Update()
    {
        if (player != null && hearts.Length > 0)
            UpdateHearts();
    }

    public void UpdateHearts()
    {


        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] == null) continue;

            if (i < player.health)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }

            if (player.health <= 0)
            {
                ShowDiePanel();
            }

        }

    }

    public void ShowDiePanel()
    {
        if (diePanel != null)
        {
            diePanel.SetActive(true);
            Time.timeScale = 0f; 
        }
    }
}

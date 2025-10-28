using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinCounter : MonoBehaviour
{
    [SerializeField] TMP_Text coinText;

    private int currentCoinsCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        UpdateCoinUI();
    }


    public void AddCoins(int amount)
    {
        currentCoinsCount += amount;
        UpdateCoinUI();
    }

    // Método privado para actualizar el texto en la UI
    private void UpdateCoinUI()
    {
        // El formato de la moneda se actualiza aquí (ej. "x 5")
        coinText.text = "x " + currentCoinsCount.ToString();
    }
}

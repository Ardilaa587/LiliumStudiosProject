using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarBoss1 : MonoBehaviour
{
    [Header("Componentes UI")]
    [SerializeField] private Slider slider;
    [SerializeField] private Image fillImage;

    [Header("Configuración de Color")]
    [SerializeField] private Color hostileColor = Color.red; // Color de inicio (Maldad)
    [SerializeField] private Color docileColor = Color.green; // Color final (Docilidad)

    [Header("Configuración de Visualización")]
    [SerializeField] private float displayTimeAfterHit = 2f; // Tiempo visible después de un golpe
    [SerializeField] public float displayTimeWhenMaxDocile = 5f; // Tiempo visible cuando está completamente dócil

    private Coroutine hideCoroutine;

    private void Start()
    {
        if (slider == null)
        {
            slider = GetComponent<Slider>();
        }
        // Ocultar al inicio
        gameObject.SetActive(false);

        // Inicializar el color en Hostile/Maldad (Rojo)
        fillImage.color = hostileColor;
    }

    // Método llamado por Boss1 para actualizar el valor
    public void UpdateDocility(int currentHits, int maxHits)
    {
        // Calcular el valor: 0 (Maldad/Hostil) a 1 (Docilidad/Dócil)
        float docilityValue = (float)currentHits / maxHits;
        slider.value = docilityValue;

        // Mezclar el color de rojo a verde (Hostile a Docile)
        fillImage.color = Color.Lerp(hostileColor, docileColor, docilityValue);

        // Mostrar la barra
        gameObject.SetActive(true);

        
    }
    public void HideBar()
    {
        gameObject.SetActive(false);
    }

    //  Nuevo método para que el jefe la muestre si está en combate
    public void ShowBar()
    {
        gameObject.SetActive(true);
    }




}

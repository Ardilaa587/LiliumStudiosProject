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
    [SerializeField] private Color hostileColor = Color.red; 
    [SerializeField] private Color docileColor = Color.green; 

    [Header("Configuración de Visualización")]
    [SerializeField] private float displayTimeAfterHit = 2f; 
    [SerializeField] public float displayTimeWhenMaxDocile = 5f; 

    private Coroutine hideCoroutine;

    private void Start()
    {
        if (slider == null)
        {
            slider = GetComponent<Slider>();
        }
        gameObject.SetActive(false);

        fillImage.color = hostileColor;
    }

    public void UpdateDocility(int currentHits, int maxHits)
    {

        float docilityValue = (float)currentHits / maxHits;
        slider.value = docilityValue;

        fillImage.color = Color.Lerp(hostileColor, docileColor, docilityValue);

        gameObject.SetActive(true);

        
    }
    public void HideBar()
    {
        gameObject.SetActive(false);
    }

    public void ShowBar()
    {
        gameObject.SetActive(true);
    }




}

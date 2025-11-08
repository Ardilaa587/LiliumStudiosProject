using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSwitcher : MonoBehaviour
{
    [Header("Objetos de Fondo")]
    public GameObject backgroundA;
    public GameObject backgroundB;

    void Start()
    {

        backgroundA.SetActive(true);
        backgroundB.SetActive(false);
    }

    public void SwitchToBackgroundB()
    {
        backgroundA.SetActive(false);
        backgroundB.SetActive(true);
        Debug.Log("Cambiando a Fondo B por Trigger.");
    }

    public void SwitchToBackgroundA()
    {
        backgroundA.SetActive(true);
        backgroundB.SetActive(false);
        Debug.Log("Cambiando a Fondo A por Trigger.");
    }

}

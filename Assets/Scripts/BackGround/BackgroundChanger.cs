using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundChanger : MonoBehaviour
{
    public BackgroundSwitcher backgroundManager;
    public string playerTag = "Player";
    public bool activatesBackgroundB = true;

    private void OnTriggerEnter2D(Collider2D other) // Usa OnTriggerEnter si es 3D
    {
        // 1. Verificar que el objeto que entró sea el jugador
        if (other.CompareTag(playerTag) && backgroundManager != null)
        {
            if (activatesBackgroundB)
            {
                // El jugador entró en la zona, activamos el Fondo B
                backgroundManager.SwitchToBackgroundB();
            }
            else
            {
                // El jugador entró en la zona, activamos el Fondo A (si esta zona fuera para volver)
                backgroundManager.SwitchToBackgroundA();
            }
        }
    }

}

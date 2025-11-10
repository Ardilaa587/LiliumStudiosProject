using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3Activator : MonoBehaviour
{
    [SerializeField] private BossHands bossController;

    // Opcional: Para asegurar que solo se active una vez
    private bool hasBeenActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        // 1. Asegúrate de que el jugador tiene el tag "Player"
        if (other.CompareTag("Player") && !hasBeenActivated)
        {
            // 2. Verifica que tenemos la referencia al jefe
            if (bossController != null)
            {
                bossController.ActivateBoss();
                hasBeenActivated = true;

                // Opcional: Desactiva o destruye este objeto Activator para ahorrar recursos
                // gameObject.SetActive(false); 
            }
            else
            {
                Debug.LogError("BossController no asignado al BossActivator.", this);
            }
        }
    }

    // Si tu juego es 2D (que es probable por los colliders 2D):
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasBeenActivated)
        {
            if (bossController != null)
            {
                bossController.ActivateBoss();
                hasBeenActivated = true;
                // Opcional: Desactiva el collider para que no se re-active (si es 2D)
                // GetComponent<Collider2D>().enabled = false;
            }
            else
            {
                Debug.LogError("BossController no asignado al BossActivator.", this);
            }
        }
    }
}

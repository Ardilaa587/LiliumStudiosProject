using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item Effects/Boss Victory")]
public class BossVictoryEffect : ItemEffect
{
    public override void ExecuteEffect(GameObject user)
    {
        // 1. Encontrar el Boss en la escena
        Boss1 boss = FindObjectOfType<Boss1>();

        // 2. Opcional: Desactivar el control del jugador inmediatamente
        PlayerController playerController = user.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.DisableMovement(true);
        }

        if (boss != null && boss.currentHits >= boss.maxHits)
        {
            // 3. Ejecuta la función de la secuencia final en el Boss
            boss.StartCarrotVictorySequence(user.transform);
        }
        else
        {
            // Debug o feedback si el jefe no está listo o no se encuentra
            Debug.Log("El Boss no está en estado dócil (derrotado) para recibir la zanahoria.");

            // Re-habilitar el movimiento si la secuencia no se inicia
            if (playerController != null)
            {
                playerController.DisableMovement(false);
            }
        }
    }

}

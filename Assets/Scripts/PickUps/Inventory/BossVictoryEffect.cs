using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item Effects/Boss Victory")]
public class BossVictoryEffect : ItemEffect
{
    public override void ExecuteEffect(GameObject user)
    {
        Boss1 boss = FindObjectOfType<Boss1>();

        PlayerController playerController = user.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.DisableMovement(true);
        }

        if (boss != null && boss.currentHits >= boss.maxHits)
        {
            boss.StartCarrotVictorySequence(user.transform);
        }
        else
        {
            if (playerController != null)
            {
                playerController.DisableMovement(false);
            }
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventario/Efecto Misión Zanahoria")]
public class CarrotMissionEffect : ItemEffect
{
    public override void ExecuteEffect(GameObject user)
    {
        Boss1 boss1 = FindObjectOfType<Boss1>();

        if(boss1 == null )
        {
            float distanceToBoss= Vector3.Distance(user.transform.position, boss1.transform.position);

            if( distanceToBoss <5f)
            {
                boss1.ActivateVictoryState();
            }
        }
    }
}

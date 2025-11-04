using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventario/Efecto Curación")]
public class HealthEffect : ItemEffect
{
    [SerializeField] private float healthAmount = 2f;

    public override void ExecuteEffect(GameObject user)
    {
        PlayerController player = user.GetComponent<PlayerController>();
        if (player != null)
        {
            player.AddHealth(healthAmount);
            Debug.Log($"Curado {healthAmount} de vida desde el inventario.");
        }
    }
}

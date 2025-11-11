using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3Activator : MonoBehaviour
{
    [SerializeField] private BossHands bossController;

    private bool hasBeenActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasBeenActivated)
        {
            if (bossController != null)
            {
                bossController.ActivateBoss();
                hasBeenActivated = true;

            }
            else
            {
                Debug.LogError("BossController no asignado al BossActivator.", this);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasBeenActivated)
        {
            if (bossController != null)
            {
                bossController.ActivateBoss();
                hasBeenActivated = true;
            }
            else
            {
                Debug.LogError("BossController no asignado al BossActivator.", this);
            }
        }
    }
}

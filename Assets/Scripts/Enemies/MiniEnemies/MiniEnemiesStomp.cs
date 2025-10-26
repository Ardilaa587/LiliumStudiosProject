using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniEnemiesStomp : MonoBehaviour
{
    private const float PLAYER_REBOUND_FORCE = 10f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Weak Point"))
        {
            // 2. Obtener la referencia al Transform del objeto padre.
            Transform parentTransform = collision.gameObject.transform.parent;

            if (parentTransform != null)
            {
                // Destruimos el objeto padre (el Mini-enemigo completo)
                Destroy(parentTransform.gameObject);

                // Rebotar el jugador
                ReboundPlayer();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. Revisamos si la colisión sólida es con el JEFE
        if (collision.gameObject.CompareTag("Boss"))
        {
            Vector2 contactNormal = collision.GetContact(0).normal;

            if (contactNormal.y > 0.5f) 
            {
                Boss1 bossScript = collision.gameObject.GetComponent<Boss1>();

                if (bossScript != null)
                {
                    bossScript.TakeHit();
                    ReboundPlayer(); // Rebotamos al jugador
                }
            }
        }
    }

    private void ReboundPlayer()
    {
        Rigidbody2D playerRb = GetComponentInParent<Rigidbody2D>();
        if (playerRb != null)
        {

            playerRb.velocity = new Vector2(playerRb.velocity.x, PLAYER_REBOUND_FORCE);
        }
    }
}

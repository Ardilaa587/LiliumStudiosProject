using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniEnemiesStomp : MonoBehaviour
{
    private const float reboundForce = 2f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Weak Point"))
        {
            Transform parentTransform = collision.gameObject.transform.parent;

            if (parentTransform != null)
            {
                Destroy(parentTransform.gameObject);

                ReboundPlayer();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Boss"))
        {
            Vector2 contactNormal = collision.GetContact(0).normal;

            if (contactNormal.y > 0.5f) 
            {
                Boss1 bossScript = collision.gameObject.GetComponent<Boss1>();

                if (bossScript != null)
                {
                    bossScript.TakeHit();
                    ReboundPlayer(); 
                }
            }
        }
    }

    private void ReboundPlayer()
    {
        Rigidbody2D playerRb = GetComponentInParent<Rigidbody2D>();
        if (playerRb != null)
        {

            playerRb.velocity = new Vector2(playerRb.velocity.x, reboundForce);
        }
    }
}

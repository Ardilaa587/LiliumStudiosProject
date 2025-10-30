using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniEnemiesStomp : MonoBehaviour
{
    private const float reboundForce = 2f;
    [SerializeField] private float respawnTime = 5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Weak Point"))
        {
            Transform parentTransform = collision.gameObject.transform.parent;

            if (parentTransform != null)
            {
                StartCoroutine(DespawnAndRespawn(parentTransform.gameObject));

                ReboundPlayer();
            }
        }
    }

    private IEnumerator DespawnAndRespawn(GameObject objectToHandle)
    {
        // 1. Desactiva el objeto (simula la "destrucción")
        objectToHandle.SetActive(false);

        // 2. Espera el tiempo de reaparición
        yield return new WaitForSeconds(respawnTime);

        // 3. Reactiva el objeto (la "reaparición")
        objectToHandle.SetActive(true);
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

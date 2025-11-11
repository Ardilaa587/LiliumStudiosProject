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
        objectToHandle.SetActive(false);

        yield return new WaitForSeconds(respawnTime);

        objectToHandle.SetActive(true);
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

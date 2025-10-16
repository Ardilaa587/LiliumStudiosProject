using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsDamage : MonoBehaviour
{
    [SerializeField] private float objectDamage;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed;

    [SerializeField] private float ObjectHitTime;
    [SerializeField] private float ObjectHitForceX;
    [SerializeField] private float ObjectHitForceY;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name);
        Debug.Log(collision.gameObject.layer);
        if (collision.gameObject.layer  == LayerMask.NameToLayer("Stairs"))
        {
            rb.AddForce(collision.transform.right * speed, ForceMode2D.Impulse);
        }


        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log(collision.gameObject.name);   
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();

            player.TakeDamage(objectDamage);

            player.hitTime = ObjectHitTime;
            player.hitForceX = ObjectHitForceX;
            player.hitForceY = ObjectHitForceY;

            if (gameObject.CompareTag("CoffeeBags"))
            {
                Destroy(gameObject);
            }
                        
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("DestroyCoffeeBags"))
        {
            Destroy(gameObject);
        }
    }
}

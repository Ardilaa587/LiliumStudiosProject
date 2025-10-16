using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatforms : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private float fallDelay;
    [SerializeField] private float respawnDelay;

    private Vector3 initialPosition;
    private Quaternion intialRotation;
    private bool falling = false;

    // Start is called before the first frame update
    void Start()
    {
        if(!rb)
            rb = GetComponent<Rigidbody2D>();

        initialPosition = transform.position;
        intialRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    { 
        if (falling)
            return;

        if (collision.transform.tag == "Player")
        {
            StartCoroutine(StartFall());
        }
    }

    private IEnumerator StartFall()
    {
        falling = true;

        yield return new WaitForSeconds(fallDelay);

        rb.bodyType = RigidbodyType2D.Dynamic;
        
        yield return new WaitForSeconds(respawnDelay);

        RespawnPlatform();
    }

    private void RespawnPlatform()
    {
        transform.position = initialPosition;
        transform.rotation = intialRotation;

        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;

        rb.bodyType = RigidbodyType2D.Kinematic;
        falling = false;
    }
}

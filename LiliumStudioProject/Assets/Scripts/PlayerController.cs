using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;

    [SerializeField] private float speed;
    public float horizontal;
    public float jumpingPower = 5f;

    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundRadius;

    public float health;

    private bool isGrounded;
    private bool canDoubleJump;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        //rb.AddForce(new Vector2(horizontal * speed, 0));
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
    }


    public void Jump(InputAction.CallbackContext context)
    {
        // saltar
        if (context.performed)
        {
            if (OnGrounded()) // primer salto normal
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
                canDoubleJump = true; // habilitamos doble salto
            }
            else if (canDoubleJump) // segundo salto
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
                canDoubleJump = false; // lo gastamos
            }
        }

        // corte de salto si sueltas botón
        if (context.canceled && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    }

    public bool OnGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
    }

        
}

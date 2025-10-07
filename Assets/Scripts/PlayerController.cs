using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


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

    private bool canDash;
    private bool isDashing;
    [SerializeField] private float dashingPower;
    [SerializeField] private float dashingTime;
    [SerializeField] private float dashingCooldown;

    // Start is called before the first frame update
    void Start()
    {
        canDash = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isDashing)
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }
        
        
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

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash)
        {
            StartCoroutine(Dash());
        }
    }


    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        float dashDirection = horizontal != 0 ? Mathf.Sign(horizontal) : 1f;
        rb.velocity = new Vector2(dashDirection * dashingPower, 0f);

        yield return new WaitForSeconds(dashingTime);
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;

    [SerializeField] private float speed;
    public float horizontal;
    public float jumpingPower;

    [SerializeField] private float coyoteTime;
    private float coyoteTimeCounter;

    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundRadius;

    public float health;

    private bool isGrounded;
    private bool canDoubleJump;
    private bool isFalling;

    private bool canDash;
    private bool isDashing;
    [SerializeField] private float dashingPower;
    [SerializeField] private float dashingTime;
    [SerializeField] private float dashingCooldown;

    [SerializeField] private float levitateDuration;
    [SerializeField] private float gravityLevitate;
    private bool isLevitating;
    private Coroutine levitateCoroutine;

    [SerializeField] private float gravity;

    // Start is called before the first frame update
    void Start()
    {
        
        rb.gravityScale = gravity;

        canDash = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(OnGrounded())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }


        if (!isDashing)
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }

        if(rb.velocity.y <= 0 && !OnGrounded())
        {
            isFalling = true;
        }
        else
        {
            isFalling = false;
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
            if (coyoteTimeCounter > 0f) 
            {
                
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
                
                canDoubleJump = true; 
            }
            else if (canDoubleJump) 
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
                canDoubleJump = false; 
            }
        }

        
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

        //float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        float dashDirection = horizontal != 0 ? Mathf.Sign(horizontal) : 1f;
        rb.velocity = new Vector2(dashDirection * dashingPower, 0f);

        yield return new WaitForSeconds(dashingTime);
        rb.gravityScale = gravity;
        //rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    public void Levitate(InputAction.CallbackContext context)
    {
        
        if (context.performed && !OnGrounded() && !isLevitating && isFalling)
        {
            levitateCoroutine = StartCoroutine(LevitateTimer());
        }

        if (context.canceled)
        {
            StopLevitate();
        }
    }

    private IEnumerator LevitateTimer()
    {
        isLevitating = true;
        rb.gravityScale = gravityLevitate;//Levitate

        yield return new WaitForSeconds(levitateDuration);

        StopLevitate();
    }
    private void StopLevitate()
    {
        
        if (levitateCoroutine != null)
        {
            StopCoroutine(levitateCoroutine);
            levitateCoroutine = null;
        }

        rb.gravityScale = gravity;
        isLevitating = false;
    }



}

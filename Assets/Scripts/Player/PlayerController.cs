using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    [SerializeField] public float gravity;

    #region Movement Variables
    [SerializeField] private float speed;
    public float horizontal;
    private bool isGrounded;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundRadius;
    #endregion

    #region Jump Variables
    public float jumpingPower;
    [SerializeField]private float jumpCount = 0f;
    private float maxJumps = 2f;
    private bool wasGrounded = false;
    #endregion

    #region Coyote Time Variables
    [SerializeField] private float coyoteTime;
    private float coyoteTimeCounter;
    [SerializeField] private float coyoteGravity;
    #endregion

    #region Dash Variables
    private bool canDash;
    private bool isDashing;
    [SerializeField] private float dashingPower;
    [SerializeField] private float dashingTime;
    [SerializeField] private float dashingCooldown;
    #endregion

    #region Levitate Variables
    //Variables de Levitate
    [SerializeField] private float levitateDuration;
    [SerializeField] private float gravityLevitate;
    private bool isLevitating;
    private Coroutine levitateCoroutine;
    #endregion

    #region Hit Variables
    public float hitTime;
    public float hitForceX;
    public float hitForceY;
    public bool hitFromRight;
    #endregion

    public float health;
    [SerializeField] public float maxHealth;
    [SerializeField] private HealthUI healthUI;

    private bool isFalling;

    [SerializeField] private GameObject pickUp;


    // Start is called before the first frame update
    void Start()
    {

        rb.gravityScale = gravity;

        health = maxHealth;

        canDash = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {


        if (OnGrounded() && !wasGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            jumpCount = 0f;
        }
        else if (!OnGrounded())
        {
            coyoteTimeCounter -= Time.deltaTime;

        }

        wasGrounded = OnGrounded();

        if (!OnGrounded() && coyoteTimeCounter > 0f)
        {
            rb.gravityScale = coyoteGravity;
        }
        else if (!isLevitating && !isDashing)
        {
            rb.gravityScale = gravity;
        }

        if (!isDashing)
        {
            
            if (hitTime <= 0)
            {
                rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);

            }
            else
            {
                if (hitFromRight)
                {
                    rb.velocity = new Vector2(hitForceX, hitForceY);
                }
                else if (!hitFromRight)
                {
                    rb.velocity = new Vector2(-hitForceX, hitForceY);
                }

            }
        }

        if (rb.velocity.y <= 0 && !OnGrounded())
        {
            isFalling = true;
        }
        else
        {
            isFalling = false;
        }

        
        hitTime -= Time.deltaTime;
    }

    #region Input System Methods

    #region Movement Methods
    public void Move(InputAction.CallbackContext context)
    {
        Debug.Log("Move");
        horizontal = context.ReadValue<Vector2>().x;
    }
    #endregion

    #region Jump Methods
    public void Jump(InputAction.CallbackContext context)
    {
        // saltar
        if (context.performed)
        {
            
            if (OnGrounded() || coyoteTimeCounter > 0f) 
            {
                
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
                    coyoteTimeCounter = 0f;
                jumpCount++;


            }
            else if (!OnGrounded() && jumpCount < maxJumps)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
                jumpCount++;
            }
            
        }

        
        if (context.canceled && rb.velocity.y > 0f)
        {
           rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            
        }
    }
    #endregion

    #region OnGrounded Method
    public bool OnGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
    }
    #endregion

    #region Dash Methods
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

        Debug.Log("Dash");

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
    #endregion

    #region Levitate Methods
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
        Debug.Log("Levitate Activated");
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
    #endregion
    #endregion

    public void TakeDamage(float damage)
    {
        

        if (health - damage <= 0)
        {
            health = 0;
        }
        else
        {
            health -= damage;
        }

        if (healthUI != null)
            healthUI.UpdateHearts();
        else
            Debug.LogError(" healthUI no está asignado en el PlayerController");
    }

    public void AddHealth(float _health)
    {
        

        if (health + _health > maxHealth)
        {
            health = maxHealth;
        }
        else
        {
            health += _health;
        }

        healthUI.UpdateHearts();
    }

 

}

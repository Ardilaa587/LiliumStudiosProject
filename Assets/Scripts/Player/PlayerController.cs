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
    [SerializeField] private float jumpCount = 0f;
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

    [SerializeField] private GameOverUI gameOverUI;

    [SerializeField] private Animator playerAnimator;
    private bool isFacingRight = true;

    // Start is called before the first frame update
    void Start()
    {
        playerAnimator = GetComponent<Animator>();

        rb.gravityScale = gravity;

        health = maxHealth;

        canDash = true;

        if (RespawnManager.instance != null && RespawnManager.instance.lastRespawnPosition != Vector2.zero)
        {
            RespawnManager.instance.RespawnPlayer(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        string nombreObjeto = collision.gameObject.name;
        Debug.Log("Colisionaste con: " + nombreObjeto);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        playerAnimator.SetFloat("Direction", horizontal);
        if (isFacingRight == true && horizontal < 0)
        {
            Flip();
        }
        else if (isFacingRight == false && horizontal > 0)
        {
            Flip();
        }

        if (OnGrounded() && !wasGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            jumpCount = 0f;
            playerAnimator.SetBool("isJumping", false);
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
        if (context.started)
        {
            

            if (OnGrounded() || coyoteTimeCounter > 0f)
            {

                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
                coyoteTimeCounter = 0f;
                jumpCount++;

                playerAnimator.SetBool("isJumping", true);
            }
            else if (!OnGrounded() && jumpCount < maxJumps)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
                jumpCount++;
                playerAnimator.SetBool("isJumping", true);
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

        if (context.performed && !OnGrounded() && !isLevitating)
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

            if (gameOverUI != null)
            {
                Time.timeScale = 0f;
                gameOverUI.gameObject.SetActive(true);
            }
        }
        else
        {
            health -= damage;
        }

        if (healthUI != null)
        {
            healthUI.UpdateHearts();
        }

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

    #region Auxiliar Methods
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.ShaderKeywordFilter;
#endif
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;


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
    public bool isFacingRight = true;

    [SerializeField] private Transform cameraFollowTarget;
    [SerializeField] private float cameraLeadDistance;

    public bool isPrimeActive = false;
    private readonly int PrimeBoolHash = Animator.StringToHash("Prime");

    // Start is called before the first frame update
    void Start()
    {
        playerAnimator = GetComponent<Animator>();

        if (playerAnimator != null)
        {
            playerAnimator.SetBool(PrimeBoolHash, isPrimeActive);
        }

        rb.gravityScale = gravity;

        health = maxHealth;

        canDash = true;

        if (RespawnManager.instance != null && RespawnManager.instance.lastRespawnPosition != Vector2.zero)
        {
            RespawnManager.instance.RespawnPlayer(gameObject);
        }

        CheckSceneForPrimeStatus();
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

        if (playerAnimator != null)
        {
            playerAnimator.SetBool(PrimeBoolHash, isPrimeActive);
        }

        float targetX = transform.position.x + (isFacingRight ? cameraLeadDistance : -cameraLeadDistance);
        cameraFollowTarget.position = Vector3.Lerp(cameraFollowTarget.position,
        new Vector3(targetX, transform.position.y, transform.position.z),
        Time.deltaTime * 10f);

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
        bool canJump = false;

        // saltar
        if (context.started)
        {
            

            if (OnGrounded() || coyoteTimeCounter > 0f)
            {

                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
                coyoteTimeCounter = 0f;
                jumpCount++;
                canJump = true;
            }
            else if (!OnGrounded() && jumpCount < maxJumps)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
                jumpCount++;
                canJump = true;
            }



        }


        if (context.canceled && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            
        }

        if (canJump && playerAnimator != null)
        {
            // Llama al Trigger "Jump" en el Animator Controller.
            // Esto iniciará la transición de Idle/Run a Jump que configuramos.
            playerAnimator.SetTrigger("Jump");
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

        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Dash");
        }

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

            if (isPrimeActive)
            {
                if (playerAnimator != null)
                {
                    playerAnimator.SetTrigger("LevJump");
                    Debug.Log("[Levitate] Animación LevJump activada (Estado Prime).");
                }
                // Salimos aquí para que NO llame al trigger "Jump" normal más abajo.
                return;
            }
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

        playerAnimator.SetTrigger("Hit");
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

    // Dentro de tu script PlayerController.cs (o el que controle el movimiento)

    private bool canMove = true;

    public void DisableMovement(bool disable)
    {
        canMove = !disable;
        // Asegúrate de que tu lógica de movimiento en Update/FixedUpdate SOLO se ejecute si canMove es true.
        if (!canMove)
        {
            // Detener la velocidad actual al ser deshabilitado
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }

        if (rb != null)
        {
            rb.simulated = !disable;

            if (disable)
            {
                rb.velocity = Vector2.zero; // Detener cualquier movimiento residual
            }
        }
    }

    public void ActivatePrime(bool activate)
    {
        isPrimeActive = activate;
        Debug.Log($"isPrimeActive establecido a: {activate}");
    }

    public void SoftRespawn(Vector2 respawnPosition)
    {
        // 1. Reiniciar la POSICIÓN
        transform.position = respawnPosition;

        // 2. Reiniciar la VIDA
        health = maxHealth;
        if (healthUI != null)
        {
            healthUI.UpdateHearts();
        }

        // 3. Limpiar variables de movimiento para evitar momentum residual
        rb.velocity = Vector2.zero;
        horizontal = 0f;

        // Reiniciar variables temporales de movimiento si es necesario:
        hitTime = 0f; // Asegurar que el knockback se detiene.
        isDashing = false;
        canDash = true; // Restaurar la capacidad de Dash.

        // Detener el Levitate si estaba activo
        if (isLevitating)
        {
            StopLevitate();
        }

        // 🌟 CLAVE: NO TOCAMOS 'isPrimeActive' NI EL ANIMATOR AQUÍ.
        // El valor de isPrimeActive se mantendrá desde la última vez que fue establecido.

        // Opcional: Si el jugador estaba deshabilitado (por Game Over), habilitar la física.
        DisableMovement(false);

        Debug.Log("✅ SoftRespawn completado. Vida y Posición actualizadas.");
    }

    private void CheckSceneForPrimeStatus()
    {
        // Obtiene el nombre de la escena actual
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Definimos los nombres de las escenas que activan 'prime'
        // 🌟 Reemplaza "Nombre_Nivel_2" y "Nombre_Nivel_3" con los nombres reales de tus escenas
        if (currentSceneName == "Level2" || currentSceneName == "Level3")
        {
            isPrimeActive = true;
            Debug.Log("PRIME activado debido a la escena: " + currentSceneName);
        }
        else
        {
            isPrimeActive = false; // O mantén el valor por defecto si es otro nivel
            Debug.Log("PRIME desactivado en la escena: " + currentSceneName);
        }
    }
    #endregion
}

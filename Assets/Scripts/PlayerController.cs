using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // Enhanced inspector with headers and tooltips for better organization 

    [Header("Player Stats")]
    public int coins;
    public int health = 100;
    public Image healthImage;

    [Header("Movement Settings")]
    public float moveSpeed = 4f;

    [Header("Jump Settings")]
    public float jumpForce = 10f;
    public float jumpContinuesForce = 1f;
    public int extraJumpsValue = 1;
    // 
    private int extraJumps;


    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    // 
    private bool isGrounded;


    [Header("Jump Polish - Coyote Time")]
    public float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    [Header("Jump Polish - Jump Buffer")]
    public float jumpBufferTime = 0.15f;
    private float jumpBufferCounter;

    [Header("Dash Mechanic - UNIQUE FEATURE")]
    [Tooltip("Horizontal dash speed")]
    public float dashSpeed = 20f;

    [Tooltip("Duration of dash in seconds")]
    public float dashDuration = 0.2f;

    [Tooltip("Cooldown time between dashes")]
    public float dashCooldown = 1f;

    [Tooltip("UI image showing dash cooldown")]
    public Image dashCooldownImage;

    // Dash state tracking
    private bool isDashing = false;
    private bool canDash = true;
    private float dashTimeLeft;
    private float dashCooldownLeft;
    private Vector2 dashDirection;
    private TrailRenderer trailRenderer;


    [Header("Audio Clips")]
    [Tooltip("Sound played when jumping")]
    public AudioClip jumpClip;

    [Tooltip("Sound played when taking damage")]
    public AudioClip hurtClip;

    [Tooltip("Sound played when dashing")]
    public AudioClip dashClip;

    [Tooltip("Sound played when collecting a strawberry")]
    public AudioClip strawberryClip;

    //[Tooltip("Sound played when collecting a coin")]
    //public AudioClip coinClip;

    // 
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;



    void Start()
    {
        // Grab the Rigidbody2D attached to the Player object once at the start.
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Trail
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.enabled = false;

        extraJumps = extraJumpsValue;

        // Initialize dash cooldown UI if assigned
        if (dashCooldownImage != null)
        {
            dashCooldownImage.fillAmount = 1f;
        }
    }

    void Update()
    {
        // Skip input during dash
        if (isDashing)
        {
            HandleDash();
            return;
        }

        HandleMovement();
        HandleGroundCheck();
        HandleCoyoteTime();
        HandleJumpBuffer();
        HandleJump();
        HandleGravity();
        HandleDashInput();
        HandleDashCooldown();
        HandleAnimation();
        CheckFallDeath();
    }

    // Handle horizontal movement based on player input
    private void HandleMovement()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Flip sprite based on movement direction
        if (moveInput != 0)
        {
            spriteRenderer.flipX = moveInput < 0;
        }
    }
    // Handle ground detection
    private void HandleGroundCheck()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }
    // Handle coyote time for jumping
    private void HandleCoyoteTime()
    {
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            extraJumps = extraJumpsValue;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
    }
    // Handle jump buffering for jumping
    private void HandleJumpBuffer()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
    }

    // 
    private void HandleJump()
    {
        // Execute buffered jump
        if (jumpBufferCounter > 0f)
        {
            // Ground or coyote time jump
            if (coyoteTimeCounter > 0f)
            {
                PerformJump();
                coyoteTimeCounter = 0f;
                jumpBufferCounter = 0f;
            }
            // Extra jump (double/triple jump)
            else if (extraJumps > 0)
            {
                PerformJump();
                extraJumps--;
                jumpBufferCounter = 0f;
            }
        }

        // Variable jump height - hold for higher jump
        if (Input.GetKey(KeyCode.Space) && rb.linearVelocityY > 0)
        {
            //Debug.Log("Applying continued jump force");
            rb.AddForceY(jumpContinuesForce);
        }
    }
    // Perform the jump action
    private void PerformJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        PlaySFX(jumpClip);
    }

    private void HandleGravity()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = 3f; // Faster falling
        }
        else
        {
            rb.gravityScale = 2f; // Normal rising
        }
    }

    // Handle dash input and execution
    private void HandleDashInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartDash();
        }
    }

    // Handle dash mechanics and cooldown
    private void StartDash()
    {
        isDashing = true;
        canDash = false;
        dashTimeLeft = dashDuration;
        dashCooldownLeft = dashCooldown;

        // Dash in facing direction
        dashDirection = spriteRenderer.flipX ? Vector2.left : Vector2.right;

        // Disable gravity during dash
        rb.gravityScale = 0f;

        PlaySFX(dashClip);
        trailRenderer.enabled = true;
        // Flash sprite
        StartCoroutine(FlashSprite());
    }
    // Handle dash movement and timing
    private void HandleDash()
    {
        dashTimeLeft -= Time.deltaTime;

        if (dashTimeLeft > 0)
        {
            // Apply dash velocity
            rb.linearVelocity = dashDirection * dashSpeed;
        }
        else
        {
            EndDash();
        }
    }

    // Handle dash cooldown timer and UI
    private void EndDash()
    {
        isDashing = false;
        rb.gravityScale = 2f;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.5f, rb.linearVelocity.y);
        trailRenderer.enabled = false;
    }

    private void HandleDashCooldown()
    {
        if (!canDash)
        {
            dashCooldownLeft -= Time.deltaTime;

            if (dashCooldownImage != null)
            {
                dashCooldownImage.fillAmount = 1f - (dashCooldownLeft / dashCooldown);
            }

            if (dashCooldownLeft <= 0)
            {
                canDash = true;
                if (dashCooldownImage != null)
                {
                    dashCooldownImage.fillAmount = 1f;
                }
            }
        }
    }

    private void HandleAnimation()
    {
        float moveInput = Input.GetAxis("Horizontal");

        if (isGrounded)
        {
            animator.Play(moveInput != 0 ? "Player_Run" : "Player_Idle");
        }
        else
        {
            animator.Play(rb.linearVelocity.y > 0 ? "Player_Jump" : "Player_Fall");
        }
    }

    private void CheckFallDeath()
    {
        if (transform.position.y < -10f)
        {
            Die();
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Damage"))
        {
            TakeDamage(10);
        }
        else if (collision.gameObject.CompareTag("BouncePad"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * 2);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Strawberry"))
        {
            extraJumps = 2;
            Destroy(collision.gameObject);

            // Play strawberry collection sound
            if (strawberryClip != null)
            {
                PlaySFX(strawberryClip);
            }

        }
    }


    private void TakeDamage(int damage)
    {
        PlaySFX(hurtClip);
        health -= damage;

        if (healthImage != null)
        {
            healthImage.fillAmount = health / 100f;
        }

        // Bounce player upward when damaged
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        StartCoroutine(BlinkRed());

        if (health <= 0)
        {
            Die();
        }
    }


    //private void SetAnimation(float moveInput)
    //{
    //    if (isGrounded)
    //    {
    //        if (moveInput != 0)
    //        {
    //            animator.Play("Player_Run");
    //        }
    //        else
    //        {
    //            animator.Play("Player_Idle");
    //        }
    //    }
    //    else
    //    {
    //        if (rb.linearVelocity.y > 0)
    //        {
    //            animator.Play("Player_Jump");
    //        }
    //        else
    //        {
    //            animator.Play("Player_Fall");
    //        }
    //    }
    //}
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.tag == "Damage")
    //    {
    //        PlaySFX(hurtClip);
    //        health -= 25;
    //        healthImage.fillAmount = health / 100f;

    //        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    //        StartCoroutine(BlinkRed());

    //        if (health <= 0)
    //        {
    //            Die();
    //        }
    //    }
    //    else if (collision.gameObject.tag == "BouncePad")
    //    {
    //        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * 2);
    //    }
    //}

    private IEnumerator BlinkRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }



    private void Die()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }


    public void PlaySFX(AudioClip audioClip, float volume = 1f)
    {
        if (audioClip == null) return;

        AudioManager.Instance.PlaySFX(audioClip, volume);
    }



    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.gameObject.tag == "Strawberry")
    //    {
    //        extraJumps = 2;
    //        Destroy(collision.gameObject);
    //    }
    //}


    // lash between white and normal color during dash
    private IEnumerator FlashSprite()
    {
        // Flash between white and normal color
        Color original = spriteRenderer.color;

        while (isDashing)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.05f);
            spriteRenderer.color = original;
            yield return new WaitForSeconds(0.05f);
        }

        spriteRenderer.color = original; // Ensure it resets
    }


}

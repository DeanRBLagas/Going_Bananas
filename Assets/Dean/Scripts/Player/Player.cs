using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, IDamageable
{
    public int money;
    public int maxHealth;
    public int currentHealth;
    public GameObject gameOverUI;
    public GameObject pauseUI;
    [SerializeField] private Rigidbody2D rb; // Store the rigidbody2D component
    private bool isFacingRight = true; // Check if the player is facing right

    [Header("Movement")]
    public float maxSpeed = 5f; // Max speed of the player
    public float speed = 5f; // Speed of the player
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashCooldown = 5f;
    private float horizontalMovement; // Store the horizontal movement input
    private bool hasDashed;
    private bool isDashing;

    [Header("Jumping")]
    public float maxJumpForce = 22f; // Jump force of the player
    public float doubleJumpForce = 10f;
    public int maxDoubleJumps = 1; // Maximum time the player can jump
    public int jumpsRemaining; // Store the number of jumps performed

    [Header("GroundCheck")]
    [SerializeField] private Transform groundCheck; // Transform to check if the player is on the ground
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.1f, 0.01f); // Size of the ground check area
    [SerializeField] private LayerMask groundLayer; // Layer mask to check for ground
    public bool isGrounded; // Check if the player is on the ground

    [Header("Gravity")]
    [SerializeField] private float baseGravity = 2f; // Base gravity of the player
    [SerializeField] private float maxFallSpeed = 15f; // Maximum fall speed of the player
    [SerializeField] private float fallSpeedMultiplier = 2f; // Multiplier for fall gravity

    [Header("WallCheck")]
    [SerializeField] private Transform wallCheck; // Transform to check if the player is on the ground
    [SerializeField] private Vector2 wallCheckSize = new Vector2(0.1f, 0.01f); // Size of the ground check area
    [SerializeField] private LayerMask wallLayer; // Layer mask to check for ground

    [Header("WallMovement")]
    [SerializeField] private float wallSlideSpeed = 2f; // Speed of the player when sliding on a wall
    [SerializeField] private float wallJumpTime = 0.5f; // Store the time of the wall jump
    [SerializeField] private Vector2 wallJumpPower = new Vector2(5, 22); // Cooldown time for the wall
    public float wallJumpTimer; // Store the time of the wall jump
    public bool isWallJumping; // Check if the player is jumping
    public float wallJumpDirection; // Store the direction of the wall jump
    public bool isWallSliding; // Gravity of the player when sliding on a wall

    [Header("Knockback")]
    [SerializeField] private float knockbackDuration = 0.2f;
    private bool isKnockedBack;
    private float knockbackTimer;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private AnimatorClipInfo[] animatorinfo;
    [SerializeField] private string current_animation;

    private void Start()
    {
        currentHealth = maxHealth;
        gameOverUI.SetActive(false);
        pauseUI.SetActive(false);
    }

    private void Update()
    {
        GroundCheck(); // Call the ground check method to check if the player is on the ground
        Gravity(); // Call the gravity method to apply gravity to the player
        WallSlide(); // Call the wall slide method to check if the player is sliding on a wall
        ProcessWallJump(); // Call the wall jump method to check if the player is jumping off a wall

        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f)
            {
                isKnockedBack = false;
            }
            return; // Skip movement while knocked back
        }

        if (!isWallJumping && !isDashing) // Check if the player is not jumping off a wall
        {
            rb.linearVelocity = new Vector2(horizontalMovement * speed, rb.linearVelocity.y); // Set the velocity of the player
            Flip(); // Call the flip method to check if the player needs to change direction
        }
        animatorinfo = this.animator.GetCurrentAnimatorClipInfo(0);
        current_animation = animatorinfo[0].clip.name;

        HandleAnimation();
    }

    private void Gravity()
    {
        if (!isDashing)
        {
            if (rb.linearVelocity.y < 0) // Check if the player is falling
            {
                rb.gravityScale = baseGravity * fallSpeedMultiplier; // Increase gravity when falling
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed)); // Limit the fall speed
            }
            else
            {
                rb.gravityScale = baseGravity; // Reset gravity to base value
            }
        }
    }

    private void WallSlide()
    {
        if (!isGrounded && WallCheck() && horizontalMovement != 0) // Check if the player is sliding on a wall
        {
            isWallSliding = true; // Set the player to wall sliding state
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed)); // Set the velocity of the player when sliding on a wall
        }
        else // If the player is not sliding anymore
        {
            isWallSliding = false; // Reset the wall sliding state
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x; // Read the horizontal movement input
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && !hasDashed && horizontalMovement != 0)
        {
            StartCoroutine(PerformDash());
        }
    }

    private IEnumerator PerformDash()
    {
        hasDashed = true;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;

        Vector2 dashDirection = new Vector2(isFacingRight ? 1 : -1, 0);
        rb.linearVelocity = dashDirection * dashSpeed;

        yield return new WaitForSeconds(0.2f); // Dash duration
        isDashing = false;

        rb.gravityScale = originalGravity;
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Stop horizontal movement after dash

        yield return new WaitForSeconds(dashCooldown); // Cooldown time
        hasDashed = false;
    }

    private void Flip()
    {
        if (isFacingRight && horizontalMovement < 0 || !isFacingRight && horizontalMovement > 0) // Check if the player is facing right
        {
            isFacingRight = !isFacingRight; // Set the player to face left
            //transform.rotation = horizontalMovement < 0 ? Quaternion.Euler(0, -180, 0) : Quaternion.identity;
            Vector3 scale = transform.localScale; // Get the current scale of the player
            scale.x *= -1; // Flip the x scale to change direction
            transform.localScale = scale; // Apply the new scale to the player
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && wallJumpTimer > 0) // Check if the player is jumping off a wall
        {
            isWallJumping = true; // Set the player to wall jumping state
            int direction = wallJumpDirection == 180 ? 1 : -1;
            rb.linearVelocity = new Vector2(direction * wallJumpPower.x, wallJumpPower.y); // Set the velocity of the player when jumping
            wallJumpTimer = 0; // Reset the wall jump timer

            bool shouldFlip = (wallJumpDirection == 180 && !isFacingRight) || (wallJumpDirection == 0 && isFacingRight);
            if (shouldFlip) // Check if the player is facing right
            {
                isFacingRight = !isFacingRight; // Set the player to face left
                //transform.rotation = horizontalMovement < 0 ? Quaternion.identity : Quaternion.Euler(0, 180, 0);
                Vector3 scale = transform.localScale; // Get the current scale of the player
                scale.x *= -1; // Flip the x scale to change direction
                transform.localScale = scale; // Apply the new scale to the player
            }
            Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f); // Cancel the wall jump after the wall jump time
            return;
        }

        if (isGrounded == true) // Check if the player can jump
        {
            if (context.performed)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, maxJumpForce);
            }
            else if (context.canceled) // When jump button is released
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
            }
        }
        else if (jumpsRemaining > 0)
        {
            if (context.performed)
            {
                jumpsRemaining--; // Decrease the number of jumps remaining
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, doubleJumpForce);
            }
            else if (context.canceled) // When jump button is released
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
            }
        }
    }

    private void GroundCheck()
    {
        if (Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer)) // Check if the player is on the ground
        {
            jumpsRemaining = maxDoubleJumps; // Reset the number of jumps remaining
            isGrounded = true; // Set the player to grounded state
            isWallJumping = false;
        }
        else
        {
            isGrounded = false; // Set the player to not grounded state
        }
    }

    private bool WallCheck()
    {
        return Physics2D.OverlapBox(wallCheck.position, wallCheckSize, 0f, wallLayer); // Check if the player is touching a wall
    }

    private void ProcessWallJump()
    {
        if (isWallSliding) // Check if the player is sliding on a wall
        {
            isWallSliding = false; // Reset the wall sliding state
            wallJumpDirection = horizontalMovement < 0 ? 180 : 0; // Set the wall jump direction to the opposite of the player's facing direction
            wallJumpTimer = wallJumpTime; // Set the wall jump timer to the wall jump time

            CancelInvoke(nameof(CancelWallJump)); // Cancel the wall jump timer
        }
        else if (wallJumpTimer > 0) // If the player is not sliding anymore
        {
            wallJumpTimer = Mathf.Max(0f, wallJumpTimer - Time.deltaTime); // Decrease the wall jump timer
        }
    }

    private void CancelWallJump()
    {
        isWallJumping = false; // Reset the wall jumping state
    }

    public void ApplyKnockback(Vector2 force)
    {
        rb.linearVelocity = force;
        isKnockedBack = true;
        knockbackTimer = knockbackDuration;
    }

    public IEnumerator Slow(float slowInterval)
    {
        yield return this;
        speed = maxSpeed / 2;
        yield return new WaitForSeconds(slowInterval);
        speed = maxSpeed;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        gameOverUI.SetActive(true);
        Time.timeScale = 0;
    }

    public void PauseGame()
    {
        pauseUI.SetActive(true);
        Time.timeScale = 0;
    }

    private void HandleAnimation()
    {
        animator.SetBool("IsRunning", horizontalMovement != 0f && isGrounded);
        animator.SetBool("IsIdle", horizontalMovement == 0 && isGrounded && !isWallJumping && !isWallSliding && !isDashing);
        animator.SetBool("IsJumping", rb.linearVelocity.y > 0.1f && !isGrounded);
        animator.SetBool("IsFalling", rb.linearVelocity.y < -0.1f && !isGrounded);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize); // Draw a wire cube for the ground check area
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wallCheck.position, wallCheckSize); // Draw a wire cube for the wall check area
    }
}

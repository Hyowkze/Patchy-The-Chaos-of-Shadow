using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.2f;

    private Rigidbody2D rb;
    private PlayerInputHandler inputHandler;
    private float horizontalInput;
    private bool isDashing = false;
    private float dashTimer;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputHandler = PlayerInputHandler.Instance;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody2D is required.");
            enabled = false;
        }

        if (inputHandler == null)
        {
            Debug.LogError("PlayerInputHandler instance not found.");
            enabled = false;
        }

        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer is required.");
            enabled = false;
        }
    }

    private void Update()
    {
        // Get horizontal input from PlayerInputHandler
        horizontalInput = inputHandler.MoveInput.x;

        // Handle jump input
        if (inputHandler.JumpTriggered && IsGrounded())
        {
            Jump();
            inputHandler.JumpTriggered = false; // Reset the flag immediately after use
        }

        // Handle dash input
        if (inputHandler.DashTriggered && !isDashing)
        {
            StartDash();
            inputHandler.DashTriggered = false; // Reset the flag immediately after use
        }

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0)
            {
                StopDash();
            }
        }

        FlipSprite();
    }

    private void FixedUpdate()
    {
        // Apply horizontal movement
        if (!isDashing)
        {
            MoveHorizontal();
        }
    }

    private void MoveHorizontal()
    {
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    private void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        rb.linearVelocity = new Vector2(inputHandler.MoveInput.x * dashSpeed, rb.linearVelocity.y);
    }

    private void StopDash()
    {
        isDashing = false;
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    private bool IsGrounded()
    {
        // Use a short raycast to check if the player is grounded
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.1f);
        return hit.collider != null;
    }

    private void FlipSprite()
    {
        if (horizontalInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (horizontalInput < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}
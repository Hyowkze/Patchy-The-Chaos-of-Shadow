using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PatchyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float jumpForce = 10f;

    [Header("Layer Detection")]
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask groundLayer;
    private bool isAgainstWall;
    private bool isGrounded;
    private float lastMoveDirection;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    private bool isDashing;
    private bool canDash = true;
    private float dashTimeLeft;
    private float dashCooldownTimeLeft;
    private Vector2 dashDirection;

    [Header("Components")]
    private Rigidbody2D rb;
    private PlayerInputHandler inputHandler;

    private float horizontalInput;
    private bool facingRight = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        inputHandler = PlayerInputHandler.Instance;
    }

    private void Update()
    {
        // Handle input reading
        horizontalInput = inputHandler.MoveInput.x;

        // Handle Dash
        if (inputHandler.DashTriggered && canDash && !isDashing)
        {
            StartDash();
        }

        HandleDashState();

        // Simplificar lógica de salto
        if (inputHandler.JumpTriggered && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            Debug.Log($"Jumping! Velocity: {rb.linearVelocity.y}");
            inputHandler.JumpTriggered = false;
        }

        // Check if player is changing direction while against wall
        if (isAgainstWall && Mathf.Sign(horizontalInput) != Mathf.Sign(lastMoveDirection))
        {
            isAgainstWall = false;
        }

        // Handle sprite flipping
        if ((horizontalInput > 0 && !facingRight) || (horizontalInput < 0 && facingRight))
        {
            FlipSprite();
        }
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            rb.linearVelocity = dashDirection * dashSpeed;
            return;
        }

        // Calculate final speed including sprint
        float currentSpeed = moveSpeed * (1f + (inputHandler.SprintValue * (sprintMultiplier - 1f)));
        
        // Only apply horizontal movement if not against wall or changing direction
        float finalHorizontalSpeed = isAgainstWall ? 0 : horizontalInput * currentSpeed;
        rb.linearVelocity = new Vector2(finalHorizontalSpeed, rb.linearVelocity.y);

        if (horizontalInput != 0)
        {
            lastMoveDirection = horizontalInput;
        }
    }

    private void StartDash()
    {
        isDashing = true;
        canDash = false;
        dashTimeLeft = dashDuration;
        dashCooldownTimeLeft = dashCooldown;
        
        // Set dash direction based on input or facing direction
        dashDirection = inputHandler.MoveInput != Vector2.zero 
            ? inputHandler.MoveInput.normalized 
            : new Vector2(facingRight ? 1f : -1f, 0f);

        Debug.Log("Dash started!");
    }

    private void HandleDashState()
    {
        if (isDashing)
        {
            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0)
            {
                isDashing = false;
                rb.linearVelocity = Vector2.zero; // Reset velocity after dash
            }
        }

        if (!canDash)
        {
            dashCooldownTimeLeft -= Time.deltaTime;
            if (dashCooldownTimeLeft <= 0)
            {
                canDash = true;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            float angle = Vector2.Angle(contact.normal, Vector2.up);
            
            // Colisión con pared (ángulo cercano a 90 grados)
            if (((1 << collision.gameObject.layer) & wallLayer) != 0 && angle > 45f)
            {
                isAgainstWall = true;
            }
            
            // Colisión con suelo (ángulo cercano a 0 grados)
            if (((1 << collision.gameObject.layer) & groundLayer) != 0 && angle < 45f)
            {
                isGrounded = true;
                Debug.Log("Ground detected - Can jump now");
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & wallLayer) != 0)
        {
            isAgainstWall = false;
        }
        
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isGrounded = false;
            Debug.Log("Left ground - Cannot jump");
        }
    }

    private void FlipSprite()
    {
        facingRight = !facingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }
}

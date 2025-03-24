using UnityEngine;
using Player.Movement;
using Core.Player.Movement;
using Core.Combat;
using Player.Input;
using Core.Utils;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MovementStateMachine))]
public class PatchyMovement : MonoBehaviour, IMoveable
{
    [Header("Configuration")]
    [SerializeField] public MovementConfig moveConfig;
    [SerializeField] private PhysicsConfig physicsConfig;

    [Header("Layer Detection")]
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask groundLayer;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPoint;

    private IMovementBehavior basicMovement;
    private Player.Movement.IDashBehavior dashBehavior;
    private Player.Movement.ISprintBehavior sprintBehavior;
    private Rigidbody2D rb;
    private PlayerInputHandler inputHandler;
    private CombatSystem combatSystem;
    private MovementStateMachine stateMachine;

    private bool IsAgainstWall;
    public bool IsGrounded;
    private float LastMoveDirection;

<<<<<<< Updated upstream
    private float MoveSpeed => moveConfig.MoveSpeed;
    private float JumpForce => moveConfig.JumpForce;
    private float WallCollisionAngleThreshold => physicsConfig.WallCollisionAngleThreshold;
    private float GroundCollisionAngleThreshold => physicsConfig.GroundCollisionAngleThreshold;

    private bool canMove = true;
    private float horizontalInput;
    private bool facingRight = true;

    public event System.Action<float> OnDashCooldownUpdate;
    public event System.Action<float> OnSprintValueChanged;

    private void Awake()
    {
        if (moveConfig == null || physicsConfig == null)
        {
            Debug.LogError($"Missing required configuration on {gameObject.name}");
            enabled = false;
            return;
        }

        rb = GetComponent<Rigidbody2D>();
        combatSystem = GetComponent<CombatSystem>();
        stateMachine = GetComponent<MovementStateMachine>();

        InitializeBehaviors();
    }

    private void InitializeBehaviors()
    {
        if (moveConfig == null)
        {
            Debug.LogError($"Missing MovementConfig on {gameObject.name}");
            enabled = false;
            return;
        }

        basicMovement = new GroundMovement(moveConfig);
        dashBehavior = new Player.Movement.DashBehavior(moveConfig.DashSettings);
        sprintBehavior = new SprintBehavior(moveConfig.SprintSettings);
    }

    private void Start()
=======
    protected override void Start()
>>>>>>> Stashed changes
    {
        inputHandler = PlayerInputHandler.Instance;
        if (inputHandler == null)
        {
            Debug.LogError("PlayerInputHandler not found in scene");
            enabled = false;
            return;
        }

        if (combatSystem != null)
        {
            combatSystem.OnAttackStateChanged += HandleAttackState;
        }

        inputHandler.OnDashInputChanged += HandleDashInput;
        inputHandler.OnMoveInputChanged += HandleMoveInput;
        inputHandler.OnJumpInputChanged += HandleJumpInput;
        inputHandler.OnSprintInputChanged += HandleSprintInput;
    }

    protected override void ValidateComponents()
    {
        // Validate required components
        RequestComponent<MovementStateMachine>();
        RequestComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!canMove) return;

        UpdateDashState();
        UpdateSprintState();
    }

    private void HandleMoveInput(Vector2 input)
    {
        horizontalInput = input.x;
        HandleSpriteFlip();
    }

    private void HandleJumpInput()
    {
        if (IsGrounded)
        {
            Jump();
        }
    }

    private void HandleSprintInput()
    {
<<<<<<< Updated upstream
        UpdateSprintState();
=======
>>>>>>> Stashed changes
    }

    private void HandleDashInput()
    {
        if (dashBehavior.CanDash)
        {
            InitiateDash();
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, JumpForce);
        stateMachine.ChangeState(MovementStateMachine.MovementState.Jumping);
    }

    private void InitiateDash()
    {
        Vector2 dashDir = CalculateDashDirection();
        rb.linearVelocity = dashBehavior.PerformDash(dashDir);
        stateMachine.ChangeState(MovementStateMachine.MovementState.Dashing);
    }

    private Vector2 CalculateDashDirection()
    {
        return inputHandler.MoveInput != Vector2.zero
            ? inputHandler.MoveInput.normalized
            : new Vector2(facingRight ? 1f : -1f, 0f);
    }

    private void UpdateDashState()
    {
        dashBehavior.UpdateDashState(Time.deltaTime);
        OnDashCooldownUpdate?.Invoke(dashBehavior.CooldownProgress);

        if (dashBehavior.IsDashing && stateMachine.CurrentState != MovementStateMachine.MovementState.Dashing)
        {
            stateMachine.ChangeState(MovementStateMachine.MovementState.Dashing);
        }        
    }

    private void UpdateSprintState()
    {
        float sprintMultiplier = sprintBehavior.CalculateSprintMultiplier(inputHandler.SprintValue);
        OnSprintValueChanged?.Invoke(sprintMultiplier);

        if (inputHandler.SprintValue > 0 && stateMachine.CurrentState != MovementStateMachine.MovementState.Sprinting)
        {
            stateMachine.ChangeState(MovementStateMachine.MovementState.Sprinting);
        }        
    }

    private void HandleSpriteFlip()
    {
        if (horizontalInput > 0 && !facingRight)
        {
            SpriteUtils.FlipSprite(transform, ref facingRight);
        }
        else if (horizontalInput < 0 && facingRight)
        {
            SpriteUtils.FlipSprite(transform, ref facingRight);
        }
    }

    private void FixedUpdate()
    {
        if (!canMove) return;

        if (stateMachine.CurrentState == MovementStateMachine.MovementState.Dashing) return;

        float sprintMultiplier = sprintBehavior.CalculateSprintMultiplier(inputHandler.SprintValue);
        Vector2 targetVelocity = basicMovement.CalculateVelocity(rb.linearVelocity, horizontalInput * sprintMultiplier);

        if (IsAgainstWall && Mathf.Sign(horizontalInput) == Mathf.Sign(LastMoveDirection))
        {
            targetVelocity.x = 0;
        }
        LastMoveDirection = horizontalInput;

        rb.linearVelocity = targetVelocity;

        //No need to call UpdateMovementState here
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            float angle = Vector2.Angle(contact.normal, Vector2.up);

            if (((1 << collision.gameObject.layer) & wallLayer) != 0 && angle > WallCollisionAngleThreshold)
            {
                IsAgainstWall = true;
            }

            if (((1 << collision.gameObject.layer) & groundLayer) != 0 && angle < GroundCollisionAngleThreshold)
            {
                IsGrounded = true;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & wallLayer) != 0)
        {
            IsAgainstWall = false;
        }

        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            IsGrounded = false;
        }
    }

    private void HandleAttackState(bool isAttacking)
    {
        canMove = !isAttacking;
    }

    private void OnDestroy()
    {
        if (combatSystem != null)
        {
            combatSystem.OnAttackStateChanged -= HandleAttackState;
        }
        if (inputHandler != null)
        {
            inputHandler.OnDashInputChanged -= HandleDashInput;
            inputHandler.OnMoveInputChanged -= HandleMoveInput;
            inputHandler.OnJumpInputChanged -= HandleJumpInput;
            inputHandler.OnSprintInputChanged -= HandleSprintInput;
        }
    }

    private void OnDisable()
    {
        if (combatSystem != null)
        {
            combatSystem.OnAttackStateChanged -= HandleAttackState;
        }
        if (inputHandler != null)
        {
            inputHandler.OnDashInputChanged -= HandleDashInput;
            inputHandler.OnMoveInputChanged -= HandleMoveInput;
            inputHandler.OnJumpInputChanged -= HandleJumpInput;
            inputHandler.OnSprintInputChanged -= HandleSprintInput;
        }
    }
}

internal interface IMoveable
{
}

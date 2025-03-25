using UnityEngine;
using Player.Movement;
using Core.Player.Movement;
using Core.Combat;
using Player.InputSystem; // Actualizar este using
using Core.Utils;

[RequireComponent(typeof(MovementStateMachine))]
[RequireComponent(typeof(Rigidbody2D))]
public class PatchyMovement : ComponentRequester
{
    [Header("Configuration")]
    [SerializeField] public MovementConfig moveConfig;
    [SerializeField] private PhysicsConfig physicsConfig;
    [SerializeField] private CombatSystem combatSystem; // Este es el campo que necesitas asignar

    [Header("Layer Detection")]
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask groundLayer;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPoint;

    public bool IsGrounded;

    private MovementStateMachine stateMachine;

    protected override void Awake()
    {
        base.Awake();
        stateMachine = RequestComponent<MovementStateMachine>();
    }

    protected override void Start()
    {
        base.Start();
        if (stateMachine == null)
        {
            Debug.LogError("StateMachine is null in PatchyMovement");
            enabled = false;
            return;
        }
    }

    protected override void SubscribeToEvents()
    {
        PlayerInputHandler.Instance.OnMoveInputChanged += HandleMoveInput;
        PlayerInputHandler.Instance.OnJumpInputChanged += HandleJumpInput;
        PlayerInputHandler.Instance.OnSprintInputChanged += HandleSprintInput;
        PlayerInputHandler.Instance.OnDashInputChanged += HandleDashInput;
    }

    protected override void UnsubscribeFromEvents()
    {
        PlayerInputHandler.Instance.OnMoveInputChanged -= HandleMoveInput;
        PlayerInputHandler.Instance.OnJumpInputChanged -= HandleJumpInput;
        PlayerInputHandler.Instance.OnSprintInputChanged -= HandleSprintInput;
        PlayerInputHandler.Instance.OnDashInputChanged -= HandleDashInput;
    }

    protected override void ValidateComponents()
    {
        stateMachine = RequestComponent<MovementStateMachine>();
        RequestComponent<Rigidbody2D>();
        
        if (moveConfig == null)
        {
            Debug.LogError("MoveConfig is not assigned in PatchyMovement");
            enabled = false;
        }
    }

    private void Update()
    {
        stateMachine.UpdateState();
        stateMachine.HandleInput(); // Add this line
    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdateState();
    }

    private void HandleMoveInput(Vector2 input)
    {        
    }

    private void HandleJumpInput()
    {
        stateMachine.ChangeState(MovementStateMachine.MovementState.Jumping);
    }

    private void HandleSprintInput()
    {
    }

    private void HandleDashInput()
    {
        stateMachine.ChangeState(MovementStateMachine.MovementState.Dashing);
    }
}

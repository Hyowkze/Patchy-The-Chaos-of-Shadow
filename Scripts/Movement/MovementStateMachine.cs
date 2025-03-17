using UnityEngine;

/// <summary>
/// Manages the different movement states of the character
/// </summary>
public class MovementStateMachine : MonoBehaviour
{
    [SerializeField] private PatchyMovement movement;
    
    public enum MovementState
    {
        Idle,
        Walking,
        Jumping
    }

    private MovementState currentState;

    private void Reset()
    {
        movement = GetComponent<PatchyMovement>();
    }

    private void Awake()
    {
        if (movement == null)
        {
            movement = GetComponent<PatchyMovement>();
            Debug.LogWarning($"Movement reference was not set on {gameObject.name}. Auto-assigning.");
        }
    }

    public void ChangeState(MovementState newState)
    {
        ExitState(currentState);
        currentState = newState;
        EnterState(newState);
    }

    private void EnterState(MovementState state)
    {
        switch (state)
        {
            // Empty switch for now - dash and sprint cases removed
        }
    }

    private void ExitState(MovementState state)
    {
        switch (state)
        {
            // Empty switch for now - dash and sprint cases removed
        }
    }

    public MovementState CurrentState => currentState;
}

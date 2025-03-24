using UnityEngine;
using Core.Interfaces;
using Core.Combat;
using Core.Player.Movement.States;
using Player.Movement;

namespace Core.Player.Movement
{
    public class MovementStateFactory
    {
        private readonly PatchyMovement movement;
        private readonly MovementConfig config;
        private readonly MovementStateMachine stateMachine;
        private readonly CombatSystem combatSystem;
        private readonly Rigidbody2D rb;

        public MovementStateFactory(PatchyMovement movement, MovementConfig config, MovementStateMachine stateMachine, CombatSystem combatSystem)
        {
            this.movement = movement;
            this.config = config;
            this.stateMachine = stateMachine;
            this.combatSystem = combatSystem;
            this.rb = movement.GetComponent<Rigidbody2D>();
        }

        public IMovementState CreateState(MovementStateMachine.MovementState state)
        {
            switch (state)
            {
                case MovementStateMachine.MovementState.Idle:
                    return new IdleState(movement, rb, config, stateMachine);
                case MovementStateMachine.MovementState.Walking:
                    return new WalkingState(movement, rb, config, stateMachine);
                case MovementStateMachine.MovementState.Jumping:
                    return new JumpingState(movement, rb, config, stateMachine);
                case MovementStateMachine.MovementState.Dashing:
                    return new DashingState(movement, rb, config, stateMachine);
                case MovementStateMachine.MovementState.Sprinting:
                    return new SprintingState(movement, rb, config, stateMachine);
                case MovementStateMachine.MovementState.Attacking:
                    return new AttackingState(movement, rb, config, stateMachine, combatSystem);
                case MovementStateMachine.MovementState.SpecialAttacking:
                    return new SpecialAttackingState(movement, rb, config, stateMachine, combatSystem);
                default:
                    Debug.LogError($"State {state} not implemented");
                    return null;
            }
        }
    }
}

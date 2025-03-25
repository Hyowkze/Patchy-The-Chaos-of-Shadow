using UnityEngine;
using Core.Combat;
using Player.Movement;
using Player.InputSystem; // Actualizar este using

namespace Core.Player.Movement.States
{
    public class AttackingState : AttackStateBase
    {
        private bool attackRequested;

        public AttackingState(PatchyMovement movement, Rigidbody2D rb, MovementConfig config, MovementStateMachine stateMachine, CombatSystem combatSystem)
            : base(movement, rb, config, combatSystem, stateMachine)
        {
        }

        public override void Enter()
        {
            attackRequested = false;
            PerformAttack();
            PlayerInputHandler.Instance.OnSpecialPowerInputChanged += HandleAttackInput; // Updated name
        }

        public override void HandleInput()
        {
            if (attackRequested)
            {
                RequestStateChange(MovementStateMachine.MovementState.SpecialAttacking);
            }
        }

        public override void Exit()
        {
            PlayerInputHandler.Instance.OnSpecialPowerInputChanged -= HandleAttackInput; // Updated name
        }

        protected override void PerformAttack()
        {
            combatSystem.PerformAttack(combatSystem.BasicAttackStrategy);
        }

        protected override void HandleAttackInput()
        {
            attackRequested = true;
        }
    }
}

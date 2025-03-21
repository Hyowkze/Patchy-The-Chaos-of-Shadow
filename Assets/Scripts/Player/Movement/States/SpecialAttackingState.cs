using UnityEngine;
using Core.Combat;
using Player.Movement;

namespace Core.Player.Movement.States
{
    public class SpecialAttackingState : AttackStateBase
    {
        public SpecialAttackingState(PatchyMovement movement, Rigidbody2D rb, MovementConfig config, MovementStateMachine stateMachine, CombatSystem combatSystem)
            : base(movement, rb, config, combatSystem, stateMachine)
        {
        }

        public override void Enter()
        {
            PerformAttack(); 
        }

        protected override void PerformAttack()
        {
            combatSystem.PerformAttack(combatSystem.SpecialAttackStrategy);
        }

        protected override void HandleAttackInput()
        {
        }
    }
}

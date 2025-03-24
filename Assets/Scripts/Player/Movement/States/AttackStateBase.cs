using UnityEngine;
using Core.Combat;
using Player.Movement;

namespace Core.Player.Movement.States
{
    public abstract class AttackStateBase : MovementStateBase
    {
        protected readonly CombatSystem combatSystem;

        protected AttackStateBase(PatchyMovement movement, Rigidbody2D rb, MovementConfig config, CombatSystem combatSystem, MovementStateMachine stateMachine)
            : base(movement, rb, config, stateMachine)
        {
            this.combatSystem = combatSystem;
        }

        protected abstract void PerformAttack();
        protected abstract void HandleAttackInput();
    }
}

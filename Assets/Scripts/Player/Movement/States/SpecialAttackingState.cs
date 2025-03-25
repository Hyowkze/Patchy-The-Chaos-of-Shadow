using UnityEngine;
using Core.Combat;
using Player.Movement;

namespace Core.Player.Movement.States
{
    public class SpecialAttackingState : AttackStateBase
    {
        private float aimTimer;
        private bool isAiming;
        private GameObject aimIndicator;
        private Vector2 lastMousePosition;

        public SpecialAttackingState(PatchyMovement movement, Rigidbody2D rb, MovementConfig config, MovementStateMachine stateMachine, CombatSystem combatSystem)
            : base(movement, rb, config, combatSystem, stateMachine)
        {
        }

        public override void Enter()
        {
            isAiming = true;
            aimTimer = 0f;
            lastMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CreateAimIndicator();
        }

        public override void Update()
        {
            if (isAiming)
            {
                aimTimer += Time.deltaTime;
                lastMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                UpdateAimIndicator();
                
                if (aimTimer >= combatSystem.AttackConfig.aimingTime)
                {
                    PerformAttack();
                    isAiming = false;
                }
            }
        }

        private void CreateAimIndicator()
        {
            if (combatSystem.AttackConfig.aimIndicatorPrefab != null)
            {
                aimIndicator = Object.Instantiate(combatSystem.AttackConfig.aimIndicatorPrefab, 
                    combatSystem.transform.position, 
                    Quaternion.identity);
                aimIndicator.layer = LayerMask.NameToLayer("UI");
            }
        }

        private void UpdateAimIndicator()
        {
            if (aimIndicator != null)
            {
                aimIndicator.transform.position = combatSystem.transform.position;
                Vector2 direction = (lastMousePosition - (Vector2)combatSystem.transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                float currentAngle = aimIndicator.transform.eulerAngles.z;
                float newAngle = Mathf.LerpAngle(currentAngle, angle, Time.deltaTime * 10f);
                aimIndicator.transform.rotation = Quaternion.Euler(0, 0, newAngle);
                
                float scaleProgress = aimTimer / combatSystem.AttackConfig.aimingTime;
                aimIndicator.transform.localScale = Vector3.one * (1f + scaleProgress * 0.5f);
            }
        }

        protected override void PerformAttack()
        {
            Vector2 finalDirection = (lastMousePosition - (Vector2)combatSystem.transform.position).normalized;
            combatSystem.PerformAttack(combatSystem.SpecialAttackStrategy);
            stateMachine.ChangeState(MovementStateMachine.MovementState.Idle);
        }

        public override void Exit()
        {
            isAiming = false;
            if (aimIndicator != null)
            {
                Object.Destroy(aimIndicator);
            }
        }

        protected override void HandleAttackInput() { }
    }
}

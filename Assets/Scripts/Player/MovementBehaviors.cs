using UnityEngine;
using Core.Player.Movement;

namespace Player.Movement
{
    public class DashBehavior : Player.Movement.IDashBehavior
    {
        private readonly DashSettings settings;
        private bool canDash = true;
        private bool isDashing;
        private float dashTimeLeft;
        private float cooldownTimeLeft;

        public bool CanDash => canDash;
        public bool IsDashing => isDashing;
        public float CooldownProgress => cooldownTimeLeft / settings.DashCooldown;

        public DashBehavior(DashSettings settings)
        {
            this.settings = settings;
        }

        public Vector2 PerformDash(Vector2 direction)
        {
            if (!canDash) return Vector2.zero;

            isDashing = true;
            canDash = false;
            dashTimeLeft = settings.DashDuration;
            cooldownTimeLeft = settings.DashCooldown;

            return direction.normalized * settings.DashSpeed;
        }

        public void UpdateDashState(float deltaTime)
        {
            if (isDashing)
            {
                dashTimeLeft -= deltaTime;
                if (dashTimeLeft <= 0)
                {
                    isDashing = false;
                }
            }
            else if (!canDash) // Combined into an else if
            {
                cooldownTimeLeft -= deltaTime;
                if (cooldownTimeLeft <= 0)
                {
                    canDash = true;
                }
            }
        }
    }

    public interface IDashBehavior
    {
        bool CanDash { get; }
        bool IsDashing { get; }
        float CooldownProgress { get; }
        Vector2 PerformDash(Vector2 direction);
        void UpdateDashState(float deltaTime);
    }

    public interface ISprintBehavior
    {
        float CalculateSprintMultiplier(float sprintInput);
    }
}

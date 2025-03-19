using UnityEngine;
using Player.Movement;

namespace Core.Player.Movement
{
    public class SprintBehavior : ISprintBehavior
    {
        private readonly SprintSettings sprintSettings;
        private float currentSprintValue;
        private float sprintEnergy = 1f;
        private const float ENERGY_DRAIN_RATE = 0.2f;
        private const float ENERGY_REGEN_RATE = 0.1f;

        public SprintBehavior(SprintSettings settings)
        {
            this.sprintSettings = settings;
        }

        public float CalculateSprintMultiplier(float sprintValue)
        {
            UpdateSprintEnergy(sprintValue > 0);
            return sprintEnergy > 0 ? 
                1f + (sprintSettings.SprintMultiplier - 1f) * sprintValue : 
                1f;
        }

        private void UpdateSprintEnergy(bool isSprinting)
        {
            if (isSprinting)
            {
                sprintEnergy = Mathf.Max(0, sprintEnergy - ENERGY_DRAIN_RATE * Time.deltaTime);
            }
            else
            {
                sprintEnergy = Mathf.Min(1, sprintEnergy + ENERGY_REGEN_RATE * Time.deltaTime);
            }
        }

        public float GetSprintEnergyPercentage() => sprintEnergy;
    }
}

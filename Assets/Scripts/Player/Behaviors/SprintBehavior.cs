using UnityEngine;
using Player.Movement;

public class SprintBehavior : ISprintBehavior
{
    private readonly SprintSettings sprintSettings;

    public SprintBehavior(SprintSettings settings)
    {
        this.sprintSettings = settings;
    }

    public float CalculateSprintMultiplier(float sprintValue)
    {
        return 1f + (sprintSettings.SprintMultiplier - 1f) * sprintValue;
    }
}

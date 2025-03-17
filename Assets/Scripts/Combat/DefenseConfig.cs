using UnityEngine;

[CreateAssetMenu(fileName = "DefenseConfig", menuName = "Combat/DefenseConfig")]
public class DefenseConfig : ScriptableObject
{
    public float defense = 5f;
    public float blockMultiplier = 0.5f;
}

public interface IDefenseStrategy
{
    float CalculateDefense(float incomingDamage);
}

public class BasicDefenseStrategy : IDefenseStrategy
{
    private readonly DefenseConfig config;

    public BasicDefenseStrategy(DefenseConfig config)
    {
        this.config = config;
    }

    public float CalculateDefense(float incomingDamage)
    {
        return Mathf.Max(0, incomingDamage - config.defense);
    }
}

public interface IAttackable
{
    void TakeDamage(float amount);
    bool IsInvulnerable { get; }
}

public interface IAttacker
{
    void PerformAttack();
    void PerformSpecialAttack();
}

public interface IDefendable
{
    float Defense { get; }
    void ApplyDefense(float incomingDamage);
}

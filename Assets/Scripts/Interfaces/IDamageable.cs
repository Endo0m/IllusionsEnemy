// Интерфейс для объектов, которые могут получать урон
public interface IDamageable
{
    void TakeDamage(float damage);
    float CurrentHealth { get; }
    float MaxHealth { get; }
}
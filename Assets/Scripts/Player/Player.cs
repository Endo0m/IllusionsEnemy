using UnityEngine;

public class Player : HealthEntity
{
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        Debug.Log($"Player took {damage} damage. Current health: {_currentHealth}");
    }

    protected override void Die()
    {
        Debug.Log("Player has died!");
        Respawn();
    }

    private void Respawn()
    {
        transform.position = RespawnManager.Instance.GetPlayerSpawnPoint();
        _currentHealth = _maxHealth;
        // Дополнительная логика респауна (например, неуязвимость на короткое время)
    }
}
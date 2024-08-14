using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HealthEntity : MonoBehaviour, IDamageable
{
    [SerializeField] protected float _maxHealth = 100f; // максимум жизни
    protected float _currentHealth; // текущие жизни

    public float CurrentHealth => _currentHealth;
    public float MaxHealth => _maxHealth;

    protected virtual void Start()
    {
        _currentHealth = _maxHealth;
    }

    public virtual void TakeDamage(float damage) // функция урона
    {
        _currentHealth -= damage; // текущие жизни минус урон
        if (_currentHealth <= 0)
        {
            Die(); // смерть
        }
    }

    protected abstract void Die();
}
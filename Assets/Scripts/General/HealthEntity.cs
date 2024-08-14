using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HealthEntity : MonoBehaviour, IDamageable
{
    [SerializeField] protected float _maxHealth = 100f; // �������� �����
    protected float _currentHealth; // ������� �����

    public float CurrentHealth => _currentHealth;
    public float MaxHealth => _maxHealth;

    protected virtual void Start()
    {
        _currentHealth = _maxHealth;
    }

    public virtual void TakeDamage(float damage) // ������� �����
    {
        _currentHealth -= damage; // ������� ����� ����� ����
        if (_currentHealth <= 0)
        {
            Die(); // ������
        }
    }

    protected abstract void Die();
}
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ������������ ��������� ����� � ����.
/// </summary>
public class FlyingEnemy : HealthEntity
{
    [SerializeField] private float _moveSpeed = 5f; // �������� �������� �����
    [SerializeField] private float _retreatRange = 15f; // ����������, �� ������� ���� �������� ���������
    [SerializeField] private float _chaseRange = 30f; // ����������, �� ������� ���� �������� ������������ ������
    [SerializeField] private float _minHeight = 1f; // ����������� ������ ��� ������
    [SerializeField] private float _maxHeight = 20f; // ������������ ������ ������
    [SerializeField] private float _attackCooldown = 6f; // ����� ����������� �����
    [SerializeField] private float _illusionCooldown = 20f; // ����� ����������� �������� �������
    [SerializeField] private GameObject _illusionPrefab; // ������ �������
    [SerializeField] private int _illusionCount = 2; // ���������� ����������� �������
    [SerializeField] private LaserShooter _laserShooter; // ��������� ��� �������� �������

    private Transform _playerTransform;
    private float _lastAttackTime;
    private float _lastIllusionTime;
    private List<FlyingEnemyIllusion> _activeIllusions = new List<FlyingEnemyIllusion>();

    /// <summary>
    /// �������������� �������� ����� � ������� ������.
    /// </summary>
    protected override void Start()
    {
        base.Start();
        _playerTransform = GameObject.FindGameObjectWithTag("PlayerBody").transform;
        _lastAttackTime = -_attackCooldown;
        _lastIllusionTime = -_illusionCooldown;
    }

    /// <summary>
    /// ��������� ��������� ����� ������ ����.
    /// </summary>
    private void Update()
    {
        MoveRelativeToPlayer();
        TryAttack();
        TryCreateIllusions();
    }

    /// <summary>
    /// ������� ����� ������������ ������� ������.
    /// </summary>
    private void MoveRelativeToPlayer()
    {
        Vector3 directionToPlayer = _playerTransform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        Vector3 targetPosition = transform.position;

        if (distanceToPlayer < _retreatRange)
        {
            // ���������
            targetPosition -= directionToPlayer.normalized * _moveSpeed * Time.deltaTime;
        }
        else if (distanceToPlayer > _chaseRange)
        {
            // ����������
            targetPosition += directionToPlayer.normalized * _moveSpeed * Time.deltaTime;
        }

        // ������������ ������
        float groundHeight = GetGroundHeight();
        targetPosition.y = Mathf.Clamp(targetPosition.y, groundHeight + _minHeight, groundHeight + _maxHeight);

        transform.position = targetPosition;
    }

    /// <summary>
    /// �������� ������ ����� ��� ������.
    /// </summary>
    /// <returns>Y-���������� �����.</returns>
    private float GetGroundHeight()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity))
        {
            return hit.point.y;
        }
        return 0f;
    }

    /// <summary>
    /// �������� ��������� ������, ���� ����������� ���������.
    /// </summary>
    private void TryAttack()
    {
        if (Time.time - _lastAttackTime >= _attackCooldown)
        {
            Attack();
            _lastAttackTime = Time.time;
        }
    }

    /// <summary>
    /// ��������� ����� �� ������.
    /// </summary>
    private void Attack()
    {
        Vector3 directionToPlayer = (_playerTransform.position - transform.position).normalized;
        Vector3 targetPoint = _playerTransform.position;
        _laserShooter.Shoot(transform.position, targetPoint);
    }

    /// <summary>
    /// �������� ������� �������, ���� ����������� ��������� � ��� �������� �������.
    /// </summary>
    private void TryCreateIllusions()
    {
        if (Time.time - _lastIllusionTime >= _illusionCooldown && _activeIllusions.Count == 0)
        {
            CreateIllusions();
        }
    }

    /// <summary>
    /// ������� ������� �����.
    /// </summary>
    private void CreateIllusions()
    {
        for (int i = 0; i < _illusionCount; i++)
        {
            Vector3 randomPosition = transform.position + Random.insideUnitSphere * 3f;
            randomPosition.y = Mathf.Clamp(randomPosition.y, GetGroundHeight() + _minHeight, GetGroundHeight() + _maxHeight);
            GameObject illusionObj = Instantiate(_illusionPrefab, randomPosition, Quaternion.identity);
            FlyingEnemyIllusion illusion = illusionObj.GetComponent<FlyingEnemyIllusion>();
            illusion.Initialize(this);
            _activeIllusions.Add(illusion);
        }
        _lastIllusionTime = Time.time;
    }

    /// <summary>
    /// ������� ������� �� ������ �������� �������.
    /// </summary>
    /// <param name="illusion">������� ��� ��������.</param>
    public void RemoveIllusion(FlyingEnemyIllusion illusion)
    {
        _activeIllusions.Remove(illusion);
        if (_activeIllusions.Count == 0)
        {
            _lastIllusionTime = Time.time;
        }
    }

    /// <summary>
    /// ������������ ������ �����.
    /// </summary>
    protected override void Die()
    {
        foreach (var illusion in _activeIllusions)
        {
            Destroy(illusion.gameObject);
        }
        _activeIllusions.Clear();
        Respawn();
    }

    /// <summary>
    /// ���������� ����� � ��������� ����� �����������.
    /// </summary>
    private void Respawn()
    {
        transform.position = RespawnManager.Instance.GetRandomEnemySpawnPoint();
        _currentHealth = _maxHealth;
        // ���������������� ��������� ������, ����� ��������� ����������� �������� ������� ����� �����������
        // _lastIllusionTime = Time.time - _illusionCooldown;
    }
}
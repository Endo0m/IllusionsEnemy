using UnityEngine;

/// <summary>
/// ������������ ������� ��������� ����� � ����.
/// </summary>
public class FlyingEnemyIllusion : HealthEntity
{
    [SerializeField] private float _moveSpeed = 4f; // �������� �������� �������
    [SerializeField] private float _retreatRange = 10f; // ���������, �� ������� ������� �������� ���������
    [SerializeField] private float _chaseRange = 25f; // ���������, �� ������� ������� �������� ������������ ������
    [SerializeField] private float _minHeight = 1f; // ����������� ������ ��� ������
    [SerializeField] private float _maxHeight = 15f; // ������������ ������ ������
    [SerializeField] private float _attackCooldown = 7f; // ����� ����������� �����
    [SerializeField] private LaserShooter _laserShooter; // ��������� ��� �������� �������

    private FlyingEnemy _originalEnemy; // ������ �� ������������� �����
    private Transform _playerTransform; // ��������� ������
    private float _lastAttackTime; // ����� ��������� �����

    /// <summary>
    /// �������������� ������� ������� ������������� �����.
    /// </summary>
    /// <param name="enemy">������������ �������� ����</param>
    public void Initialize(FlyingEnemy enemy)
    {
        _originalEnemy = enemy;
        _laserShooter.SetDamage(_laserShooter.Damage / 2);

        // ������ ������� ��������� �������
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Color currentColor = renderer.material.color;
            renderer.material.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0.5f);
        }
    }

    /// <summary>
    /// ������������� ��� ������.
    /// </summary>
    protected override void Start()
    {
        base.Start();
        _playerTransform = GameObject.FindGameObjectWithTag("PlayerBody").transform;
        _lastAttackTime = -_attackCooldown; // ��������� ��������� ����� ����� ��������
    }

    /// <summary>
    /// ���������� ��������� ������� ������ ����.
    /// </summary>
    private void Update()
    {
        MoveRelativeToPlayer();
        TryAttack();
    }

    /// <summary>
    /// ���������� ������� ������������ ������.
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
    /// �������� ������ ����� ��� ��������.
    /// </summary>
    /// <returns>Y-���������� ����������� �����</returns>
    private float GetGroundHeight()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity))
        {
            return hit.point.y;
        }
        return 0f;
    }

    /// <summary>
    /// �������� ���������, ���� ����� ����������� �������.
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
    /// ������������ ������ �������.
    /// </summary>
    protected override void Die()
    {
        _originalEnemy.RemoveIllusion(this);
        Destroy(gameObject);
    }
}
using UnityEngine;

/// <summary>
/// Представляет иллюзию летающего врага в игре.
/// </summary>
public class FlyingEnemyIllusion : HealthEntity
{
    [SerializeField] private float _moveSpeed = 4f; // Скорость движения иллюзии
    [SerializeField] private float _retreatRange = 10f; // Дистанция, на которой иллюзия начинает отступать
    [SerializeField] private float _chaseRange = 25f; // Дистанция, на которой иллюзия начинает преследовать игрока
    [SerializeField] private float _minHeight = 1f; // Минимальная высота над землей
    [SerializeField] private float _maxHeight = 15f; // Максимальная высота полета
    [SerializeField] private float _attackCooldown = 7f; // Время перезарядки атаки
    [SerializeField] private LaserShooter _laserShooter; // Компонент для стрельбы лазером

    private FlyingEnemy _originalEnemy; // Ссылка на оригинального врага
    private Transform _playerTransform; // Трансформ игрока
    private float _lastAttackTime; // Время последней атаки

    /// <summary>
    /// Инициализирует иллюзию данными оригинального врага.
    /// </summary>
    /// <param name="enemy">Оригинальный летающий враг</param>
    public void Initialize(FlyingEnemy enemy)
    {
        _originalEnemy = enemy;
        _laserShooter.SetDamage(_laserShooter.Damage / 2);

        // Делаем иллюзию визуально тусклее
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Color currentColor = renderer.material.color;
            renderer.material.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0.5f);
        }
    }

    /// <summary>
    /// Инициализация при старте.
    /// </summary>
    protected override void Start()
    {
        base.Start();
        _playerTransform = GameObject.FindGameObjectWithTag("PlayerBody").transform;
        _lastAttackTime = -_attackCooldown; // Позволяет атаковать сразу после создания
    }

    /// <summary>
    /// Обновление поведения иллюзии каждый кадр.
    /// </summary>
    private void Update()
    {
        MoveRelativeToPlayer();
        TryAttack();
    }

    /// <summary>
    /// Перемещает иллюзию относительно игрока.
    /// </summary>
    private void MoveRelativeToPlayer()
    {
        Vector3 directionToPlayer = _playerTransform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        Vector3 targetPosition = transform.position;

        if (distanceToPlayer < _retreatRange)
        {
            // Отступаем
            targetPosition -= directionToPlayer.normalized * _moveSpeed * Time.deltaTime;
        }
        else if (distanceToPlayer > _chaseRange)
        {
            // Преследуем
            targetPosition += directionToPlayer.normalized * _moveSpeed * Time.deltaTime;
        }

        // Ограничиваем высоту
        float groundHeight = GetGroundHeight();
        targetPosition.y = Mathf.Clamp(targetPosition.y, groundHeight + _minHeight, groundHeight + _maxHeight);
        transform.position = targetPosition;
    }

    /// <summary>
    /// Получает высоту земли под иллюзией.
    /// </summary>
    /// <returns>Y-координата поверхности земли</returns>
    private float GetGroundHeight()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity))
        {
            return hit.point.y;
        }
        return 0f;
    }

    /// <summary>
    /// Пытается атаковать, если время перезарядки истекло.
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
    /// Выполняет атаку на игрока.
    /// </summary>
    private void Attack()
    {
        Vector3 directionToPlayer = (_playerTransform.position - transform.position).normalized;
        Vector3 targetPoint = _playerTransform.position;
        _laserShooter.Shoot(transform.position, targetPoint);
    }

    /// <summary>
    /// Обрабатывает смерть иллюзии.
    /// </summary>
    protected override void Die()
    {
        _originalEnemy.RemoveIllusion(this);
        Destroy(gameObject);
    }
}
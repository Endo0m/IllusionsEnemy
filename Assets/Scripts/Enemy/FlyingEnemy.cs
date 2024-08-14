using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Представляет летающего врага в игре.
/// </summary>
public class FlyingEnemy : HealthEntity
{
    [SerializeField] private float _moveSpeed = 5f; // Скорость движения врага
    [SerializeField] private float _retreatRange = 15f; // Расстояние, на котором враг начинает отступать
    [SerializeField] private float _chaseRange = 30f; // Расстояние, на котором враг начинает преследовать игрока
    [SerializeField] private float _minHeight = 1f; // Минимальная высота над землей
    [SerializeField] private float _maxHeight = 20f; // Максимальная высота полета
    [SerializeField] private float _attackCooldown = 6f; // Время перезарядки атаки
    [SerializeField] private float _illusionCooldown = 20f; // Время перезарядки создания иллюзий
    [SerializeField] private GameObject _illusionPrefab; // Префаб иллюзии
    [SerializeField] private int _illusionCount = 2; // Количество создаваемых иллюзий
    [SerializeField] private LaserShooter _laserShooter; // Компонент для стрельбы лазером

    private Transform _playerTransform;
    private float _lastAttackTime;
    private float _lastIllusionTime;
    private List<FlyingEnemyIllusion> _activeIllusions = new List<FlyingEnemyIllusion>();

    /// <summary>
    /// Инициализирует свойства врага и находит игрока.
    /// </summary>
    protected override void Start()
    {
        base.Start();
        _playerTransform = GameObject.FindGameObjectWithTag("PlayerBody").transform;
        _lastAttackTime = -_attackCooldown;
        _lastIllusionTime = -_illusionCooldown;
    }

    /// <summary>
    /// Обновляет поведение врага каждый кадр.
    /// </summary>
    private void Update()
    {
        MoveRelativeToPlayer();
        TryAttack();
        TryCreateIllusions();
    }

    /// <summary>
    /// Двигает врага относительно позиции игрока.
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
    /// Получает высоту земли под врагом.
    /// </summary>
    /// <returns>Y-координата земли.</returns>
    private float GetGroundHeight()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity))
        {
            return hit.point.y;
        }
        return 0f;
    }

    /// <summary>
    /// Пытается атаковать игрока, если перезарядка завершена.
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
    /// Пытается создать иллюзии, если перезарядка завершена и нет активных иллюзий.
    /// </summary>
    private void TryCreateIllusions()
    {
        if (Time.time - _lastIllusionTime >= _illusionCooldown && _activeIllusions.Count == 0)
        {
            CreateIllusions();
        }
    }

    /// <summary>
    /// Создает иллюзии врага.
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
    /// Удаляет иллюзию из списка активных иллюзий.
    /// </summary>
    /// <param name="illusion">Иллюзия для удаления.</param>
    public void RemoveIllusion(FlyingEnemyIllusion illusion)
    {
        _activeIllusions.Remove(illusion);
        if (_activeIllusions.Count == 0)
        {
            _lastIllusionTime = Time.time;
        }
    }

    /// <summary>
    /// Обрабатывает смерть врага.
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
    /// Возрождает врага в случайной точке возрождения.
    /// </summary>
    private void Respawn()
    {
        transform.position = RespawnManager.Instance.GetRandomEnemySpawnPoint();
        _currentHealth = _maxHealth;
        // Раскомментируйте следующую строку, чтобы разрешить немедленное создание иллюзий после возрождения
        // _lastIllusionTime = Time.time - _illusionCooldown;
    }
}
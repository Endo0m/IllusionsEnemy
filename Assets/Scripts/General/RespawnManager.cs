using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Менеджер точек возрождения для игрока и врагов.
/// </summary>
public class RespawnManager : MonoBehaviour
{
    [SerializeField] private Transform _playerSpawnPoint; // Точка возрождения игрока
    [SerializeField] private List<Transform> _enemySpawnPoints; // Список точек возрождения врагов

    private static RespawnManager _instance;
    public static RespawnManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    /// <summary>
    /// Возвращает позицию точки возрождения игрока.
    /// </summary>
    /// <returns>Позиция точки возрождения игрока.</returns>
    public Vector3 GetPlayerSpawnPoint()
    {
        return _playerSpawnPoint.position;
    }

    /// <summary>
    /// Возвращает случайную точку возрождения врага.
    /// </summary>
    /// <returns>Позиция случайной точки возрождения врага.</returns>
    public Vector3 GetRandomEnemySpawnPoint()
    {
        if (_enemySpawnPoints.Count > 0)
        {
            int randomIndex = Random.Range(0, _enemySpawnPoints.Count);
            return _enemySpawnPoints[randomIndex].position;
        }
        return Vector3.zero;
    }
}
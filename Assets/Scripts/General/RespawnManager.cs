using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �������� ����� ����������� ��� ������ � ������.
/// </summary>
public class RespawnManager : MonoBehaviour
{
    [SerializeField] private Transform _playerSpawnPoint; // ����� ����������� ������
    [SerializeField] private List<Transform> _enemySpawnPoints; // ������ ����� ����������� ������

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
    /// ���������� ������� ����� ����������� ������.
    /// </summary>
    /// <returns>������� ����� ����������� ������.</returns>
    public Vector3 GetPlayerSpawnPoint()
    {
        return _playerSpawnPoint.position;
    }

    /// <summary>
    /// ���������� ��������� ����� ����������� �����.
    /// </summary>
    /// <returns>������� ��������� ����� ����������� �����.</returns>
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
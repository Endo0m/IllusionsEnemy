using UnityEngine;

/// <summary>
/// Поведение объекта, следящего за игроком.
/// </summary>
public class LookAtPlayerBehavior : MonoBehaviour, ILookAtPlayer
{
    [SerializeField] private float _rotationSpeed = 5f; // Скорость поворота
    [SerializeField] private Vector3 _rotationOffset = new Vector3(0, 180, 0); // Смещение поворота

    private Transform _playerTransform; // Трансформ игрока

    private void Start()
    {
        FindPlayer();
    }

    private void Update()
    {
        if (_playerTransform == null)
        {
            FindPlayer();
        }
        else
        {
            LookAtPlayer(_playerTransform);
        }
    }

    /// <summary>
    /// Ищет игрока на сцене.
    /// </summary>
    private void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerTransform = player.transform;
        }
    }

    /// <summary>
    /// Поворачивает объект в сторону игрока.
    /// </summary>
    /// <param name="target">Трансформ цели (игрока).</param>
    public void LookAtPlayer(Transform target)
    {
        if (target != null)
        {
            Vector3 directionToPlayer = target.position - transform.position;
            directionToPlayer.y = 0; // Игнорируем вертикальное вращение

            if (directionToPlayer != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer) * Quaternion.Euler(_rotationOffset);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 forward = transform.rotation * Quaternion.Euler(-_rotationOffset) * Vector3.forward;
        Gizmos.DrawRay(transform.position, forward * 3f);
    }
}
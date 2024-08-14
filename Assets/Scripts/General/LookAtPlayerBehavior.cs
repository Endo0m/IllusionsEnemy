using UnityEngine;

/// <summary>
/// ��������� �������, ��������� �� �������.
/// </summary>
public class LookAtPlayerBehavior : MonoBehaviour, ILookAtPlayer
{
    [SerializeField] private float _rotationSpeed = 5f; // �������� ��������
    [SerializeField] private Vector3 _rotationOffset = new Vector3(0, 180, 0); // �������� ��������

    private Transform _playerTransform; // ��������� ������

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
    /// ���� ������ �� �����.
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
    /// ������������ ������ � ������� ������.
    /// </summary>
    /// <param name="target">��������� ���� (������).</param>
    public void LookAtPlayer(Transform target)
    {
        if (target != null)
        {
            Vector3 directionToPlayer = target.position - transform.position;
            directionToPlayer.y = 0; // ���������� ������������ ��������

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
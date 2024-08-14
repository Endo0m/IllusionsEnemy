using System.Collections;
using UnityEngine;

/// <summary>
/// ��������� ��� �������� �������.
/// </summary>
public class LaserShooter : MonoBehaviour
{
    [SerializeField] private float _damage = 10f; // ���� �� ������
    [SerializeField] private float _laserDuration = 0.5f; // ������������ ����������� ������
    [SerializeField] private LineRenderer _laserLine; // ��������� ��� ��������� ������
    [SerializeField] private LayerMask _targetLayer; // ���� ���� (������)

    public float Damage { get => _damage; private set => _damage = value; }

    /// <summary>
    /// ������������� ����� �������� �����.
    /// </summary>
    /// <param name="newDamage">����� �������� �����.</param>
    public void SetDamage(float newDamage)
    {
        Damage = Mathf.Max(0, newDamage);
    }

    private void Awake()
    {
        if (_laserLine == null)
        {
            _laserLine = GetComponent<LineRenderer>();
        }
    }

    /// <summary>
    /// ���������� ������� �������.
    /// </summary>
    /// <param name="startPoint">��������� ����� ������.</param>
    /// <param name="targetPoint">�������� ����� ������.</param>
    public void Shoot(Vector3 startPoint, Vector3 targetPoint)
    {
        StartCoroutine(ShootCoroutine(startPoint, targetPoint));
    }

    private IEnumerator ShootCoroutine(Vector3 startPoint, Vector3 targetPoint)
    {
        gameObject.SetActive(true);

        Vector3 direction = (targetPoint - startPoint).normalized;
        float distance = Vector3.Distance(startPoint, targetPoint);

        _laserLine.SetPosition(0, startPoint);
        _laserLine.SetPosition(1, targetPoint);
        _laserLine.enabled = true;

        RaycastHit[] hits = Physics.RaycastAll(startPoint, direction, distance, _targetLayer);
        foreach (RaycastHit hit in hits)
        {
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(Damage);
                Debug.Log($"����� ����� � ������ � ����� {Damage} �����");
                break;
            }
        }

        yield return new WaitForSeconds(_laserDuration);

        _laserLine.enabled = false;
    }
}
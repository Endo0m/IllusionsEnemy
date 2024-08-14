using System.Collections;
using UnityEngine;

/// <summary>
/// Компонент для стрельбы лазером.
/// </summary>
public class LaserShooter : MonoBehaviour
{
    [SerializeField] private float _damage = 10f; // Урон от лазера
    [SerializeField] private float _laserDuration = 0.5f; // Длительность отображения лазера
    [SerializeField] private LineRenderer _laserLine; // Компонент для отрисовки лазера
    [SerializeField] private LayerMask _targetLayer; // Слой цели (игрока)

    public float Damage { get => _damage; private set => _damage = value; }

    /// <summary>
    /// Устанавливает новое значение урона.
    /// </summary>
    /// <param name="newDamage">Новое значение урона.</param>
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
    /// Производит выстрел лазером.
    /// </summary>
    /// <param name="startPoint">Начальная точка лазера.</param>
    /// <param name="targetPoint">Конечная точка лазера.</param>
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
                Debug.Log($"Лазер попал в игрока и нанес {Damage} урона");
                break;
            }
        }

        yield return new WaitForSeconds(_laserDuration);

        _laserLine.enabled = false;
    }
}
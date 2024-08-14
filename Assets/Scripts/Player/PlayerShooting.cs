using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Контроллер стрельбы игрока.
/// </summary>
public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private LaserShooter _laserShooter; // Компонент для стрельбы лазером
    [SerializeField] private Transform _eyePosition; // Позиция "глаз" игрока, откуда производится выстрел
    [SerializeField] private float _maxShootDistance = 100f; // Максимальная дистанция стрельбы
    [SerializeField] private LayerMask _shootableLayers; // Слои, по которым можно стрелять
    [SerializeField] private string _fireButtonName = "Fire1"; // Имя кнопки для стрельбы
    [SerializeField] private Sprite _crosshairSprite; // Спрайт прицела
    [SerializeField] private float _crosshairSize = 64f; // Размер прицела

    private Camera _mainCamera; // Основная камера
    private Image _crosshairImage; // Изображение прицела

    private void Start()
    {
        _mainCamera = Camera.main;

        if (_eyePosition == null)
        {
            Debug.LogWarning("Позиция глаз не установлена. Используется позиция игрока.");
            _eyePosition = transform;
        }

        CreateCrosshair();
    }

    /// <summary>
    /// Создает прицел на экране.
    /// </summary>
    private void CreateCrosshair()
    {
        GameObject crosshairObject = new GameObject("Crosshair");
        _crosshairImage = crosshairObject.AddComponent<Image>();
        _crosshairImage.sprite = _crosshairSprite;
        RectTransform rectTransform = _crosshairImage.rectTransform;

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            canvas = new GameObject("Canvas").AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.gameObject.AddComponent<CanvasScaler>();
        }
        rectTransform.SetParent(canvas.transform, false);
        rectTransform.sizeDelta = new Vector2(_crosshairSize, _crosshairSize);
        rectTransform.anchoredPosition = Vector2.zero;
    }

    private void Update()
    {
        if (Input.GetButtonDown(_fireButtonName))
        {
            Vector3 aimPoint = GetAimPoint();
            Shoot(aimPoint);
        }
    }

    /// <summary>
    /// Получает точку прицеливания.
    /// </summary>
    /// <returns>Позиция точки прицеливания в мировых координатах.</returns>
    private Vector3 GetAimPoint()
    {
        Ray ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _maxShootDistance, _shootableLayers))
        {
            return hit.point;
        }

        return ray.GetPoint(_maxShootDistance);
    }

    /// <summary>
    /// Производит выстрел в указанную точку.
    /// </summary>
    /// <param name="aimPoint">Точка прицеливания.</param>
    private void Shoot(Vector3 aimPoint)
    {
        Vector3 shootDirection = (aimPoint - _eyePosition.position).normalized;

        RotatePlayerInstantly(shootDirection);

        RaycastHit hit;
        if (Physics.Raycast(_eyePosition.position, shootDirection, out hit, _maxShootDistance, _shootableLayers))
        {
            if (hit.collider.gameObject != gameObject)
            {
                _laserShooter.Shoot(_eyePosition.position, hit.point);
            }
            else
            {
                Debug.Log("Нельзя стрелять сквозь себя!");
            }
        }
        else
        {
            _laserShooter.Shoot(_eyePosition.position, aimPoint);
        }
    }

    /// <summary>
    /// Мгновенно поворачивает игрока в указанном направлении.
    /// </summary>
    /// <param name="direction">Направление поворота.</param>
    private void RotatePlayerInstantly(Vector3 direction)
    {
        direction.y = 0; // Игнорируем вертикальное вращение
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
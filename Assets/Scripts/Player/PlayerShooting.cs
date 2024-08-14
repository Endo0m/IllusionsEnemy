using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���������� �������� ������.
/// </summary>
public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private LaserShooter _laserShooter; // ��������� ��� �������� �������
    [SerializeField] private Transform _eyePosition; // ������� "����" ������, ������ ������������ �������
    [SerializeField] private float _maxShootDistance = 100f; // ������������ ��������� ��������
    [SerializeField] private LayerMask _shootableLayers; // ����, �� ������� ����� ��������
    [SerializeField] private string _fireButtonName = "Fire1"; // ��� ������ ��� ��������
    [SerializeField] private Sprite _crosshairSprite; // ������ �������
    [SerializeField] private float _crosshairSize = 64f; // ������ �������

    private Camera _mainCamera; // �������� ������
    private Image _crosshairImage; // ����������� �������

    private void Start()
    {
        _mainCamera = Camera.main;

        if (_eyePosition == null)
        {
            Debug.LogWarning("������� ���� �� �����������. ������������ ������� ������.");
            _eyePosition = transform;
        }

        CreateCrosshair();
    }

    /// <summary>
    /// ������� ������ �� ������.
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
    /// �������� ����� ������������.
    /// </summary>
    /// <returns>������� ����� ������������ � ������� �����������.</returns>
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
    /// ���������� ������� � ��������� �����.
    /// </summary>
    /// <param name="aimPoint">����� ������������.</param>
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
                Debug.Log("������ �������� ������ ����!");
            }
        }
        else
        {
            _laserShooter.Shoot(_eyePosition.position, aimPoint);
        }
    }

    /// <summary>
    /// ��������� ������������ ������ � ��������� �����������.
    /// </summary>
    /// <param name="direction">����������� ��������.</param>
    private void RotatePlayerInstantly(Vector3 direction)
    {
        direction.y = 0; // ���������� ������������ ��������
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
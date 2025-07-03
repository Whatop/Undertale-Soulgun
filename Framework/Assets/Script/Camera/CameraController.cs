using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachinePixelPerfect))]
public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    [Header("Virtual Cameras")]
    [SerializeField] private CinemachineVirtualCamera mainCamera;
    [SerializeField] private CinemachineVirtualCamera battleCamera;

    [Header("Confiner Settings")]
    // ����: CinemachineConfiner2D �� CinemachineConfiner
    [SerializeField] private CinemachineConfiner confiner;
    [SerializeField] private List<PolygonCollider2D> roomBounds;
    [SerializeField] private Transform player;

    [Header("Shake Settings")]
    public float shakeAmount = 0.1f;
    public float shakeDuration = 0.5f;

    private GameManager gameManager;
    private CinemachineBrain cinemachineBrain;
    private CinemachinePixelPerfect pixelPerfectCamera;
    private float shakeTimer;
    private Vector3 originalPosition;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
        pixelPerfectCamera = GetComponent<CinemachinePixelPerfect>();
    }

    private void Start()
    {
        gameManager = GameManager.Instance;

        if (mainCamera != null && player != null)
            mainCamera.Follow = player;
    }

    private void Update()
    {
        HandleShake();
        UpdateCameraSettings();
    }

    private void HandleShake()
    {
        if (shakeTimer > 0f)
        {
            transform.position = originalPosition + Random.insideUnitSphere * shakeAmount;
            shakeTimer -= Time.unscaledDeltaTime;
        }
        else
        {
            transform.position = originalPosition;
        }
    }

    private void UpdateCameraSettings()
    {
        bool isBattle = gameManager.isBattle;

        // Pixel Perfect Ȱ��/��Ȱ��
        if (pixelPerfectCamera != null)
            pixelPerfectCamera.enabled = isBattle;

        // Orthographic Size ����
        var activeCam = cinemachineBrain.ActiveVirtualCamera as CinemachineVirtualCamera;
        if (activeCam != null && (pixelPerfectCamera == null || !pixelPerfectCamera.enabled))
        {
            float targetSize = isBattle ? 6f : 6f; // �ʿ� �� �� ����
            if (Mathf.Abs(activeCam.m_Lens.OrthographicSize - targetSize) > 0.01f)
            {
                activeCam.m_Lens.OrthographicSize = Mathf.Lerp(
                    activeCam.m_Lens.OrthographicSize,
                    targetSize,
                    8f * Time.deltaTime
                );
            }
        }

        // ��Ʋ ī�޶� �켱���� ��ȯ
        if (battleCamera != null && mainCamera != null)
            battleCamera.Priority = isBattle ? mainCamera.Priority + 1 : mainCamera.Priority - 1;
    }

    /// <summary>
    /// �� ��ȯ �� Confiner ��ü
    /// </summary>
    public void SwitchRoomConfiner(int roomIndex)
    {
        if (confiner == null || roomIndex < 0 || roomIndex >= roomBounds.Count)
        {
            Debug.LogWarning($"SwitchRoomConfiner: �߸��� �ε��� {roomIndex}");
            return;
        }

        // ����: CinemachineConfiner ������ �����ϰ� BoundingShape�� Collider2D �Ҵ�
        confiner.m_BoundingShape2D = roomBounds[roomIndex];
        confiner.InvalidatePathCache();
    }

    /// <summary>
    /// ī�޶� ���� Ʈ����
    /// </summary>
    public void ShakeCamera(float duration = -1f)
    {
        originalPosition = transform.position;
        shakeTimer = duration > 0f ? duration : shakeDuration;
    }
}

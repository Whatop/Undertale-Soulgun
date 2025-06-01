using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachinePixelPerfect))]
public class CameraController : MonoBehaviour
{
    private GameManager gameManager;
    private CinemachineBrain cinemachineBrain;
    private CinemachinePixelPerfect pixelPerfectCamera; // Pixel Perfect Camera

    public CinemachineVirtualCamera[] virtualCameras;
    public CinemachineVirtualCamera virtualBattleCamera;

    public float shakeAmount = 0.1f;  // ��鸲 ����
    public float shakeDuration = 0.5f;  // ��鸲 ���� �ð�
    private float shakeTimer = 0f;  // ��鸲 Ÿ�̸�
    private Vector3 originalPosition;  // ���� ī�޶� ��ġ

    private static CameraController instance;

    // �̱��� ������ ���� ������Ƽ
    public static CameraController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CameraController>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("CameraController");
                    instance = obj.AddComponent<CameraController>();
                }
            }
            return instance;
        }
    }
    private void Awake()
    {
        cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>(); // ���� ī�޶� ���� CinemachineBrain ��������
        pixelPerfectCamera = GetComponent<CinemachinePixelPerfect>(); // CinemachinePixelPerfect ������Ʈ ��������
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
 
    }
    void Update()
    {
        if (shakeTimer > 0)
        {
            // ��鸲�� ���� ���� ���
            transform.position = originalPosition + Random.insideUnitSphere * shakeAmount;
            shakeTimer -= Time.deltaTime;
        }
        else
        {
            // ��鸲�� �������� ���� ��ġ�� ���ư�����
            transform.position = originalPosition;
        }

        UpdateCameraSize();
    }


    void UpdateCameraSize()
    {
        // ���� Ȱ��ȭ�� ���� ī�޶� ��������
        CinemachineVirtualCamera activeVirtualCamera = cinemachineBrain.ActiveVirtualCamera as CinemachineVirtualCamera;

        if (activeVirtualCamera != null)
        {
            // ��Ʋ ���ο� ���� Pixel Perfect Ȱ��ȭ ���θ� ����
            if (pixelPerfectCamera != null)
            {
                pixelPerfectCamera.enabled = gameManager.isBattle; // ��Ʋ �߿��� Pixel Perfect Ȱ��ȭ
            }

            // Pixel Perfect�� ��Ȱ��ȭ�� ��쿡�� ī�޶� ũ�� ����
            if (pixelPerfectCamera == null || !pixelPerfectCamera.enabled)
            {
                float targetCameraSize = gameManager.isBattle ? 6 : 6; // �ʿ��� ��� ũ�⸦ ����
                if (Mathf.Abs(activeVirtualCamera.m_Lens.OrthographicSize - targetCameraSize) > 0.01f)
                {
                    activeVirtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(
                        activeVirtualCamera.m_Lens.OrthographicSize,
                        targetCameraSize,
                        8f * Time.deltaTime
                    );
                }
            }
        }
        else
        {
            Debug.LogWarning("Ȱ��ȭ�� CinemachineVirtualCamera�� �����ϴ�.");
        }

        // ī�޶� �켱���� ����
        if (gameManager.isBattle)
        {
            virtualBattleCamera.Priority = 11;
        }
        else
        {
            virtualBattleCamera.Priority = 6;
        }
    }
    public void ShakeCamera()
    {
        originalPosition = transform.position;  // ���� ��ġ ����
        shakeTimer = shakeDuration;  // ��鸲 ���� �ð� ����
    }

}

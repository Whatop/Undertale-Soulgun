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

    private void Awake()
    {
        gameManager = GameManager.Instance;
        cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>(); // ���� ī�޶� ���� CinemachineBrain ��������
        pixelPerfectCamera = GetComponent<CinemachinePixelPerfect>(); // CinemachinePixelPerfect ������Ʈ ��������
    }

    private void Update()
    {
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

}

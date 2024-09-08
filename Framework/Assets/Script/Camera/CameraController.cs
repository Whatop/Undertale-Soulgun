using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    private GameManager gameManager;

    // ���� ���� ī�޶� �迭�� ����������, ���� �������� ����
    public CinemachineVirtualCamera[] virtualCameras;

    private void Awake()
    {
        gameManager = GameManager.Instance;
    }

    private void Update()
    {
        UpdateCameraSize();
    }

    void UpdateCameraSize()
    {
        foreach (var virtualCamera in virtualCameras)
        {
            if (virtualCamera == null)
            {
                Debug.LogWarning("CinemachineVirtualCamera�� CameraController�� �Ҵ���� �ʾҽ��ϴ�.");
                continue;
            }

            // 2D ī�޶��� OrthographicSize�� ����
            float targetCameraSize = gameManager.isBattle ? 10 : 6; // ���� ���� ���� �⺻ ������ ī�޶� ũ��
            if (Mathf.Abs(virtualCamera.m_Lens.OrthographicSize - targetCameraSize) > 0.01f )
            {
                
                virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(virtualCamera.m_Lens.OrthographicSize, targetCameraSize, 8f * Time.deltaTime);
            }
        }
    }
}

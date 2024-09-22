using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    private GameManager gameManager;
    private CinemachineBrain cinemachineBrain;

    public CinemachineVirtualCamera[] virtualCameras;
    public CinemachineVirtualCamera virtualBattleCamera;

    private void Awake()
    {
        gameManager = GameManager.Instance;
        cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>(); // ���� ī�޶� ���� CinemachineBrain ������Ʈ ��������
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
            float targetCameraSize = gameManager.isBattle ? 6 : 6;

            // ī�޶� ũ�Ⱑ �ʹ� �۰� �������� �ʵ��� ����
            if (Mathf.Abs(activeVirtualCamera.m_Lens.OrthographicSize - targetCameraSize) > 0.01f)
            {
             activeVirtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(activeVirtualCamera.m_Lens.OrthographicSize, targetCameraSize, 8f * Time.deltaTime);
            }
        }
        else
        {
            Debug.LogWarning("Ȱ��ȭ�� CinemachineVirtualCamera�� �����ϴ�.");
        }
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

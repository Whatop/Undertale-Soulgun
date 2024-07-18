using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public float zoomSpeed = 6f; // ������ Ȯ�� �ӵ�
    public float minZoom = 2f;   // �ּ� ũ��
    public float maxZoom = 10f;  // �ִ� ũ��

    void Update()
    {
        float scrollWheelInput = Input.GetAxis("Mouse ScrollWheel");

        // ���콺 �� �Է��� �����Ǹ�
        if (scrollWheelInput != 0 && !UIManager.Instance.isUserInterface)
        {
            // ī�޶� Ȯ��/��Ҹ� ���� ���ο� ī�޶� ũ�� ���
            float newCameraSize = Camera.main.orthographicSize - scrollWheelInput * zoomSpeed;

            // ���ο� ī�޶� ũ�⸦ �ּ� ũ��� �ִ� ũ�� ���̷� ����
            newCameraSize = Mathf.Clamp(newCameraSize, minZoom, maxZoom);

            // ī�޶� ũ�⸦ ������Ʈ
            Camera.main.orthographicSize = newCameraSize;
        }
    }
}

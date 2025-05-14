using UnityEngine;

public class RadialMenuHelper : MonoBehaviour
{
    [Header("�⺻ ����")]
    public GameObject segmentPrefab;    // ������ ���׸�Ʈ ������
    public int count = 8;               // ���׸�Ʈ ����
    public float radius;         // �߽ɿ��� �Ÿ�
    public bool faceOutward = true;     // ���׸�Ʈ�� �ٱ� �������� ȸ����ų�� ����

    [ContextMenu("Generate Segments")]
    public void GenerateSegments()
    {
        if (segmentPrefab == null)
        {
            Debug.LogError("���׸�Ʈ �������� �������� �ʾҽ��ϴ�.");
            return;
        }

        // ���� �ڽ� ����
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }

        float angleStep = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float angle = angleStep * i;
            float rad = angle * Mathf.Deg2Rad;

            Vector2 pos = new Vector2(
                Mathf.Cos(rad) * radius,  // ��õ radius
                Mathf.Sin(rad) * radius
            );

            GameObject segment = Instantiate(segmentPrefab, transform);
            segment.transform.localPosition = pos;

            if (faceOutward)
            {
                segment.transform.localRotation = Quaternion.Euler(0f, 0f, angle - 90f);
            }

            // ������ ����
            segment.transform.localScale = Vector3.one * 0.85f;

            segment.name = $"Segment_{i}";
        }

        Debug.Log($"{count}���� ���׸�Ʈ�� �����߽��ϴ�.");
    }


}

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PieceShooter : MonoBehaviour
{
    public GameObject piecePrefab; // ���� ������
    public int pieceCount = 6; // �߻��� ���� ����
    public float minLaunchForce = 350f; // �ʱ� �߻� �� �ּҰ�
    public float deceleration = 0.95f; // X�� ���� ��
    public float gravityForce = 475f; // Y�� �Ʒ��� ������� �߷� ��
    public float pieceLifetime = 10f; // ������ ������� �ð�
    public float maxLaunchForce = 600f; // �ʱ� �߻� �� �ִ밪

    private List<GameObject> activePieces = new List<GameObject>(); // �߻�� ������ ������ ����Ʈ
    private List<Vector2> velocities = new List<Vector2>(); // �� ������ �ӵ��� ����
    private List<float> maxVelocities = new List<float>(); // �� ������ �ִ� �ӵ��� ����

    public void ShootPieces(RectTransform sourceTransform, Color pieceColor)
    {
        for (int i = 0; i < pieceCount; i++)
        {
            // -90������ 90�� ������ ���� ���� ����
            float randomAngle = Random.Range(-100f, 100f);

            // ������ ����
            GameObject piece = Instantiate(piecePrefab, sourceTransform.position, Quaternion.identity, sourceTransform.parent);
            RectTransform pieceRect = piece.GetComponent<RectTransform>();
            pieceRect.anchoredPosition = sourceTransform.anchoredPosition; // anchoredPosition���� ��ġ ����

            // 350 ~ 600 ������ ���� �ʱ� �߻� �� ����
            float randomLaunchForce = Random.Range(minLaunchForce, maxLaunchForce);

            // �� �������� �ٸ� �ִ� �ӵ��� ����
            maxVelocities.Add(randomLaunchForce);

            // ������ ���� �ʱ� �ӵ� ����
            Vector2 launchDirection = Quaternion.Euler(0, 0, randomAngle) * Vector2.up;
            Vector2 initialVelocity = launchDirection * randomLaunchForce;
            Image image = piece.GetComponent<Image>();
            if (image != null)
            {
                image.color = pieceColor;
            }
            // ������ ����Ʈ�� �߰�
            activePieces.Add(piece);
            velocities.Add(initialVelocity);
        }

        // ���� �ð� �Ŀ� ������ �ı�
        Invoke(nameof(ClearPieces), pieceLifetime);
    }

    void Update()
    {
        // �� �����Ӹ��� ��� ������ �̵���Ŵ
        for (int i = 0; i < activePieces.Count; i++)
        {
            if (activePieces[i] != null)
            {
                RectTransform pieceRect = activePieces[i].GetComponent<RectTransform>();

                // X�� ���� ó��
                velocities[i] = new Vector2(velocities[i].x * deceleration, velocities[i].y);

                // �߷� �߰� (Y�࿡�� ����)
                velocities[i] += Vector2.down * gravityForce * Time.deltaTime;

                // �������� ������ �ִ� �ӵ� ����
                if (Mathf.Abs(velocities[i].x) > maxVelocities[i])
                {
                    velocities[i] = new Vector2(Mathf.Sign(velocities[i].x) * maxVelocities[i], velocities[i].y);
                }

                // ���� �̵�
                pieceRect.anchoredPosition += velocities[i] * Time.deltaTime;
            }
        }
    }

    void ClearPieces()
    {
        // ��� ������ �����ϰ� ����Ʈ �ʱ�ȭ
        foreach (GameObject piece in activePieces)
        {
            if (piece != null)
            {
                Destroy(piece);
            }
        }
        activePieces.Clear();
        velocities.Clear();
        maxVelocities.Clear();
    }
}

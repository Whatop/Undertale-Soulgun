using UnityEngine;

[CreateAssetMenu(fileName = "GameConfigSO", menuName = "GameData/GameConfigSO", order = 1)]
public class GameConfigSO : ScriptableObject
{
    [Header("SavePoint Settings")]
    public GameObject savePrefab; // SavePoint ������
    public Vector3[] savePointPositions; // SavePoint ��ġ ����Ʈ

    [Header("Map Settings")]
    public string mapName; // �� �̸�

    [Header("Game Time Settings")]
    public float savedTime; // ����� ���� �ð�

    [Header("Sound Settings")]
    public AudioClip backgroundMusic; // �������

    [Header("Other Settings")]
    public bool isDebugMode; // ����� ��� ����
}

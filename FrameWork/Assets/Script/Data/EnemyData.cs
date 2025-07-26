using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "GameData/EnemyData")]
public class EnemyData : ScriptableObject
{
    public float minDistance = 3f;  // �÷��̾���� �ּ� ���� �Ÿ�
    public float maxDistance = 6f;  // �÷��̾���� �ִ� ���� �Ÿ�

    public VirtueType virtue;

    [Tooltip("�� ���Ͱ� �����ϴ� ���� Ű���� (��: 'Mercy', 'Anger')")]
    public List<string> reactableEmotions = new List<string>();

    [Header("�⺻ ����")]
    public float maxHealth = 100f;
    public float moveSpeed = 2f;
    public float shootCooldown = 4f;


    [Header("Ʈ�� ����")]
    public bool isTrapActive = false;     // Ʈ�� Ȱ��ȭ ����
    public float trapShootInterval = 2f; // Ʈ�� �߻� �ֱ�
    private float trapTimer = 0f;        // Ʈ���� Ÿ�̸�

    [Header("��Ÿ")]
    public string bulletPrefabName = "Enemy_None";
}
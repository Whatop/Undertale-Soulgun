using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "NewEnemyData", menuName = "GameData/EnemyData")]
public class EnemyData : ScriptableObject
{
    public float minDistance = 3f;  // �÷��̾���� �ּ� ���� �Ÿ�
    public float maxDistance = 6f;  // �÷��̾���� �ִ� ���� �Ÿ�

    public VirtueType virtue;
    public enum ReactionType
    {
        SpeedUp, SlowDown,
        BulletSpeedUp, BulletSpeedDown,
        Heal, TakeDamage,
        Stun, StopAttack, Invincible, Undying,
        Flee,                       // ����/���� ��Ż ��
        PlayAnim, PlayEffect,       // �����
        Custom                      // Ŀ���� ��
    }
    [System.Serializable]
    public class EmotionReaction
    {
        public string emotionKey;        // ������ ����
        public ReactionType action;     // ���� ReactionType
        public float amount;            // ��ġ(�ӵ�����, ȸ���� ��)
        public float duration;          // ���ӽð�(��) �ʿ� ��
        public string reactionText;        // ������ ����
        public string animTrigger;      // �ִϸ����� Ʈ����(����)
        public string effectName;       // ����Ʈ Ǯ Ű(����)
        public bool onlyOnce;           // 1ȸ�� ��������
    }
    public string GetReaction(string emotion)
    {
        foreach (var r in emotionKeys)
        {
            if (r == emotion)
            {
                foreach (var i in reactableEmotions)
                {
                    if(i.emotionKey == emotion)
                    {
                        return i.reactionText;
                    }
                }

            }
        }
        return string.Empty;
    }
    [Tooltip("�� ���Ͱ� �����ϴ� ���� Ű���� (��: 'Mercy', 'Anger')")]
    // ������ ���� �ؽ�Ʈ�� ������ ����Ʈ�Դϴ�.
    public List<EmotionReaction> reactableEmotions = new List<EmotionReaction>();

    //����ǥ�� ���ÿ�
    public List<string> emotionKeys = new List<string>();

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
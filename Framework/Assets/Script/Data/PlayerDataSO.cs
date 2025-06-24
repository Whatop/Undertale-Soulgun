using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PlayerDataSO", menuName = "GameData/PlayerDataSO", order = 1)]
public class PlayerDataSO : ScriptableObject
{
    public GameObject player; // �÷��̾� ��ü (Prefab ��)
    public int Maxhealth = 6; // �ִ� ü��
    public int health = 6; // ���� ü��
    public Vector3 position = Vector3.zero; // �ʱ� ��ġ
    public string player_Name = "frisk"; // �÷��̾� �̸�

    public List<Item> inventory = new List<Item>(); // �ʱ� �κ��丮 ����
    public GameState currentState = GameState.None; // �ʱ� ����
    public bool isStop = false; // �÷��̾� ���� ����
    public Animator playerAnimator; // �÷��̾� �ִϸ�����
    public bool isInvincible = false; // ���� ����
    public bool isDie = false; // ��� ����
    public bool isPhone = false; // Ư�� ����(�� ��� ���� ��)

    public int LEVEL = 1; // �ʱ� ����
    public int AT = 0; // ���ݷ�
    public int DF = 0; // ����
    public int AT_level = 0; // ���ݷ� ����
    public int DF_level = 0; // ���� ����
    public int EXP = 10; // ����ġ
    public int NextEXP = 0; // ���� �������� ����ġ
    public int GOLD = 0; // �ʱ� ���

    public Item curWeapon; // ���� ����
    public Item curAmmor; // ���� ��

    /// <summary>
    /// �ʱ� �����͸� �����ϴ� �޼��� (�����Ϳ��� ȣ�� ����)
    /// </summary>
    public void ResetData()
    {
        Maxhealth = 6;
        health = 6;
        position = Vector3.zero;
        player_Name = "frisk";
        inventory.Clear();
        currentState = GameState.None;
        isStop = false;
        playerAnimator = null;
        isInvincible = false;
        isDie = false;
        isPhone = false;
        LEVEL = 1;
        AT = 0;
        DF = 0;
        AT_level = 0;
        DF_level = 0;
        EXP = 10;
        NextEXP = 0;
        GOLD = 0;
        curWeapon = null;
        curAmmor = null;
    }
}

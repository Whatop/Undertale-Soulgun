using UnityEngine;
using System;

public enum GameState
{
    Fight,
    Event,
    NpcTalk,
    None
}

[System.Serializable]
public class PlayerData
{
    public float health;
    public Vector3 position;
    public string player_Name;
    public string[] inventory;
    public GameState currentState; // �÷��̾��� ���� ���� ���� �߰�

    public PlayerData()
    {
        // �ʱ�ȭ ���� �߰� (��: �⺻�� ����)
        health = 100f;
        position = Vector3.zero;
        player_Name = "";
        inventory = new string[10];
        currentState = GameState.None; // �ʱ� ���� ����
        // �߰� ������ �ʱ�ȭ
    }
}

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    private PlayerData playerData;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("GameManager");
                    instance = obj.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // �÷��̾� ������ �ʱ�ȭ
        playerData = new PlayerData();
    }

    public PlayerData GetPlayerData()
    {
        return playerData;
    }

    public void SavePlayerData(PlayerData newData)
    {
        // �÷��̾� ������ ����
        playerData = newData;
    }

    public void ChangeGameState(GameState newState)
    {
        playerData.currentState = newState;
        // ���¿� ���� �߰����� ���� ����
        switch (newState)
        {
            case GameState.Fight:
                // Fight ���¿� ���� ���� ����
                break;
            case GameState.Event:
                // Event ���¿� ���� ���� ����
                break;
            case GameState.NpcTalk:
                // NpcTalk ���¿� ���� ���� ����
                break;
            default:
                break;
        }
    }
}

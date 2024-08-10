using UnityEngine;
using System;
public enum GameState
{
    Fight,
    Event,
    NpcTalk,
    None
}
public enum CameraType
{
    Hor,
    Ver,
    All,
    None
}
[System.Serializable]
public class PlayerData
{
    public int Maxhealth;
    public int health;
    public Vector3 position;
    public string player_Name;
    public string[] inventory;
    public GameState currentState; // �÷��̾��� ���� ���� ���� �߰�
    public bool isStop = false;


    public PlayerData()
    {
        // �ʱ�ȭ ���� �߰� (��: �⺻�� ����)
        Maxhealth = 10;
        health = 6;
        position = Vector3.zero;
        player_Name = "";
        inventory = new string[10]; // �������� ũ�⸦ ������ �� �ֵ��� ��� ����
        currentState = GameState.None; // �ʱ� ���� ����
        // �߰� ������ �ʱ�ȭ
    }
}

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    private PlayerData playerData;
    private Weapon weaponData;

    /// <summary>
    /// ���� Ȯ�ο�
    /// </summary>
    public bool isBattle;
    public CameraType cameraType; // ī�޶��� ���� ���� 
    public int curportalNumber = 0;

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
        weaponData = new Weapon();
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

    public Weapon GetWeaponData()
    {
        return weaponData;
    }

    public void SaveWeaponData(Weapon newData)
    {
        // ���� ������ ����, �Ϻ� ����� ����Ҽ���?
        weaponData = newData;
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
    public void ChangeCameraState(CameraType newState)
    {
        cameraType = newState;
    }
    public void ChangeCameraState(CameraType newState, int point)
    {
        cameraType = newState;
        curportalNumber = point;
        
    }
    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    public void OpenUI()
    {
        UIManager.Instance.isUserInterface = true;
    }
}


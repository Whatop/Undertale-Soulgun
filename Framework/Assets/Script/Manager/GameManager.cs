using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
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
    public int Maxhealth;
    public int health;
    public Vector3 position;
    public string player_Name;
    public List<string> inventory;
    public GameState currentState; // �÷��̾��� ���� ���� ���� �߰�
    public bool isStop = false;
    public Animator playerAnimator;
    public bool isInvincible;
    public bool isDie;

    public PlayerData()
    {
        // �ʱ�ȭ ���� �߰� (��: �⺻�� ����)
        Maxhealth = 6;
        health = 6;
        position = Vector3.zero;
        player_Name = "";
        inventory = new List<string>();// �������� ũ�⸦ ������ �� �ֵ��� ��� ����
        currentState = GameState.None; // �ʱ� ���� ����
        playerAnimator = null;
        isDie = false;
        // �߰� ������ �ʱ�ȭ
    }
}

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    private PlayerData playerData;
    private Weapon weaponData;

    public Action<GameState> OnGameStateChanged;
    /// <summary>
    /// ���� Ȯ�ο�
    /// </summary>
    public bool isBattle;
    public int curportalNumber = 0;

    public bool isPortalTransition = false;
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
        isBattle = (newState == GameState.Fight); // ���� ���¿� ����
        OnGameStateChanged?.Invoke(newState);
    }
    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    public void OpenUI()
    {
        UIManager.Instance.isUserInterface = true;
    }

    public void Die()
    {
        playerData.isDie = true;
        playerData.playerAnimator.SetBool("isDie",true);
        UIManager.Instance.playGameover();
        DestroyAllEnemies();

    }
    public void DestroyAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
    }
}


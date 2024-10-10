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
    public List<Item> inventory;
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
        player_Name = "frisk";
        inventory = new List<Item>();// �������� ũ�⸦ ������ �� �ֵ��� ��� ����
        currentState = GameState.None; // �ʱ� ���� ����
        playerAnimator = null;
        isDie = false;
        // �߰� ������ �ʱ�ȭ
    }
}
[System.Serializable]
public class Item
{
    public int id;          // ������ ���� ID
    public string itemName; // ������ �̸�
    public string description; // ������ ����

    public Item(int id, string name, string description)
    {
        this.id = id;
        this.itemName = name;
        this.description = description;
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
    private float startTime;   // ���� ���� �ð�
    private float savedTime;   // ������ ����� �ð� (���� �ð�)
    private bool isSave;

    string mapName = "���� - �� ������ ";
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
    private void Start()
    {
        startTime = Time.time;
        isSave = PlayerPrefs.GetInt("MyBoolValue", 0) == 1 ? true : false;

        if (isSave) // ������ִٸ�
        {
            Load();
            LoadGameTime();
        }
    }
    public void SaveGameTime()
    {
        // ��������� ��� �ð��� ���� (�� ����)
        savedTime += Time.time - startTime;

        // ������ ���Ѵٸ� PlayerPrefs ��� (������ ����)
        PlayerPrefs.SetFloat("SavedGameTime", savedTime);

        // ������ �ٽ� ������ �� �ð��� �缳��
        startTime = Time.time;
    }
    private void LoadGameTime()
    {
        // ����� �ð��� ������ �ε�, ������ 0���� ����
        savedTime = PlayerPrefs.GetFloat("SavedGameTime", 0f);
    }
    public string GetElapsedTimeInMinutes()
    { // ���� ����� �ð� ��� (�� ����)
        float elapsedTime = (Time.time - startTime) + savedTime;

        // �а� �ʸ� �и��Ͽ� �� �ڸ��� ǥ��
        string minutes = Mathf.Floor(elapsedTime / 60).ToString("00");
        string seconds = (elapsedTime % 60).ToString("00");
        return  (minutes+":"+seconds); // �� ������ ��ȯ
    }
    public string GetMapName()
    {
        return mapName;
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
    public void AddItem(int id, string name, string description)
    {
        // �κ��丮�� ���� ���� �ʾҴ��� Ȯ��
        if (GetPlayerData().inventory.Count < 9)
        {
            Item newItem = new Item(id, name, description);
            GetPlayerData().inventory.Add(newItem);
        }
        else
        {
            Debug.Log("�κ��丮�� ���� á���ϴ�.");
            //@@ �߰���
        }
    }


    // Save Method
    public void Save()
    {
        isSave = true;
        // �÷��̾� ��ġ ����
        PlayerPrefs.SetFloat("PlayerPosX", playerData.position.x);
        PlayerPrefs.SetFloat("PlayerPosY", playerData.position.y);
        PlayerPrefs.SetFloat("PlayerPosZ", playerData.position.z);

        // ü�� �� ��Ÿ �÷��̾� ������ ����
        PlayerPrefs.SetInt("PlayerHealth", playerData.Maxhealth);  // �ִ�ü������ ȸ��
        PlayerPrefs.SetInt("PlayerMaxHealth", playerData.Maxhealth);
        PlayerPrefs.SetString("PlayerName", playerData.player_Name);

        PlayerPrefs.SetInt("MyBoolValue", isSave ? 1 : 0);
        // �κ��丮 ������ ����
        for (int i = 0; i < playerData.inventory.Count; i++)
        {
            PlayerPrefs.SetString("InventoryItem_" + i, playerData.inventory[i].ToString());
        }
        PlayerPrefs.SetInt("InventoryCount", playerData.inventory.Count);

        PlayerPrefs.Save();
        Debug.Log("������ ����Ǿ����ϴ�.");
    }

    // Load Method
    public void Load()
    {
        // �÷��̾� ��ġ �ε�
        float posX = PlayerPrefs.GetFloat("PlayerPosX", 0f);
        float posY = PlayerPrefs.GetFloat("PlayerPosY", 0f);
        float posZ = PlayerPrefs.GetFloat("PlayerPosZ", 0f);
        playerData.position = new Vector3(posX, posY, posZ);

        // ü�� �� ��Ÿ �÷��̾� ������ �ε�
        playerData.health = PlayerPrefs.GetInt("PlayerHealth", 6); // �⺻ �� 6
        playerData.Maxhealth = PlayerPrefs.GetInt("PlayerMaxHealth", 6);
        playerData.player_Name = PlayerPrefs.GetString("PlayerName", "frisk");

        // �κ��丮 ������ �ε�
      //  int inventoryCount = PlayerPrefs.GetInt("InventoryCount", 0);
      //  playerData.inventory.Clear(); // ���� �κ��丮 �ʱ�ȭ
      //  for (int i = 0; i < inventoryCount; i++)
      //  {
      //      string item = PlayerPrefs.GetInt("InventoryItem_" + i, );
      //      if (!string.IsNullOrEmpty(item))
      //      {
      //          playerData.inventory.Add(item);
      //      }
      //  }

        Debug.Log("������ �ε�Ǿ����ϴ�.");
    }
}


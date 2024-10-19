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
public enum ItemType
{
    HealingItem,
    Weapon,
    Armor,
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

    // �÷��̾� �������� ����, �尩
    private Item curWeapon;
    private Item curAmmor;

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

    public void IncreaseHealth(int v)
    {
        if(health < Maxhealth)
            health += v;
        else
            health = Maxhealth;

    }
    public void EquipWeapon(Item item)
    {
        curWeapon = item;
    }
    public void EquipAmmor(Item item)
    {
        curAmmor = item;
    }
}
[System.Serializable]
public class Item
{
    public int id;          // ������ ���� ID
    public string itemName; // ������ �̸�
    public string description; // ������ ����
    public ItemType itemType = ItemType.None;
    public Item(int id, string name, string description)
    {
        this.id = id;
        this.itemName = name;
        this.description = description;
    }
    public Item(int id, string name, string description,ItemType itemType)
    {
        this.id = id;
        this.itemName = name;
        this.description = description;
        this.itemType = itemType;
    }
}

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    [SerializeField]
    private PlayerData playerData;
    [SerializeField]
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
            //AddItem(0, "���� ����", "�������� �ƴ����� �ѷ��� ���� ����.", ItemType.HealingItem);
            //AddItem(0, "���� ����", "�������� �ƴ����� �ѷ��� ���� ����.", ItemType.HealingItem);
            //AddItem(49,"������", "��ǰ ��������.", ItemType.Weapon);
            //AddItem(48, "ī�캸�� ����", "������ ���� �� ���ڿ� �μ����� �� ��︱�ٵ�.", ItemType.Armor);
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
    public void AddItem(int id, string name, string description, ItemType itemType = ItemType.None)
    {
        // �κ��丮�� ���� ���� �ʾҴ��� Ȯ��
        if (GetPlayerData().inventory.Count < 9)
        {
            Item newItem = new Item(id, name, description, itemType);
            GetPlayerData().inventory.Add(newItem);
        }
        else
        {
            Debug.Log("�κ��丮�� ���� á���ϴ�.");
            //@@ �߰���
        }
    }
    public void UseItem(int Id)
    {
                SoundManager.Instance.SFXPlay("select_sound", 173);
        // Inventory�� �����ϴ��� Ȯ���ϴ� ����
        if (Id < 0 || Id >= GetPlayerData().inventory.Count)
        {
            Debug.LogWarning("Invalid item ID.");
            return;
        }

        int itemId = GetPlayerData().inventory[Id].id;
        switch (itemId)
        {
            case 0:
                Debug.Log("Item does nothing.");
                break;
            case 1:
                // ����: ü�� ����
                GetPlayerData().IncreaseHealth(1);
                DropItem(Id); // ��� �� ����
                break;
            case 2:
                // ����: ���� ����
                GetPlayerData().EquipWeapon(GetPlayerData().inventory[Id]);
                break;
            // �ٸ� ������ ȿ�� �߰� ����
            default:
                Debug.Log("Unknown item.");
                break;
        }
    }

  

    public string InfoItem(int Id)
    {
                SoundManager.Instance.SFXPlay("select_sound", 173);
        if (Id < 0 || Id >= GetPlayerData().inventory.Count)
        {
            return "Invalid item ID.";
        }

        string item_Description = GetPlayerData().inventory[Id].description;
        return item_Description;
    }

    public void DropItem(int Id)
    {
                SoundManager.Instance.SFXPlay("select_sound", 173);
        if (Id < 0 || Id >= GetPlayerData().inventory.Count)
        {
            Debug.LogWarning("Invalid item ID.");
            return;
        }

        GetPlayerData().inventory.RemoveAt(Id);
        Debug.Log("Item dropped.");
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
        List<Item> inventory = playerData.inventory;

        // �κ��丮 ũ�� ����
        PlayerPrefs.SetInt("InventoryCount", inventory.Count);

        for (int i = 0; i < inventory.Count; i++)
        {
            // �� �������� ���� �Ӽ� ���� (ID, �̸�, ����, Ÿ��)
            PlayerPrefs.SetInt($"Item_{i}_ID", inventory[i].id);
            PlayerPrefs.SetString($"Item_{i}_Name", inventory[i].itemName);
            PlayerPrefs.SetString($"Item_{i}_Description", inventory[i].description);
            PlayerPrefs.SetInt($"Item_{i}_Type", (int)inventory[i].itemType);
        }
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
        playerData.player_Name = PlayerPrefs.GetString("PlayerName", playerData.player_Name);

        int inventoryCount = PlayerPrefs.GetInt("InventoryCount", 0);
        List<Item> loadedInventory = new List<Item>();

        for (int i = 0; i < inventoryCount; i++)
        {
            // ���� �Ӽ� ��������
            int id = PlayerPrefs.GetInt($"Item_{i}_ID");
            string name = PlayerPrefs.GetString($"Item_{i}_Name");
            string description = PlayerPrefs.GetString($"Item_{i}_Description");
            ItemType itemType = (ItemType)PlayerPrefs.GetInt($"Item_{i}_Type");

            // ������ ��ü�� �����ؼ� ����Ʈ�� �߰�
            Item newItem = new Item(id, name, description, itemType);
            loadedInventory.Add(newItem);
        }
        playerData.inventory = loadedInventory;

        Debug.Log("������ �ε�Ǿ����ϴ�.");
    }
}


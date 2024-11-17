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
    Ammor,
    None
}
[System.Serializable]
public class PlayerData
{
    public GameObject player;
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
    public bool isPhone;

    public int LEVEL = 1;
    public int AT = 0;
    public int DF = 0;
    public int AT_level = 0;
    public int DF_level = 0;
    public int EXP = 10;
    public int NextEXP = 0;
    public int GOLD = 0;

    // �÷��̾� �������� ����, �尩
    public Item curWeapon;
    public Item curAmmor;

    public PlayerData()
    {
        // �ʱ�ȭ ���� �߰� (��: �⺻�� ����)
        Maxhealth = 6;
        health = 6;
        position = Vector3.zero;
        player_Name = "frisk";
        LEVEL = 1;

        inventory = new List<Item>();// �������� ũ�⸦ ������ �� �ֵ��� ��� ����
        currentState = GameState.None; // �ʱ� ���� ����
        playerAnimator = null;
        isDie = false;
        isPhone = false;
        // �߰� ������ �ʱ�ȭ
    }

    public void IncreaseHealth(int v)
    {
        if(health < Maxhealth)
            health += v;
        else
            health = Maxhealth;

    }
    public void LevelUp()
    {
        LEVEL++;
        IncreaseHealth(10);
    }
    public void EquipWeapon(Item item)
    {
        curWeapon = item;
    }
    public void EquipAmmor(Item item)
    {
        curAmmor = item;
    }
    public Item GetEquippedWeapon()
    {
        return curWeapon;
    }
    public Item GetEquippedAmmor()
    {
        return curAmmor;
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
    public float savedTime;   // ������ ����� �ð� (���� �ð�)
    private bool isSave;

    string mapName = "���� - �� ������ ";
    public bool isPortalTransition = false;

    private DialogueManager dialogueManager;
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
        dialogueManager = DialogueManager.Instance;
    }
    private void Start()
    {
        startTime = Time.time;
        isSave = PlayerPrefs.GetInt("MyBoolValue", 0) == 1 ? true : false;

        if (isSave) // ������ִٸ�
        {
            Load();
            LoadGameTime();
            //UIManager.Instance.ResetSettings();
         
            Item fristWaepon = new Item(49, "������", "* ��ǰ ��������.", ItemType.Weapon);
            Item fristIAmmor = new Item(48, "ī�캸�� ����", "* ������ ���� �� ���ڿ� �μ����� �� ��︱�ٵ�.", ItemType.Ammor);
            GetPlayerData().EquipWeapon(fristWaepon);
            GetPlayerData().EquipAmmor(fristIAmmor);

        }
        else
        {
            //AddItem(0);
            //AddItem(0);
            //AddItem(52);
            //AddItem(61);
            //Item fristWaepon = new Item(51, "������", "��ǰ ��������.", ItemType.Weapon);
            //Item fristIAmmor = new Item(48,"ī�캸�� ����", "* ������ ���� �� ���ڿ� \n    * �μ����� �� ��︱�ٵ�.", ItemType.Ammor);
            //GetPlayerData().EquipWeapon(fristWaepon);
            //GetPlayerData().EquipAmmor(fristIAmmor);
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
    public void AddItem(int id)
    {
        // �κ��丮�� ���� ���� �ʾҴ��� Ȯ��
        if (GetPlayerData().inventory.Count >= 9)
        {
            Debug.Log("�κ��丮�� ���� á���ϴ�.");
            return;
        }

        // JSON �����Ϳ��� �ش� id�� �������� ã��
        Item itemToAdd = dialogueManager.itemDatabase.items.Find(item => item.id == id);
        if (itemToAdd != null)
        {
            // ID ������ ���� ������ Ÿ�� ����
            if (itemToAdd.id >= 0 && itemToAdd.id <= 50)
            {
                itemToAdd.itemType = ItemType.HealingItem;
            }
            else if (itemToAdd.id >= 51 && itemToAdd.id <= 60)
            {
                itemToAdd.itemType = ItemType.Ammor;
            }
            else if (itemToAdd.id >= 61 && itemToAdd.id <= 70)
            {
                itemToAdd.itemType = ItemType.Weapon;
            }
            else
            {
                itemToAdd.itemType = ItemType.None;
            }

            // ������ Ÿ�԰� �Բ� �������� �κ��丮�� �߰�
            GetPlayerData().inventory.Add(itemToAdd);
        }
    }

    public void UseItem(int Id)
    {
        SoundManager.Instance.SFXPlay("select_sound", 173);

        // �κ��丮���� ��ȿ�� ������ ID���� Ȯ��
        if (Id < 0 || Id >= GetPlayerData().inventory.Count)
        {
            Debug.LogWarning("Invalid item ID.");
            return;
        }

        Item itemToEquip = GetPlayerData().inventory[Id];
        ItemType itemType = itemToEquip.itemType;
        dialogueManager.StartItemDialogue(itemToEquip); // �̺�Ʈ ���ó�� ó��

        switch (itemType)
        {
            case ItemType.None:
                Debug.Log("Item does nothing.");
                break;

            case ItemType.HealingItem:
                // ü�� ���� ����
                GetPlayerData().IncreaseHealth(1); 
                        
                        
                GetPlayerData().inventory.RemoveAt(Id); 
                break;

            case ItemType.Weapon:
                // ���� ���� �� ��ü
                Item currentWeapon = GetPlayerData().GetEquippedWeapon();
                if (currentWeapon != null)
                {
                    // ������ ������ ���⸦ �κ��丮�� �ٽ� �߰�
                    GetPlayerData().inventory.Add(currentWeapon);
                }
                // ���ο� ���� ����
                GetPlayerData().EquipWeapon(itemToEquip);
                GetPlayerData().inventory.RemoveAt(Id); 
                break;

            case ItemType.Ammor:
                // �� ���� �� ��ü
                Item currentAmmor = GetPlayerData().GetEquippedAmmor();
                if (currentAmmor != null)
                {
                    // ������ ������ ���� �κ��丮�� �ٽ� �߰�
                    GetPlayerData().inventory.Add(currentAmmor);
                }
                // ���ο� �� ����
                GetPlayerData().EquipAmmor(itemToEquip);
                GetPlayerData().inventory.RemoveAt(Id); 
                // ���Ӱ� ����� ���� �κ��丮���� ����
                break;

            // �߰����� ������ ȿ���� ���⼭ �߰� ����
            default:
                Debug.Log("Unknown item.");
                break;
        }
    }



    public void InfoItem(int Id)
    {
                SoundManager.Instance.SFXPlay("select_sound", 173);
        if (Id < 0 || Id >= GetPlayerData().inventory.Count)
        {
            Debug.LogWarning("Invalid item ID.");
            return;
        }

        Item itemToEquip = GetPlayerData().inventory[Id];
        
        dialogueManager.StartInfoDialogue(itemToEquip); // �̺�Ʈ ���ó�� ó��

    }

    public void DropItem(int Id)
    {
        SoundManager.Instance.SFXPlay("select_sound", 173);
        if (Id < 0 || Id >= GetPlayerData().inventory.Count)
        {
            Debug.LogWarning("Invalid item ID.");
            return;
        }

        GetPlayerData().inventory.RemoveAt(Id); // �ε����� �״�� ���

        // ������ ���� ���
                dialogueManager.SetUINPC(); // �̺�Ʈ ���ó�� ó��
        Item itemToEquip = GetPlayerData().inventory[Id];
        dialogueManager.StartDropDialogue(itemToEquip); // �̺�Ʈ ���ó�� ó��
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
        PlayerPrefs.SetInt("PlayerHealth", playerData.Maxhealth);  // �ִ� ü������ ȸ��
        PlayerPrefs.SetInt("PlayerMaxHealth", playerData.Maxhealth);
        PlayerPrefs.SetString("PlayerName", playerData.player_Name);

        PlayerPrefs.SetInt("MyBoolValue", isSave ? 1 : 0);

        // �κ��丮 ������ ����
        List<Item> inventory = playerData.inventory;
        PlayerPrefs.SetInt("InventoryCount", inventory.Count);

        for (int i = 0; i < inventory.Count; i++)
        {
            // �� �������� ���� �Ӽ� ���� (ID, �̸�, ����, Ÿ��)
            PlayerPrefs.SetInt($"Item_{i}_ID", inventory[i].id);
            PlayerPrefs.SetString($"Item_{i}_Name", inventory[i].itemName);
            PlayerPrefs.SetString($"Item_{i}_Description", inventory[i].description);
            PlayerPrefs.SetInt($"Item_{i}_Type", (int)inventory[i].itemType);
        }

        // ���� ���� ����
        if (playerData.curWeapon != null)
        {
            PlayerPrefs.SetInt("CurWeapon_ID", playerData.curWeapon.id);
            PlayerPrefs.SetString("CurWeapon_Name", playerData.curWeapon.itemName);
            PlayerPrefs.SetString("CurWeapon_Description", playerData.curWeapon.description);
            PlayerPrefs.SetInt("CurWeapon_Type", (int)playerData.curWeapon.itemType);
        }

        // ���� ���� ����
        if (playerData.curAmmor != null)
        {
            PlayerPrefs.SetInt("CurAmmor_ID", playerData.curAmmor.id);
            PlayerPrefs.SetString("CurAmmor_Name", playerData.curAmmor.itemName);
            PlayerPrefs.SetString("CurAmmor_Description", playerData.curAmmor.description);
            PlayerPrefs.SetInt("CurAmmor_Type", (int)playerData.curAmmor.itemType);
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

        // �κ��丮 ������ �ε�
        int inventoryCount = PlayerPrefs.GetInt("InventoryCount", 0);
        List<Item> loadedInventory = new List<Item>();

        for (int i = 0; i < inventoryCount; i++)
        {
            // ���� �Ӽ� ��������
            int id = PlayerPrefs.GetInt($"Item_{i}_ID");
            string name = PlayerPrefs.GetString($"Item_{i}_Name");
            string description = PlayerPrefs.GetString($"Item_{i}_Description");
            ItemType itemType = (ItemType)PlayerPrefs.GetInt($"Item_{i}_Type");

            // ������ ��ü�� �����Ͽ� ����Ʈ�� �߰�
            Item newItem = new Item(id, name, description, itemType);
            loadedInventory.Add(newItem);
        }
        playerData.inventory = loadedInventory;

        // ���� ���� �ε�
        if (PlayerPrefs.HasKey("CurWeapon_ID"))
        {
            int weaponId = PlayerPrefs.GetInt("CurWeapon_ID");
            string weaponName = PlayerPrefs.GetString("CurWeapon_Name");
            string weaponDescription = PlayerPrefs.GetString("CurWeapon_Description");
            ItemType weaponType = (ItemType)PlayerPrefs.GetInt("CurWeapon_Type");

            // ���� ���� ������ ����
            playerData.curWeapon = new Item(weaponId, weaponName, weaponDescription, weaponType);
        }

        // ���� ���� �ε�
        if (PlayerPrefs.HasKey("CurAmmor_ID"))
        {
            int armorId = PlayerPrefs.GetInt("CurAmmor_ID");
            string armorName = PlayerPrefs.GetString("CurAmmor_Name");
            string armorDescription = PlayerPrefs.GetString("CurAmmor_Description");
            ItemType armorType = (ItemType)PlayerPrefs.GetInt("CurAmmor_Type");

            // ���� ���� ������ ����
            playerData.curAmmor = new Item(armorId, armorName, armorDescription, armorType);
        }

        Debug.Log("������ �ε�Ǿ����ϴ�.");
        if (GetPlayerData().player !=null)
        GetPlayerData().player.GetComponent<PlayerMovement>().updateLoad();
    }

}


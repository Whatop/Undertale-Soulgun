using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIManager : MonoBehaviour
{
    GameManager gameManager;

    private static UIManager instance;
    public GameObject[] ui_positions; //health : 0, Weapon : 1
    
    [SerializeField]
    private GameObject[] ui_healths;

    [SerializeField]
    private GameObject[] ui_ammo;
    public Image ui_weaponImage;
    public TextMeshProUGUI ui_ammoText;


    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UIManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("UIManager");
                    instance = obj.AddComponent<UIManager>();
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

        gameManager = GameManager.Instance;
    }
    private void Start()
    {
        InitHeart();
        InitWeapon();
    }
    void InitHeart() // ü�� ����
    {
        PlayerData player = gameManager.GetPlayerData();

        int heart_count = player.Maxhealth / 2;
        ui_healths = new GameObject[heart_count];

        for (int i = 0; i < heart_count; i++)
        {
            GameObject heartPrefab = Resources.Load<GameObject>("Prefabs/Heart");
            GameObject instance = Instantiate(heartPrefab, ui_positions[0].transform);

            float sizeX = heartPrefab.GetComponent<Image>().sprite.texture.width;
            // �̵��� �Ÿ��� ����Ͽ� ��ġ ����
            Vector3 newPosition = instance.transform.position;
            newPosition.x = sizeX + i * sizeX;
            instance.transform.position = newPosition;
            
            // ui_healths �迭�� �ν��Ͻ� ����
            ui_healths[i] = instance;
        }
    }
    void InitWeapon() // �� ����
    {
        Weapon weapon = gameManager.GetWeaponData();

        int ammo_count= weapon.currentAmmo;
        ui_ammo = new GameObject[ammo_count];

        for (int i = 0; i < ammo_count; i++)
        {
            GameObject weaponPrefab = Resources.Load<GameObject>("Prefabs/Ammo");
            GameObject instance = Instantiate(weaponPrefab, ui_positions[1].transform);

            float sizeY = weaponPrefab.GetComponent<Image>().sprite.texture.height;
            Debug.Log(sizeY);
            // �̵��� �Ÿ��� ����Ͽ� ��ġ ����
            Vector3 newPosition = instance.transform.position;
            newPosition.y = ui_positions[1].transform.position.y + i * 10;
            instance.transform.position = newPosition;

            // ui_ammo �迭�� �ν��Ͻ� ����
            ui_ammo[i] = instance;
        }
    }
    private void Update()
    {
        PlayerData player = gameManager.GetPlayerData();
        int currentHealth = player.health;

        // ü�� �̹��� ������Ʈ
        for (int i = 0; i < ui_healths.Length; i++)
        {
            int spriteIndex = Mathf.Clamp(currentHealth - i * 2, 0, 2); // ü�¿� ���� ��������Ʈ �ε��� ���
            ui_healths[i].GetComponent<HeartScript>().SetImage(spriteIndex);
        }
    }
}

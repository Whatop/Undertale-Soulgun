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
    public GameObject[] pedestal;
    public Image ui_weaponImage;
    public TextMeshProUGUI ui_ammoText;
    public Canvas uicanvas;
    public Camera mainCamera;
    public TextMeshProUGUI damageTextPrefab; // DamageText ������

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
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float screenRatio = screenWidth / screenHeight;

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

            float sizeX = instance.GetComponent<RectTransform>().sizeDelta.x;

            Vector3 newPosition = instance.transform.position;
            newPosition.x = ui_positions[0].transform.position.x + i * sizeX; // ���� �������� ��ġ ����
            instance.transform.position = newPosition;
            ui_healths[i] = instance;
        }
    }
    void InitWeapon() // �� ����
    {
        Weapon weapon = gameManager.GetWeaponData();

        int ammo_count = weapon.magazine;
        ui_ammo = new GameObject[ammo_count];


        for (int i = 0; i < ammo_count; i++)
        {
            GameObject weaponPrefab = Resources.Load<GameObject>("Prefabs/Ammo");
            GameObject instance = Instantiate(weaponPrefab, ui_positions[1].transform);

            float sizeY = instance.GetComponent<RectTransform>().sizeDelta.y;
            Debug.Log(sizeY);
            Vector3 newPosition = instance.transform.position;
            newPosition.y = ui_positions[1].transform.position.y + i * sizeY  * 1.25f; // ���� �������� ��ġ ����
            instance.transform.position = newPosition;
            ui_ammo[i] = instance;
        }
    }

    public void ShowDamageText(Vector3 worldPosition, int damageAmount)
    { // ���� ��ǥ�� ��ũ�� ��ǥ�� ��ȯ
        Vector2 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);

        // ��ũ�� ��ǥ�� Canvas �ȿ��� ��� ������ ��ġ�� ��ȯ
        RectTransformUtility.ScreenPointToLocalPointInRectangle(uicanvas.transform as RectTransform, screenPosition, uicanvas.worldCamera, out Vector2 canvasPosition);
        
        // �ؽ�Ʈ ����
        TextMeshProUGUI damageText = Instantiate(damageTextPrefab, uicanvas.transform);
        damageText.rectTransform.localPosition = canvasPosition;
        damageText.text = damageAmount.ToString();
    }
    private void Update()
    {
        PlayerData player = gameManager.GetPlayerData();
        int currentHealth = player.health;

        // ü�� �̹��� ������Ʈ
        for (int i = 0; i < ui_healths.Length; i++)
        {
            int spriteIndex = Mathf.Clamp(currentHealth - i * 2, 0, 2); // ü�¿� ���� ��������Ʈ �ε��� ���
            ui_healths[i].GetComponent<ImageScript>().SetImage(spriteIndex);
        }
        Weapon weapon = gameManager.GetWeaponData();
        int current_magazine = weapon.current_magazine;

        // �Ѿ� �̹��� ������Ʈ
        for (int i = 0; i < ui_ammo.Length; i++)
        {
            int spriteIndex = (i < current_magazine) ? 0 : 1; // �Ѿ� ������ ���� ��������Ʈ �ε��� ���

            // Image ������Ʈ�� sprite �Ӽ��� ����Ͽ� ��������Ʈ ����
            ui_ammo[i].GetComponent<ImageScript>().SetImage(spriteIndex);
        }
        ui_ammoText.text = weapon.current_Ammo + "/" + weapon.maxAmmo;
    }
}

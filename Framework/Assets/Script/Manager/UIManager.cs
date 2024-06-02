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

    //Option
    public GameObject optionPanel;
    public GameObject puasePanel;

    public Button[] buttons;
    public Toggle fullScreenToggle;
    public Scrollbar brightnessScrollbar;
    public Toggle vSyncToggle;
    public Scrollbar bgmScrollbar;
    public Scrollbar sfxScrollbar;
    public Scrollbar cameraShakeScrollbar;
    public Scrollbar miniMapSizeScrollbar;

    private int currentIndex = 0;

    public GameObject[] Interface;
    public bool isUserInterface = false;

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
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;  // Local copy for the closure
            buttons[i].onClick.AddListener(() => OnButtonClick(index));
        }
        InitHeart();
        InitWeapon();
        UpdateSelection();
    }
    public void upButton()
    {
        Debug.Log("��");
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
        damageText.GetComponent<DamageText>().Initialize(damageAmount);
    }
    public void UpdateUI()
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
    private void Update()
    {
        if (isUserInterface)
        {
            Time.timeScale = 0f;
            Interface[0].SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
        }
        UpdateUI();
        OptionInput();
    }
    #region option

    void OptionInput()
    {
        //Option 
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            currentIndex = 0;// ���� ������ ���ư���
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            Navigate(-1);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            Navigate(1);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            AdjustValue(-0.1f);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            AdjustValue(0.1f);
        }
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            ToggleValue();
        }
    }
    void Navigate(int direction)
    {
        currentIndex = Mathf.Clamp(currentIndex + direction, 0, buttons.Length - 1);
        UpdateSelection();
        //selectSound.Play(); �̵� �Ҹ�SoundManager�� ��ü
    }

    void AdjustValue(float value)
    {
        switch (currentIndex)
        {
            case 4: // ��� ����
                brightnessScrollbar.value = Mathf.Clamp(brightnessScrollbar.value + value, 0, 1);
                break;
            case 8: // BGM ���� ����
                bgmScrollbar.value = Mathf.Clamp(bgmScrollbar.value + value, 0, 1);
                break;
            case 9: // SFX ���� ����
                sfxScrollbar.value = Mathf.Clamp(sfxScrollbar.value + value, 0, 1);
                break;
            case 10: // ī�޶� ��鸲 ����
                cameraShakeScrollbar.value = Mathf.Clamp(cameraShakeScrollbar.value + value, 0, 1);
                break;
            case 11: // �̴ϸ� ũ�� ����
                miniMapSizeScrollbar.value = Mathf.Clamp(miniMapSizeScrollbar.value + value, 0, 1);
                break;
        }

        //selectSound.Play(); SoundManager�� ��ü
    }

    void ToggleValue()
    {
        switch (currentIndex)
        {
            case 5: // ��ü ȭ�� ���
                fullScreenToggle.isOn = !fullScreenToggle.isOn;
                break;
            case 7: // ���� ����ȭ ���
                vSyncToggle.isOn = !vSyncToggle.isOn;
                break;
        }
        //selectSound.Play(); SoundManager�� ��ü
    }

    void UpdateSelection()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            ColorBlock colors = buttons[i].colors;
            colors.normalColor = Color.white; // �⺻ ����
            colors.highlightedColor = Color.white; // ���� ����
            colors.pressedColor = Color.gray; // Ŭ�� �� ����
            colors.selectedColor = (i == currentIndex) ? Color.white : Color.white; // ���õ� ����
            buttons[i].colors = colors;

            // �ؽ�Ʈ ���� ����
            TextMeshProUGUI buttonText = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.color = (i == currentIndex) ? Color.white : Color.gray; // ���� ����
            }
        }
    }
    public void OnButtonClick(int index)
    {
        currentIndex = index;
        UpdateSelection();
        //navigateSound.Play(); ���� �߰�
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

using UnityEngine.SceneManagement;
using System;

public class UIManager : MonoBehaviour
{
    GameManager gameManager;
    DialogueManager dialogueManager;
    private SoundManager soundManager; // SoundManager �ν��Ͻ��� �ʵ�� �߰�

    private static UIManager instance;
    public GameObject[] ui_positions; //health : 0, Weapon : 1, hp2 : 0

    [SerializeField]
    private GameObject[] ui_healths;

    [SerializeField]
    private GameObject[] ui_ammo;
    public GameObject[] pedestal;
    public Image ui_weaponImage;
    public Image ui_weaponBackImage;
    public GameObject ui_healthImage;
    public TextMeshProUGUI ui_ammoText;
    public Canvas uicanvas;
    public Camera mainCamera;
    public TextMeshProUGUI damageTextPrefab; // DamageText ������

    // Option
    public GameObject optionPanel;
    public GameObject puasePanel;
    public GameObject keyChangePanel;
    public GameObject YN_ResetPanel;
    public GameObject KeyCheckPanel;

    public Button[] mainButtons;
    public Button[] optionButtons;
    public Button[] keyChangeButtons;
    public Button[] YNButtons;

    public Button[] keyChangefunctions;

    [SerializeField]
    private Button[] currentButtons;
    public Button fullScreenToggle;
    public Scrollbar brightnessScrollbar;
    public Button vSyncToggle;
    public Button cusorToggle;
    public Scrollbar bgmScrollbar;
    public Scrollbar sfxScrollbar;
    public Scrollbar cameraShakeScrollbar;
    public Scrollbar miniMapSizeScrollbar;

    public Sprite onSprite;
    public Sprite offSprite;

    public bool isFullScreen = false;
    public bool isVSyncOn = false;
    public bool isCursorVisible = true;

    [SerializeField]
    private int currentIndex = 0;

    [SerializeField]
    private int curRIndex = 6; // curResolutionsIndex
    private List<Resolution> predefinedResolutions;

    public GameObject[] Interface;
    public bool isUserInterface = false;

    /// <summary>
    /// 0 : Up
    /// 1 : Down
    /// 2 : Left
    /// 3 : Right
    /// 4 : Shot
    /// 5 : Roll
    /// 6 : Check
    /// 7 : Inventroy
    /// 8 : Map
    /// </summary>
    private KeyCode[] keyBindings = new KeyCode[9]; // 9���� Ű ������ ���� �迭
    private bool isWaitingForKey = false; // Ű �Է� ��� ���¸� ��Ÿ���� �÷���
    private int currentKeyIndex = 0; // ���� ���� ���� Ű�� �ε���

    //PlayerUI
    public Slider reloadSlider; // ������ �����̴� UI


    public TextMeshProUGUI currentResolutionText;

    [SerializeField]
    private string currentPanel = "Default"; // ���� �г��� ����
    private string prevPanel = "Default"; // ���� �г��� ����

    //panel
    /// <summary>
    /// 0 : Up
    /// 1 : Down
    /// </summary>
    public GameObject[] textbarPos;
    public GameObject textbar;
    public TextMeshProUGUI text;
    public Image npcFaceImage;
    
    //gameover UI
    public GameObject gameover_Object;
    public TextMeshProUGUI gameover_text;
    public Image gameover_image;
    public GameObject gameover_soul;
    public Sprite[] soul_sprites;
    // heartbreak sound 87->89
    // heartbreak_c sound 88->90
    // 4,5,6,7 -> ��Ʈ ����
    // 3-> �μ��� 

    // Save UI
    public GameObject savePanel;
    /// <summary>
    /// 0. ����Ϸ�
    /// 1. �̸�
    /// 2. ����
    /// 3. �ð�
    /// 4. ���
    /// 5. ���ư���
    /// 6. ����
    /// </summary>
    public TextMeshProUGUI[] savePanel_texts;
    public GameObject savePanel_soul;
    public GameObject[] save_points;
    public bool isSavePanel = false;
    public int saveNum = 0;
    public bool isSaveDelay = false;
    //inventory panel
    /// <summary>
    /// 0. �κ��丮
    /// 1. ������
    /// 2. ���� 
    /// 3. ��ȭ
    /// </summary>
    public int inventroy_panelNum = 0;
    public int inventroy_curNum = 0;
    public bool isInventroy = false;

    public GameObject inventroy_panel;
    public GameObject item_panel;
    public GameObject stat_panel;
    public GameObject call_panel;

    public TextMeshProUGUI[] inventroy_texts;
    public TextMeshProUGUI[] item_texts;
    public TextMeshProUGUI[] stat_texts;
    public TextMeshProUGUI[] call_texts;
    public GameObject[] inventroy_points;
    public GameObject[] item_points;
    public GameObject[] call_points;
    public Image inventroy_soul;

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
        soundManager = SoundManager.Instance;
        dialogueManager = DialogueManager.Instance;
    }

    private void Start()
    {
        gameover_Object.SetActive(false);
        LoadSettings();
        predefinedResolutions = new List<Resolution>
    {
        new Resolution { width = 640, height = 480 },
        new Resolution { width = 720, height = 480 },
        new Resolution { width = 800, height = 600 },
        new Resolution { width = 1024, height = 768 },
        new Resolution { width = 1280, height = 720 },
        new Resolution { width = 1440, height = 1080 },
        new Resolution { width = 1920, height = 1080 },
        new Resolution { width = 2560, height = 1440 }
    };
        keyBindings[0] = KeyCode.W;
        keyBindings[1] = KeyCode.S;
        keyBindings[2] = KeyCode.A;
        keyBindings[3] = KeyCode.D;
        keyBindings[4] = KeyCode.Mouse0;
        keyBindings[5] = KeyCode.Mouse1;
        keyBindings[6] = KeyCode.F;
        keyBindings[7] = KeyCode.E;
        keyBindings[8] = KeyCode.Tab;
        int screenWidth = 1920;
        int screenHeight = 1080;
        float screenRatio = (float)screenWidth / screenHeight;

        Screen.SetResolution(screenWidth, screenHeight, Screen.fullScreen);

        for (int i = 0; i < mainButtons.Length; i++)
        {
            int index = i;  // Ŭ������ ���� ���� ���纻
            mainButtons[i].onClick.AddListener(() => OnButtonClick(index));
            AddEventTriggerListener(mainButtons[i].gameObject, EventTriggerType.PointerEnter, () => OnButtonHover(index));
        }
        for (int i = 0; i < optionButtons.Length; i++)
        {
            int index = i;  // Ŭ������ ���� ���� ���纻
            optionButtons[i].onClick.AddListener(() => OnButtonClick(index));
            AddEventTriggerListener(optionButtons[i].gameObject, EventTriggerType.PointerEnter, () => OnButtonHover(index));
        }
        for (int i = 0; i < keyChangeButtons.Length; i++)
        {
            int index = i;  // Ŭ������ ���� ���� ���纻
            keyChangeButtons[i].onClick.AddListener(() => OnButtonClick(index));
            AddEventTriggerListener(keyChangeButtons[i].gameObject, EventTriggerType.PointerEnter, () => OnButtonHover(index));
        }
        for (int i = 0; i < YNButtons.Length; i++)
        {
            int index = i;  // Ŭ������ ���� ���� ���纻
            YNButtons[i].onClick.AddListener(() => OnButtonClick(index));
            AddEventTriggerListener(YNButtons[i].gameObject, EventTriggerType.PointerEnter, () => OnButtonHover(index));
        }
        InitHeart();
        InitWeapon();
        ShowPanel("Game");
        OptionInput();
        UpdateUI();
        SaveOff();
    }

    #region savePanel
    public void SaveOpen()
    {
        saveNum = 0;
        isSavePanel = true;
        isSaveDelay = false;
        savePanel.SetActive(true);
        savePanel_soul.SetActive(true);

        savePanel_texts[1].color = new Color(255, 255, 255);
        savePanel_texts[2].color = new Color(255, 255, 255);
        savePanel_texts[3].color = new Color(255, 255, 255);
        savePanel_texts[4].color = new Color(255, 255, 255);


        savePanel_texts[0].gameObject.SetActive(false);
        savePanel_texts[5].gameObject.SetActive(true);
        savePanel_texts[6].gameObject.SetActive(true);

    }

    public void SaveOff()
    {
        saveNum = 0;
        isSavePanel = false;
        savePanel.SetActive(false);
    }

    public void SaveComplete()
    {
        savePanel_soul.SetActive(false);
        saveNum = -1; //�߸�
        isSavePanel = false;
        isSaveDelay = true;
        TextYellow();
        SoundManager.Instance.SFXPlay("save_sound", 171);

        savePanel_texts[5].gameObject.SetActive(false);
        savePanel_texts[6].gameObject.SetActive(false);
        savePanel_texts[0].gameObject.SetActive(true);
        gameManager.Save();
        gameManager.SaveGameTime();
    }
    public void TextYellow()
    {
        savePanel_texts[1].color = new Color(255,255,0);
        savePanel_texts[2].color = new Color(255,255,0);
        savePanel_texts[3].color = new Color(255,255,0);
        savePanel_texts[4].color = new Color(255,255,0);

        savePanel_texts[3].text = gameManager.GetElapsedTimeInMinutes().ToString();
        savePanel_texts[4].text = gameManager.GetMapName();
    }
    #endregion
    public void TextBarOpen()
    {
        // �÷��̾� ��ġ ��������
        Vector3 playerPosition = gameManager.GetPlayerData().position;

        // ī�޶� ��ġ ��������
        Vector3 cameraPosition = Camera.main.transform.position;
        if (playerPosition.y > cameraPosition.y)
        {
            // �÷��̾ ī�޶󺸴� ���� �ִ� ���
            textbar.transform.position = textbarPos[1].transform.position;
        }
        else 
        {
            // �÷��̾ ī�޶󺸴� ���� �ִ� ���
            textbar.transform.position = textbarPos[0].transform.position;
        }
        textbar.SetActive(true);
    }
    public void CloseTextbar()
    {
        textbar.SetActive(false);
    }
    private void Update()
    {
        if (isUserInterface || isInventroy)
        {
            Time.timeScale = 0f;
        }
        else
        {
            gameManager.ResumeGame();
        }

        if (currentIndex > currentButtons.Length)
        {
            OnButtonHover(0);
        }
        if (isWaitingForKey)
        {
            DetectKeyInput();
        }
        UpdateUI();
        OptionInput();

        HandleInput();
        UpdateSoulPosition();

        if (Input.GetKeyDown(KeyCode.E))
        {
            RectTransform transform = gameover_soul.GetComponent<RectTransform>();
            gameover_soul.GetComponent<PieceShooter>().ShootPieces(transform);

        }
        if (isSavePanel)
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                saveNum--;
                if (saveNum < 0)
                {
                    saveNum = save_points.Length - 1; // �迭�� ������ �ε����� ��ȯ
                }
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                saveNum++;
                if (saveNum >= save_points.Length)
                {
                    saveNum = 0; // �迭�� ó������ ��ȯ
                }
            }

            savePanel_soul.gameObject.transform.localPosition = save_points[saveNum].transform.localPosition;

            switch (saveNum)
            {
                case 0:
                    if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Space))
                        SaveComplete();
                    break;

                case 1:
                    if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Space))
                        SaveOff();
                    break;
            }
        }
        else
        {
            if(saveNum == -1)
            {
                if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Space))
                {
                    SaveOff();
                    StartCoroutine(SaveDalay());
                }
            }
        }

    }
    #region InventroyUi
    void OnInventroy()
    {
        isInventroy = true;
    }
    void OffInventroy()
    {
        isInventroy = false;
    }
    void OnPanel(int i)
    {
        item_panel.SetActive(false);
        stat_panel.SetActive(false);
        call_panel.SetActive(false);
        switch (i)
        {
            case 0:
                item_panel.SetActive(true);
                break;

            case 1:
                stat_panel.SetActive(true);
                break;

            case 2:
                call_panel.SetActive(true);
                break;
        }
    }
    void HandleInput()
    {
        if (!isInventroy)
            return;
        // W �Է� �� inventroy_curNum ����
        if (Input.GetKeyDown(KeyCode.W))
        {
            inventroy_curNum--;
            if (inventroy_curNum < 0)
            {
                inventroy_curNum = GetCurrentPanelTextLength() - 1; // ���� �г��� ������ �̵�
            }
        }

        // S �Է� �� inventroy_curNum ����
        if (Input.GetKeyDown(KeyCode.S))
        {
            inventroy_curNum++;
            if (inventroy_curNum >= GetCurrentPanelTextLength())
            {
                inventroy_curNum = 0; // ���� �г��� ó������ �̵�
            }
        }
    }

    int GetCurrentPanelTextLength()
    {
        switch (inventroy_panelNum)
        {
            case 0: // �κ��丮 �г�
                return inventroy_texts.Length;
            case 1: // ������ �г�
                return item_texts.Length;
            case 3: // ��ȭ �г�
                return call_texts.Length;
            default:
                return 0;
        }
    }

    void UpdateSoulPosition()
    {
        GameObject[] currentPoints = null;

        // �гο� �´� ����Ʈ �迭�� ����
        switch (inventroy_panelNum)
        {
            case 0:
                currentPoints = inventroy_points;
                break;
            case 1:
                currentPoints = item_points;
                break;
            case 3: // ���� ����
                currentPoints = call_points;
                break;
        }

        // ���� ���õ� ����Ʈ ��ġ�� inventroy_soul �̵�
        if (currentPoints != null && currentPoints.Length > 0 && inventroy_curNum < currentPoints.Length)
        {
            inventroy_soul.transform.position = currentPoints[inventroy_curNum].transform.position;
        }
    }
    #endregion
    IEnumerator SaveDalay()
    {
        yield return new WaitForSeconds(0.5f);
        isSaveDelay = false;

    }
    public void LoadIntro()
    {
        SceneManager.LoadScene("IntroScene");
    }
    #region KeyBoardUi
    private void DetectKeyInput()
    {
        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keyCode))
            {
                if (keyCode == KeyCode.Escape)
                {
                    // ESC�� ������ �� �Է� ���
                    isWaitingForKey = false;
                    Debug.Log("Key binding cancelled.");
                }
                else
                {
                    // �ٸ� Ű�� ������ �� �ش� Ű�� ����
                    // ������ Ű�� �ִ��� Ȯ��
                    for (int i = 0; i < keyBindings.Length; i++)
                    {
                        if (keyBindings[i] == keyCode)
                        {
                            keyBindings[i] = KeyCode.None; // ���� Ű�� None���� ����
                            break;
                        }
                    }

                    keyBindings[currentKeyIndex] = keyCode;
                    isWaitingForKey = false;
                    SaveKeyBindings();
                    Debug.Log("Key binding set to: " + keyCode);
                }
                CloseKeyCheck();
                UpdateKeyBindingUI();
                break;
            }
        }
    }
    public void StartKeyBinding(int index)
    {
        currentKeyIndex = index;
        isWaitingForKey = true;
        Debug.Log("Press any key to bind to action " + index);
    }

    private void SaveKeyBindings()
    {
        for (int i = 0; i < keyBindings.Length; i++)
        {
            PlayerPrefs.SetString("KeyBinding" + i, keyBindings[i].ToString());
        }
        PlayerPrefs.Save();
    }

    private void LoadKeyBindings()
    {
        for (int i = 0; i < keyBindings.Length; i++)
        {
            string keyString = PlayerPrefs.GetString("KeyBinding" + i, KeyCode.None.ToString());
            keyBindings[i] = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyString);
        }
        UpdateKeyBindingUI();
    }

    private void UpdateKeyBindingUI()
    {
        for (int i = 0; i < keyChangefunctions.Length; i++)
        {
            TextMeshProUGUI buttonText = keyChangefunctions[i].GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                if (keyBindings[i].ToString() == "Mouse0")
                    buttonText.text = "Left Mouse Button";
                else if (keyBindings[i].ToString() == "Mouse1")
                    buttonText.text = "Right Mouse Button";
                else
                    buttonText.text = keyBindings[i].ToString();
            }
        }
    }

    public void OpenKeyCheck()
    {
        KeyCheckPanel.SetActive(true);
    }
    public void CloseKeyCheck()
    {
        KeyCheckPanel.SetActive(false);
    }

    #endregion

    #region playerUi

    void InitHeart() // ü�� ����
    {
        PlayerData player = gameManager.GetPlayerData();
        int heart_count = player.Maxhealth / 2; // �� ü�¿� �´� ��Ʈ ���� ����
        ui_healths = new GameObject[heart_count];

        // ü�¹� ��ü ���̸� �����ϱ� ���� �θ� UI RectTransform
        RectTransform parentRect = ui_positions[0].GetComponent<RectTransform>();

        // �� ��Ʈ�� ���� ���� (���� ����)�� �����մϴ�.
        float totalWidth = parentRect.rect.width; // �θ� UI�� �ʺ�
        float padding = 10f; // ��Ʈ ���� ���� (�ʿ信 ���� ����)
        float heartWidth = (totalWidth - padding * (heart_count - 1)) / heart_count; // �� ��Ʈ�� �ʺ�
        ui_healthImage.SetActive(true);

        for (int i = 0; i < heart_count; i++)
        {
            GameObject heartPrefab = Resources.Load<GameObject>("Prefabs/Heart");
            GameObject instance = Instantiate(heartPrefab, ui_positions[0].transform);

            RectTransform heartRect = instance.GetComponent<RectTransform>();

            // ��Ʈ�� ũ�⸦ �θ� UI �ʺ� ���� �����մϴ�.
            heartRect.sizeDelta = new Vector2(heartWidth, heartRect.sizeDelta.y);

            // ��Ʈ�� ���� �������� ��ġ�մϴ�.
            Vector3 newPosition = instance.transform.position;

            if (!GameManager.Instance.isBattle)
            {
                newPosition.x = parentRect.position.x - totalWidth / 2 + (heartWidth + padding) * i + heartWidth / 2; // ���� ��ġ
            }
            else
            {
                newPosition.x = ui_positions[2].transform.position.x + (heartWidth + padding) * i; // ���� ������ �� ��ġ ����
            }

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
            newPosition.y = ui_positions[1].transform.position.y + i * sizeY * 1.25f; // ���� �������� ��ġ ����
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
        SetCombatMode();
        PlayerData player = gameManager.GetPlayerData();
        int currentHealth = player.health;

        // ü�� �̹��� ������Ʈ
        RectTransform parentRect = ui_positions[0].GetComponent<RectTransform>();
        float totalWidth = parentRect.rect.width; // �θ� UI�� �ʺ�
        float padding = 10f; // ��Ʈ ���� ����
        int heart_count = ui_healths.Length;
        float heartWidth = (totalWidth - padding * (heart_count - 1)) / heart_count; // �� ��Ʈ�� �ʺ�

        for (int i = 0; i < heart_count; i++)
        {
            // ü�¿� ���� ��������Ʈ �ε��� ���
            int spriteIndex = Mathf.Clamp(currentHealth - i * 2, 0, 2);
            ui_healths[i].GetComponent<ImageScript>().SetImage(spriteIndex);

            // ��Ʈ�� ��ġ�� �ٽ� ���� (ü�� UI�� ���ŵ� ������ ��ġ�� �ٽ� ����)
            RectTransform heartRect = ui_healths[i].GetComponent<RectTransform>();
            Vector3 newPosition = ui_healths[i].transform.position;

            if (!GameManager.Instance.isBattle)
            {
                newPosition.x = parentRect.position.x - totalWidth / 2 + (heartWidth + padding) * i + heartWidth / 2; // ���� ��ġ
                ui_healths[i].GetComponent<Image>().color = new Color(1, 1, 1, 0.75f);
            }
            else
            {
                newPosition.x = ui_positions[2].transform.position.x + (heartWidth + padding) * i; // ���� ������ �� ��ġ ����
                ui_healths[i].GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }

            // ��ġ ����
            ui_healths[i].transform.position = newPosition;
        }

        // ���� ������ �������� �� �Ѿ� �̹��� ������Ʈ
        Weapon weapon = gameManager.GetWeaponData();
        int current_magazine = weapon.current_magazine;

        // �Ѿ� �̹��� ������Ʈ
        for (int i = 0; i < ui_ammo.Length; i++)
        {
            int spriteIndex = (i < current_magazine) ? 0 : 1; // �Ѿ� ������ ���� ��������Ʈ �ε��� ���
            ui_ammo[i].GetComponent<ImageScript>().SetImage(spriteIndex);
        }

        // ���� ź�� �ؽ�Ʈ ������Ʈ
        ui_ammoText.text = weapon.current_Ammo + "/" + weapon.maxAmmo;
    }

    public void SetCombatMode()
    {
        if (gameManager.isBattle || GameManager.Instance.GetPlayerData().isInvincible)
        {
            OnPlayerUI(); // ���� ���¿����� UI�� ������

        }
        else
        {
            OffPlayerUI(); // ������ ���¿����� UI�� ����
        }
    }
    // ü��, ���� ���� UI�� ���� �Լ�
    public void OffPlayerUI()
    {
        foreach (var ui in ui_ammo)
        {
            ui.gameObject.SetActive(false);
        }
        foreach (var ui in pedestal)
        {
            ui.gameObject.SetActive(false);
        }
        ui_weaponImage.gameObject.SetActive(false);
        ui_ammoText.gameObject.SetActive(false);
        ui_weaponBackImage.gameObject.SetActive(false);
        ui_healthImage.gameObject.SetActive(false);
    }

    // ü��, ���� ���� UI�� �Ѵ� �Լ�
    public void OnPlayerUI()
    {
        foreach (var ui in ui_ammo)
        {
            ui.gameObject.SetActive(true);
        }
        foreach (var ui in pedestal)
        {
            ui.gameObject.SetActive(true);
        }
        ui_weaponImage.gameObject.SetActive(true);
        ui_ammoText.gameObject.SetActive(true);
        ui_weaponBackImage.gameObject.SetActive(true);
        ui_healthImage.gameObject.SetActive(true);
    }
    #endregion

    #region optionUI

    public void OpenYNReset()
    {
        YN_ResetPanel.SetActive(true);
        ShowPanel("YNCheck");

    }
    public void CloseYNReset()
    {
        YN_ResetPanel.SetActive(false);
    }

    public void SaveSettings()
    {
        // ���� ���� ����
        PlayerPrefs.SetFloat("BGMVolume", bgmScrollbar.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxScrollbar.value);

        // ��üȭ�� ���� ����
        PlayerPrefs.SetInt("IsFullScreen", isFullScreen ? 1 : 0);

        // VSync ���� ����
        PlayerPrefs.SetInt("IsVSyncOn", isVSyncOn ? 1 : 0);

        // Ŀ�� ���� ����
        PlayerPrefs.SetInt("IsCursorVisible", isCursorVisible ? 1 : 0);

        // ��� ���� ����
        PlayerPrefs.SetFloat("Brightness", brightnessScrollbar.value);

        // ��Ÿ ���� ����
        PlayerPrefs.SetFloat("CameraShake", cameraShakeScrollbar.value);
        PlayerPrefs.SetFloat("MiniMapSize", miniMapSizeScrollbar.value);

        // �ػ� �ε��� ����
        PlayerPrefs.SetInt("ResolutionIndex", curRIndex);

        PlayerPrefs.Save();
    }
    public void LoadSettings()
    {
        // ���� ���� �ҷ�����
        if (PlayerPrefs.HasKey("BGMVolume"))
        {
            bgmScrollbar.value = PlayerPrefs.GetFloat("BGMVolume");
        }

        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            sfxScrollbar.value = PlayerPrefs.GetFloat("SFXVolume");
        }

        // ��üȭ�� ���� �ҷ�����
        if (PlayerPrefs.HasKey("IsFullScreen"))
        {
            isFullScreen = PlayerPrefs.GetInt("IsFullScreen") == 1;
            Screen.fullScreen = isFullScreen;
            fullScreenToggle.image.sprite = isFullScreen ? onSprite : offSprite;
        }

        // VSync ���� �ҷ�����
        if (PlayerPrefs.HasKey("IsVSyncOn"))
        {
            isVSyncOn = PlayerPrefs.GetInt("IsVSyncOn") == 1;
            QualitySettings.vSyncCount = isVSyncOn ? 1 : 0;
            vSyncToggle.image.sprite = isVSyncOn ? onSprite : offSprite;
        }

        // Ŀ�� ���� �ҷ�����
        if (PlayerPrefs.HasKey("IsCursorVisible"))
        {
            isCursorVisible = PlayerPrefs.GetInt("IsCursorVisible") == 1;
            Cursor.visible = isCursorVisible;
            cusorToggle.image.sprite = isCursorVisible ? onSprite : offSprite;
        }

        // ��� ���� �ҷ�����
        if (PlayerPrefs.HasKey("Brightness"))
        {
            brightnessScrollbar.value = PlayerPrefs.GetFloat("Brightness");
            // ���⼭ ��� ���� �ڵ带 �߰��ϼ���
        }

        // ��Ÿ ���� �ҷ�����
        if (PlayerPrefs.HasKey("CameraShake"))
        {
            cameraShakeScrollbar.value = PlayerPrefs.GetFloat("CameraShake");
        }

        if (PlayerPrefs.HasKey("MiniMapSize"))
        {
            miniMapSizeScrollbar.value = PlayerPrefs.GetFloat("MiniMapSize");
        }

        // �ػ� �ε��� �ҷ�����
        if (PlayerPrefs.HasKey("ResolutionIndex"))
        {
            curRIndex = PlayerPrefs.GetInt("ResolutionIndex");
            // �ػ� ���� ���� �ڵ� �߰�
        }
    }
    public void ResetSettings()
    {
        PlayerPrefs.DeleteAll();
        // �⺻�� ����
        bgmScrollbar.value = 0.5f; // �⺻ ���� ��
        sfxScrollbar.value = 0.5f;  // �⺻ ���� ��
        isFullScreen = true;
        isVSyncOn = true;
        isCursorVisible = true;
        brightnessScrollbar.value = 0.5f; // �⺻ ��� ��
        cameraShakeScrollbar.value = 0.5f;  // �⺻ ī�޶� ��鸲 ��
        miniMapSizeScrollbar.value = 0.5f;  // �⺻ �̴ϸ� ũ�� ��
        curRIndex = 0; // �⺻ �ػ� �ε���

        keyBindings[0] = KeyCode.W;
        keyBindings[1] = KeyCode.S;
        keyBindings[2] = KeyCode.A;
        keyBindings[3] = KeyCode.D;
        keyBindings[4] = KeyCode.Mouse0;
        keyBindings[5] = KeyCode.Mouse1;
        keyBindings[6] = KeyCode.F;
        keyBindings[7] = KeyCode.E;
        keyBindings[8] = KeyCode.Tab;
        // UI ������Ʈ
        Screen.fullScreen = isFullScreen;
        fullScreenToggle.image.sprite = isFullScreen ? onSprite : offSprite;
        QualitySettings.vSyncCount = isVSyncOn ? 1 : 0;
        vSyncToggle.image.sprite = isVSyncOn ? onSprite : offSprite;
        Cursor.visible = isCursorVisible;
        cusorToggle.image.sprite = isCursorVisible ? onSprite : offSprite;

        // ����
        SaveSettings();
    }
    public void exitGame()
    {
        Debug.Log("Game is exiting...");
        Application.Quit();
    }
    public void ShowPanel(string panelName)
    {
        switch (panelName)
        {
            case "Main":
                currentPanel = "Main";
                isUserInterface = true;
                currentButtons = mainButtons;
                puasePanel.SetActive(true);
                optionPanel.SetActive(false);
                keyChangePanel.SetActive(false);
                break;
            case "Option":
                currentPanel = "Option";
                currentButtons = optionButtons;
                isUserInterface = true;
                puasePanel.SetActive(false);
                optionPanel.SetActive(true);
                keyChangePanel.SetActive(false);
                prevPanel = currentPanel;
                break;
            case "KeyChange":
                currentPanel = "KeyChange";
                currentButtons = keyChangeButtons;
                isUserInterface = true;
                puasePanel.SetActive(false);
                optionPanel.SetActive(false);
                keyChangePanel.SetActive(true);
                prevPanel = currentPanel;
                break;
            case "YNCheck":
                currentPanel = "YNCheck";
                currentButtons = YNButtons;
                break;
            case "Return":
                ShowPanel(prevPanel);
                break;
            default:
                currentPanel = "Game";
                isUserInterface = false;
                currentButtons = mainButtons;
                puasePanel.SetActive(false);
                optionPanel.SetActive(false);
                keyChangePanel.SetActive(false);
                break;
        }
        currentIndex = 0; // �г� ���� �� �ε��� �ʱ�ȭ
        UpdateSelection();
    }
    void Navigate(int direction)
    {
        currentIndex = (currentIndex + direction + currentButtons.Length) % currentButtons.Length;
        UpdateSelection();
    }

    void OptionInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentPanel == "KeyChange")
            {
                ShowPanel("Option");
                CloseYNReset();
                SaveSettings();
            }
            else if (currentPanel == "Option")
            {
                ShowPanel("Main");
                CloseYNReset();
                SaveSettings();
            }
            else if (currentPanel == "Main")
            {
                ShowPanel("Game");
                gameManager.ResumeGame();
                Time.timeScale = 1f;
                soundManager.ResumeBGSound();
                CloseYNReset();
            }
            else if (currentPanel == "YNCheck")
            {
                CloseYNReset();
                ShowPanel("Return");
            }
            else
            {
                ShowPanel("Main");
                Time.timeScale = 0f;
                soundManager.PauseBGSound();
            }
            soundManager.SFXPlay("mus_piano1", 32);
        }

        if (isUserInterface)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                Navigate(-1);
                soundManager.SFXPlay("snd_piano3", 34);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                Navigate(1);
                soundManager.SFXPlay("snd_piano4", 35);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                soundManager.SFXPlay("snd_piano5", 36);
                AdjustValue(-1);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                soundManager.SFXPlay("snd_piano5", 36);
                AdjustValue(1);
            }
            else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                if (currentButtons != null && currentButtons.Length > 0)
                {
                    if (currentPanel == "Option")
                    {
                        if (currentIndex == 1 || currentIndex == 3 || currentIndex == 4)
                            ToggleValue();  // ToggleValue ȣ��
                        else
                        {
                            currentButtons[currentIndex].onClick.Invoke();
                            soundManager.SFXPlay("snd_piano6", 37);
                        }
                    }
                    else
                    {
                        currentButtons[currentIndex].onClick.Invoke();
                        soundManager.SFXPlay("snd_piano6", 37);
                    }
                }
            }
        }
    }

    void AdjustValue(int direction)
    {
        switch (currentIndex)
        {
            case 2: // ��� ����
                brightnessScrollbar.value = Mathf.Clamp(brightnessScrollbar.value + direction * 0.1f, 0, 1);
                break;
            case 5: // BGM ���� ����
                bgmScrollbar.value = Mathf.Clamp(bgmScrollbar.value + direction * 0.1f, 0, 1);
                soundManager.BGSoundVolume(bgmScrollbar.value);
                break;
            case 6: // SFX ���� ����
                sfxScrollbar.value = Mathf.Clamp(sfxScrollbar.value + direction * 0.1f, 0, 1);
                soundManager.SFXSoundVolume(sfxScrollbar.value);
                break;
            case 7: // ī�޶� ��鸲 ����
                cameraShakeScrollbar.value = Mathf.Clamp(cameraShakeScrollbar.value + direction * 0.1f, 0, 1);
                break;
            case 8: // �̴ϸ� ũ�� ����
                miniMapSizeScrollbar.value = Mathf.Clamp(miniMapSizeScrollbar.value + direction * 0.1f, 0, 1);
                break;
            case 0: // �ػ� ����
                ChangeResolution(direction);
                break;
        }
    }

    // 5
    public void SetBGVolume()
    {
        if (soundManager != null)
        {
            soundManager.BGSoundVolume(bgmScrollbar.value);
        }
    }

    public void SetSFXVolume()
    {
        if (soundManager != null)
        {
            soundManager.SFXSoundVolume(sfxScrollbar.value);
        }
    }
    //
    void ToggleValue()
    {
        switch (currentIndex)
        {
            case 1: // ��ü ȭ�� ���
                ToggleFullScreen();
                break;
            case 3: // ���� ����ȭ ���
                ToggleVSync();
                break;
            case 4: // Ŀ�� ���
                ToggleCursorVisibility();
                break;
        }
        StartCoroutine(ForceToggleUpdate());
    }
    void UpdateButtonState(Button button, bool state)
    {
        SpriteState spriteState = button.spriteState;

        if (state)
        {
            button.image.sprite = onSprite;
            spriteState.highlightedSprite = onSprite;
            spriteState.pressedSprite = onSprite;
            spriteState.selectedSprite = onSprite;
            spriteState.disabledSprite = onSprite;
        }
        else
        {
            button.image.sprite = offSprite;
            spriteState.highlightedSprite = offSprite;
            spriteState.pressedSprite = offSprite;
            spriteState.selectedSprite = offSprite;
            spriteState.disabledSprite = offSprite;
        }

        button.spriteState = spriteState;
    }

    void ToggleFullScreen()
    {
        isFullScreen = !isFullScreen;
        Screen.fullScreen = isFullScreen;
        UpdateButtonState(fullScreenToggle, isFullScreen);
    }

    void ToggleVSync()
    {
        isVSyncOn = !isVSyncOn;
        QualitySettings.vSyncCount = isVSyncOn ? 1 : 0;
        UpdateButtonState(vSyncToggle, isVSyncOn);
    }

    void ToggleCursorVisibility()
    {
        isCursorVisible = !isCursorVisible;
        Cursor.visible = isCursorVisible;
        UpdateButtonState(cusorToggle, isCursorVisible);
    }

    IEnumerator ForceToggleUpdate()
    {
        yield return null; // Wait for one frame
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(currentButtons[currentIndex].gameObject);
    }
    void UpdateSelection()
    {
        for (int i = 0; i < currentButtons.Length; i++)
        {
            ColorBlock colors = currentButtons[i].colors;
            colors.normalColor = Color.white; // �⺻ ����
            colors.highlightedColor = Color.white; // ���� ����
            colors.pressedColor = Color.gray; // Ŭ�� �� ����
            colors.selectedColor = (i == currentIndex) ? Color.white : Color.white; // ���õ� ����
            currentButtons[i].colors = colors;

            // �ؽ�Ʈ ���� ����
            TextMeshProUGUI buttonText = currentButtons[i].GetComponentInChildren<TextMeshProUGUI>();
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
        soundManager.SFXPlay("snd_piano2", 33);
    }
    public void ChangeResolution(int direction)
    {
        curRIndex = (curRIndex + direction + predefinedResolutions.Count) % predefinedResolutions.Count;
        Resolution selectedResolution = predefinedResolutions[curRIndex];
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);
        Debug.Log("Resolution changed to: " + selectedResolution.width + " x " + selectedResolution.height);

        // ���� �ػ� �ؽ�Ʈ ������Ʈ
        UpdateCurrentResolutionText();
    }

    void UpdateCurrentResolutionText()
    {
        currentResolutionText.text = Screen.width + " x " + Screen.height;
    }

    //���콺 ȣ�� �̺�Ʈ ������
    void AddEventTriggerListener(GameObject target, EventTriggerType eventType, System.Action action)
    {
        EventTrigger trigger = target.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = target.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = eventType };
        entry.callback.AddListener((eventData) => action());
        trigger.triggers.Add(entry);
    }
    ///<summary>
    /// OnClickEvnet�� �Ȱ��� �ڵ����� , ��ģ���� ���콺�� ���� �÷������� �۵���
    ///</summary>

    void OnButtonHover(int index)
    {
        currentIndex = index;
        UpdateSelection();
    }
    #endregion
    //player UI
    public void ShowReloadSlider(bool show)
    {
        reloadSlider.gameObject.SetActive(show);
    }

    public void SetReloadSliderMaxValue(float maxValue)
    {
        reloadSlider.maxValue = maxValue;
    }

    public void SetReloadSliderValue(float value)
    {
        reloadSlider.value = value;
    }

    /// <summary>
    /// 0 : Up
    /// 1 : Down
    /// 2 : Left
    /// 3 : Right
    /// 4 : Shot
    /// 5 : Roll
    /// 6 : Check
    /// 7 : Inventory
    /// 8 : Map
    /// </summary>
    public KeyCode GetKeyCode(int i)
    {
        // �ε��� ������ Ȯ���Ͽ� ��ȿ�� ��쿡�� ��ȯ
        if (i >= 0 && i < keyBindings.Length)
        {
            return keyBindings[i];
        }

        // ��ȿ���� ���� �ε����� ��� �⺻��(KeyCode.None) ��ȯ
        return KeyCode.None;
    }


    #region gameOver
    public void playGameover()
    {
        gameover_Object.SetActive(true);
        soundManager.StopBGSound();
        soundManager.BGSoundPlayDelayed(4, 3);
        //gameover_Object;
        //gameover_text;
        //gameover_image;
        StartCoroutine(Okdo());
        StartCoroutine(Okdso());
        StartCoroutine(FadeIn());
        StartCoroutine(gameoverTextOn());


    } 
    public void End_And_Load()
    {
        StartCoroutine(FadeOut());
        StartCoroutine(Load_SavePoint());
        // Off gameover
        // last load -> go!

    }
    private IEnumerator Load_SavePoint()
    {
        yield return new WaitForSeconds(5f);
        gameover_Object.SetActive(false);
        gameManager.Load();

    }
    private IEnumerator gameoverTextOn()
    {
        yield return new WaitForSeconds(4f);
        gameover_text.gameObject.SetActive(true);
        dialogueManager.StartGameOverDialogue(0);

    }
    private IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(3f);
        float duration = 2f; // 2 seconds
        float currentTime = 0f;

        // Get the current color of the image
        Color color = gameover_image.color;
        color.a = 0f; // Start with alpha at 0
        gameover_image.color = color;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(currentTime / duration); // Alpha value from 0 to 1

            // Update the alpha of the image color
            color.a = alpha;
            gameover_image.color = color;

            yield return null;
        }

        // Ensure the alpha is fully set to 1 after the loop
        color.a = 1f;
        gameover_image.color = color;
    }
    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(0.8f);
        gameover_text.gameObject.SetActive(false);
        float duration = 2f; // 2�� ���� ����
        float currentTime = 0f;

        // ���� �̹��� ���� ��������
        Color color = gameover_image.color;
        color.a = 1f; // ������ �� ���İ��� 1�� ����
        gameover_image.color = color;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(1 - (currentTime / duration)); // ���� ���� 1���� 0����

            // �̹��� ������ ���İ� ������Ʈ
            color.a = alpha;
            gameover_image.color = color;

            yield return null;
        }

        // �ݺ����� ���� �� ���� ���� 0���� ������ ����
        color.a = 0f;
        gameover_image.color = color;
    }

    private IEnumerator Okdo()
    {
        yield return new WaitForSeconds(0.1f);

        gameover_soul.GetComponent<Image>().sprite = soul_sprites[1];
        soundManager.SFXPlay("heartbreak1", 87);
    }
    private IEnumerator Okdso()
    {
        yield return new WaitForSeconds(1f);

        gameover_soul.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        soundManager.SFXPlay("heartbreak2", 89);
        RectTransform transform = gameover_soul.GetComponent<RectTransform>();
        gameover_soul.GetComponent<PieceShooter>().ShootPieces(transform);

    }
   
    #endregion
}

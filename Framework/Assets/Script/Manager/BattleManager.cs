using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class BossDialogue
{
    public string text;
    public string expression;
    public string attack; // ���� ����
    public string direction; // ���� (Left, Right ��)
    public string eventType; // Ư�� �̺�Ʈ
    public string music; // ���� ����
    public int skipToDialogue; // Ư�� ���� �̵�
}

[System.Serializable]
public class BossBattleData
{
    public int bossID;
    public string name;
    public List<BossDialogue> dialogues;
}

[System.Serializable]
public class BattleDatabase
{
    public List<BossBattleData> bossBattles;
}

public class BattleManager : MonoBehaviour
{
    public Queue<BossDialogue> boss_sentences;
    [SerializeField]
    private BattleDatabase battleDatabase;
    private static BattleManager instance;
    public GameObject battleObject;
    Animator battleAnimator;
    GameManager gameManager;

    public enum BattleState { None, BasicBattle, BossBattle }  // ���� ���¸� ����
    public BattleState currentState;

    public GameObject[] roomPrefabs;  // ���� ���� �� ������ �迭
    public Transform roomSpawnPoint;


    public GameObject[] enemyPrefabs;  // �� ������ �迭
    public Room currentRoom;  // ���� ��

    public Transform[] bulletPoints;
    public Transform nonePoint;
    public GameObject floweybulletprefab;

    public TextAsset bossDataJson;
    //�׽�Ʈ�� �Ƹ��� ���߿��� �迭�� �ϵ��� �������� ���� ������� �ҵ�
    //�ϴ� �̰� Ʃ�� ������ 
    public GameObject Boss_AllObject;
    public GameObject Boss_Face;
    public GameObject Boss_Face_UI;
    public GameObject Boss_Textbar;
    public TextMeshProUGUI Boss_Text;
    public GameObject battlePoint;
    public int test_curboss = 0;
    [SerializeField]
    private List<GameObject> activeBullets = new List<GameObject>(); // ���� Ȱ��ȭ�� �Ѿ� ���

    public TypeEffect currentTypeEffect;
    
    //�÷��̾�
    public PlayerMovement player;
    //���� ��
    // ���� �� �����ϴ� "���� Ȱ��ȭ�� ��" ����Ʈ
    public List<GameObject> curEnemies = new List<GameObject>();
    [SerializeField]
    private List<Transform> bulletSpawnTransforms = new List<Transform>();

    private Vector2 prevPos;

    private BossBattleData currentBoss;
    private int currentDialogueIndex = 0;

    public GameObject bulletPointPrefab; // ������ ������ (����Ƽ �ν����Ϳ��� ����)
    public Transform spawnParent;   // ������ ������Ʈ�� ���� �θ� ������Ʈ (������)

    public static BattleManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<BattleManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("BattleManager");
                    instance = obj.AddComponent<BattleManager>();
                }
            }
            return instance;
        }
    }
    public bool isTalking = false;
    private bool isFirstInteraction = true; // ó�� ��ȭ���� Ȯ��
    public bool isEvent = false;
    public int boss_id;

    private void Awake()
    {
        gameManager = GameManager.Instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        battleAnimator = battleObject.GetComponent<Animator>();
        battleObject.SetActive(true);
        boss_sentences = new Queue<BossDialogue>();

        // �ʼ� ���� �ʱ�ȭ
        isTalking = false;
        isFirstInteraction = true;
        isEvent = false;
        test_curboss = 0;
        GenerateGrid(40.60f, 59.54f, 5, 6.00f, -4.05f, 7);
        LoadBossData();
    }
    private void GenerateGrid(float xStart, float xEnd, int xCount, float yStart, float yEnd, int yCount)
    {
        float[] xPositions = GeneratePositions(xStart, xEnd, xCount);
        float[] yPositions = GeneratePositions(yStart, yEnd, yCount);

        for (int i = 0; i < xCount; i++)
        {
            for (int j = 0; j < yCount; j++)
            {
                Vector3 spawnPosition = new Vector3(xPositions[i], yPositions[j], 0);
                GameObject obj = Instantiate(bulletPointPrefab, Vector3.zero, Quaternion.identity, spawnParent);
                obj.transform.position = spawnPosition;  // ��ġ ����
                obj.name = $"Bullet_{i}_{j}"; // �̸� ����

                bulletSpawnTransforms.Add(obj.transform); // Transform ����
            }
        }
    }

    private float[] GeneratePositions(float start, float end, int count)
    {
        float[] positions = new float[count];

        for (int i = 0; i < count; i++)
        {
            positions[i] = Mathf.Lerp(start, end, (float)i / (count - 1));
        }

        return positions;
    }
    public void BattleSetting()
    {
        Boss_AllObject.SetActive(true);
        prevPos = player.transform.position;
        player.TeleportPlayer(battlePoint.transform.position);
        Boss_Face.gameObject.SetActive(true);
        Boss_Text.gameObject.SetActive(true);
        gameManager.ChangeGameState(GameState.Fight);
        isTalking = true;


    }
    public void BattleReSetting()
    {
        Boss_AllObject.SetActive(false);
       // player.TeleportPlayer(prevPos);
        Boss_Text.gameObject.SetActive(false);
        gameManager.ChangeGameState(GameState.None);
        Boss_Textbar.SetActive(false);
        EndDialogue();
    }

    void Update()
    {
        HandleInteraction();
    }
    public void BattleStart(int eventNumber)
    {

        gameManager.isBattle = true;
        // ���� ���
        SoundManager.Instance.SFXPlay("BattleStart", 0);
        // ���� �ִϸ��̼� ����
        battleAnimator.SetTrigger("BattleStart");

        // �ڷ�ƾ���� ������ �������� ����
        prevPos = player.transform.position;
        
        player.TeleportPlayer(battlePoint.transform.position);
        StartCoroutine(StartBattleAfterDelay(eventNumber, 1.5f));
    }
    private void LoadBossData()
    {
        if (bossDataJson != null)
        {
            battleDatabase = JsonUtility.FromJson<BattleDatabase>(bossDataJson.text);
        }
        else
        {
            Debug.LogError("Boss data JSON is not assigned.");
        }
    }

    public void StartBossBattle(int bossID)
    {
        if (battleDatabase == null || battleDatabase.bossBattles == null)
        {
            Debug.LogError("Battle database is empty.");
            return;
        }

        currentBoss = battleDatabase.bossBattles.Find(boss => boss.bossID == bossID);
        if (currentBoss == null)
        {
            Debug.LogError($"Boss with ID {bossID} not found.");
            return;
        }
        curEnemies.Add(enemyPrefabs[0]);
        currentDialogueIndex = 0;
        DisplayNextDialogue();
    }

    private void HandleInteraction()
    {

        if (isTalking && !UIManager.Instance.isInventroy)
        {
            // ��ȭ �߿��� ��ȣ�ۿ� ����
            UIManager.Instance.isInventroy = false;

            // Ÿ���� ȿ�� ���� ���
            if (IsEffecting())
            {
                if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Space))
                {
                    SkipTypeEffect();
                    UIManager.Instance.isSaveDelay = true;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Space))
            {
                DisplayNextDialogue();
            }

            return; // ��ȭ ���̹Ƿ� ������ ��ȣ�ۿ��� ó������ ����
        }


    }
    public void StartDialogue(int bossID)
    {
        boss_sentences.Clear();
        isTalking = true;

        // ���� ������ �˻�
        currentBoss = FindBossBattle(bossID);
        if (currentBoss == null)
        {
            Debug.LogError($"Boss with ID {bossID} not found.");
            return;
        }

        // ��� �����͸� ť�� �߰�
        foreach (var dialogue in currentBoss.dialogues)
        {
            boss_sentences.Enqueue(new BossDialogue
            {
                text = dialogue.text,
                expression = dialogue.expression,
                attack = dialogue.attack,
                direction = dialogue.direction,
                eventType = dialogue.eventType,
                music = dialogue.music,
                skipToDialogue = dialogue.skipToDialogue,
            });
        }

        // ù ��° ��� ���
        DisplayNextDialogue();
    }

    private void DisplayNextDialogue()
    {
        if (boss_sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        var dialogue = boss_sentences.Dequeue();
        // Ư�� �̺�Ʈ ó��
        if (!string.IsNullOrEmpty(dialogue.eventType))
        {
            HandleSpecialEvent(dialogue.eventType, dialogue.text);
        }
        else
        {
            currentTypeEffect.SetMsg(dialogue.text, OnSentenceComplete, 100, dialogue.expression);

            // ǥ�� ����
            SetBossExpression(dialogue.expression);
        }
        // ���� ���� ����
        if (!string.IsNullOrEmpty(dialogue.attack))
        {
            ExecuteAttack(dialogue.attack);
        }

       

        // Ư�� ���� �ǳʶٱ� ó��
        if (currentBoss.dialogues[currentDialogueIndex].skipToDialogue > 0)
        {
            currentDialogueIndex = currentBoss.dialogues[currentDialogueIndex].skipToDialogue - 1;
            DisplayNextDialogue(); // �ǳʶ� ���� ��� �̵�
        }
        else
        {
            currentDialogueIndex++;
        }
    }


    private BossBattleData FindBossBattle(int bossID)
    {
        if (battleDatabase == null || battleDatabase.bossBattles == null)
        {
            Debug.LogError("Battle database is not loaded or empty.");
            return null;
        }

        foreach (var bossBattle in battleDatabase.bossBattles)
        {
            if (bossBattle.bossID == bossID)
            {
                return bossBattle;
            }
        }

        Debug.LogWarning($"No boss battle found for Boss ID: {bossID}");
        return null;
    }

    private void OnSentenceComplete()
    {
        SetBossExpression("Default");
        Debug.Log("���� ������ �Ϸ�Ǿ����ϴ�.");
        SetBossExpression("Restare");
    }

    public void EndDialogue()
    {
        isTalking = false;
        isEvent = false;
        isFirstInteraction = true;
    }
    public bool IsEffecting()
    {
        return currentTypeEffect != null && currentTypeEffect.IsEffecting();
    }

    public void SkipTypeEffect()
    {
        if (currentTypeEffect != null && currentTypeEffect.IsEffecting())
        {
            currentTypeEffect.Skip();
        }
    }
  
    private void SetBossExpression(string expression) //@@@ test ��
    {
        Debug.Log($"���� ǥ�� : {expression}");
        // �ִϸ��̼� Ʈ���� ����
        if (!string.IsNullOrEmpty(expression))
        {
            Animator animator = null; // ���� ���� ����
            switch (test_curboss)
            {
                case 0:
                    animator = Boss_Face.GetComponent<Animator>();
                    animator?.SetTrigger(expression);
                    break;
                case 1:
                default:
                    animator = Boss_Face_UI.GetComponent<Animator>();
                    animator?.SetTrigger(expression);
                    break;

            }
        }
    }
    #region Attack
    //MoveBulletsInDirection
    //HomingBullets
    //SpiralBullets
    //SplitBullets
    private void ExecuteAttack(string attack)
    {
        switch (attack)
        {
            case "Attack1":
                Debug.Log("Executing Attack 1");
                // SpawnAndMoveBullets();
                SetAttack("Directional");//����
                SetAttack("Spiral");//ȸ����
                SetAttack("Split");//�п�
                SetAttack("Homing");//����
                SetAttack("Homing");
                break;

            case "Attack2":
                Debug.Log("Executing Attack 2");

              //  MoveBulletsToPlayer(true);  // accelerate = true
                break;

            case "Attack3":
                Debug.Log("���� ����");
                //SpawnAndMoveBullets();
               // MoveBulletsInDirection(Vector2.left,10);
                break;


            case "Attack4":
                Debug.Log("����");
               // SpawnAndMoveBullets();
                //StartCoroutine(HomingBullets(10.5f, 8f));

                break;

            case "Attack5":
                Debug.Log("ȸ����");
               // SpawnAndMoveBullets();
               // StartCoroutine(SpiralBullets(120, 2.5f));

                break;

            case "Attack6":
                Debug.Log("�п�");
               // SpawnAndMoveBullets();
               // StartCoroutine(SplitBullets(10, 8f));

                break;

            default:
                Debug.LogWarning($"Unknown attack pattern: {attack}");
                break;
        }
    }

    void SetAttack(string attack,int bulletpoint = 0)
    {
        switch (attack)
        {
            case "Directional":
                SpawnBullets(BulletType.Directional, bulletpoint);
                break;
            case "FixedPoint":
                SpawnBullets(BulletType.FixedPoint, bulletpoint);
                break;
            case "Normal":
                SpawnBullets(BulletType.Normal, bulletpoint);
                break;
            case "Homing":
                SpawnBullets(BulletType.Homing, bulletpoint);
                break;
            case "Spiral":
                SpawnBullets(BulletType.Spiral, bulletpoint);
                break;
            case "Split":
                SpawnBullets(BulletType.Split, bulletpoint);
                break;
            default:
                Debug.LogWarning($"Unknown attack pattern: {attack}");
                break;
        }
    }
    // �Ѿ��� �����ϰ� Ư�� Ÿ���� ������ �����ϴ� �޼���
    void SpawnBullets(BulletType bulletType,int n =0)
    {
        foreach (Transform bulletSpawnPoint in bulletPoints)
        {
            GameObject bullet = Instantiate(floweybulletprefab, bulletSpawnPoint.position, Quaternion.identity);
            BulletController bulletController = bullet.GetComponent<BulletController>();

            if (bulletController != null)
            {
                Vector2 direction = (gameManager.GetPlayerData().position - bulletSpawnPoint.position).normalized;
                bulletController.InitializeBullet(direction, 5f, 0f, 10, 15f, bulletType, bulletSpawnTransforms[n]);
                activeBullets.Add(bulletController.gameObject);
            }
        }
    }



#endregion
    private void HandleSpecialEvent(string eventType,string dialogue)
    {
        switch (eventType)
        {
            case "ChangeSoul":
                currentTypeEffect.Clear();
                gameManager.GetPlayerData().player.GetComponent<PlayerMovement>().EnableSoul(0.7f);
                gameManager.GetPlayerData().player.GetComponent<PlayerMovement>().MakePlayerTransparent();
                SetBossExpression("Sink");  // ������ �ִϸ��̼� 'Sink'�� ����
                test_curboss = 1;
                Boss_Face_UI.SetActive(true);
                SetBossExpression("Appear");
                //ingame sink Ű��, ui Ű��
                StartCoroutine(FloweyAnimationThenNextDialogue(dialogue,1.5f));

                break;

            case "tutorialShot":

                //@@@�����Ұ�
                gameManager.GetPlayerData().player.GetComponent<PlayerMovement>().tutorialDontShot = false;
                break;

            case "EndDialogue":
                // ������ ������ ǥ�� Oh�� ǥ������ 0.3�ʵ� "�Ѿ�"�̶�� ��縦 "ģ��"�� ���� 
                Debug.Log("Ending dialogue.");

                break;

            case "LowerTone":
                // ������ ���� ������ ����
                Debug.Log("Changing music to lower tone.");
                SoundManager.Instance.PlayMusic("LowerTone");  // ������ �����ϴ� ����
                break;

            case "AttackWithBullet":
                // ��� �� �Ѿ˷� �����ϴ� �̺�Ʈ ó��
                Debug.Log("Attacking with bullet after dialogue.");
                ExecuteAttack("Attack1");  // ���÷� Attack1�� ����
                break;

            case "CreepFace":
                // �Ҹ��� �ִϸ��̼��� ���� ������, creep_face�� ��ȯ
                Debug.Log("Slowing down sound and switching to creep face.");
                // ������ ������ �ϰų� �ִϸ��̼��� �����ϴ� ���� �߰�
                SoundManager.Instance.SlowDownMusic();  // ������ ������ �����ϴ� ����
                SetBossExpression("Smile");  // ������ ǥ���� 'CreepFace'�� ����
                Debug.Log("Revelation after 1 second.");
                StartCoroutine(HandleFinalRevelation());
                break;

            default:
                Debug.LogWarning($"Unknown special event: {eventType}");
                break;
        }
    }
    // �ö��� �ִϸ��̼��� ���� �� ���� ��縦 �����ϴ� �ڷ�ƾ
    private IEnumerator FloweyAnimationThenNextDialogue(string dialogue, float waitTime)
    {
        // waitTime ���� ���(�ִϸ��̼��� ���� ������ Ȥ�� �˳��� ��Ƶ� �ð�)
        yield return new WaitForSeconds(waitTime);
        SetBossExpression("Talking");
        currentTypeEffect.SetMsg(dialogue, OnSentenceComplete, 100);
    }
    private IEnumerator HandleFinalRevelation()
    {
        yield return new WaitForSeconds(1f);  // 1�� ��ٸ���
        Debug.Log("��ġë�ٴ� ���: ���� �װ� �� �ϴ��� �˰ڴ�.");
        // �߰����� ��糪 �ִϸ��̼� ó��
    }


    private IEnumerator StartBattleAfterDelay(int eventNumber, float delay)
    {
        // ������ �ð� ���� ���
        yield return new WaitForSeconds(delay);

        // �⺻ ���� Ȥ�� ���� ���� ����
        if (eventNumber == 1)
        {
            StartBasicBattle();
        }
        else if (eventNumber == 2)
        {
            StartBossBattle();
            BattleSetting();
            StartCoroutine(DelayDialogue(2, 1f));
        }
        UIManager.Instance.OnPlayerUI(); // ���� ���¿����� UI�� ������
    }

    private IEnumerator DelayDialogue(int eventNumber, float delay)
    {
        yield return new WaitForSeconds(delay);
        Boss_Textbar.SetActive(true);
        StartDialogue(eventNumber);
    }
    // �⺻ ���� ����
    void StartBasicBattle()
    {
        currentState = BattleState.BasicBattle;
        gameManager.ChangeGameState(GameState.Fight);

        // �������� ���� ����
        //int randomRoomIndex = Random.Range(0, roomPrefabs.Length);
        //currentRoom = Instantiate(roomPrefabs[randomRoomIndex], roomSpawnPoint.position, Quaternion.identity).GetComponent<Room>();

        // ���� ����
        SpawnEnemies();
    }

    // ���� ���� ����
    void StartBossBattle()
    {
        currentState = BattleState.BossBattle;
        gameManager.ChangeGameState(GameState.Fight);
        // ���� �� ����, �̵�?

    }

    // �� ���� ����
    void SpawnEnemies()
    {
        if (currentRoom != null)
        {
            currentRoom.SpawnEnemies(enemyPrefabs);
            //GameObject spawnedEnemy = Instantiate(enemyPrefabs[0], somePosition, Quaternion.identity);
            //curEnemies.Add(spawnedEnemy);
        }
        else
        {
            Debug.LogWarning("Current Room is not set.");
        }
    }
    // ���� ����ϰų� ������� �� ����Ʈ���� ����
    public void RemoveEnemy(GameObject enemy)
    {
        //BattleManager.Instance.RemoveEnemy(this.gameObject)
        if (curEnemies.Contains(enemy))
        {
            curEnemies.Remove(enemy);
        }
    }
}

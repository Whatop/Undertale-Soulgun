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
    public Queue<SentenceData> boss_sentences;
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

    public TextAsset bossDataJson;
    //�׽�Ʈ�� �Ƹ��� ���߿��� �迭�� �ϵ��� �������� ���� ������� �ҵ�
    //�ϴ� �̰� Ʃ�� ������ 
    public GameObject Boss_AllObject;
    public GameObject Boss_Face;
    public TextMeshProUGUI Boss_Text;
    public GameObject battlePoint;

    public TypeEffect currentTypeEffect;
    public GameObject Boss_Wall;

    public PlayerMovement player;
    private Vector2 prevPos;

    private BossBattleData currentBoss;
    private int currentDialogueIndex = 0;
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
        boss_sentences = new Queue<SentenceData>();

        // �ʼ� ���� �ʱ�ȭ
        isTalking = false;
        isFirstInteraction = true;
        isEvent = false;
        LoadBossData();
    }
    public void BattleSetting()
    {
        Boss_AllObject.SetActive(true);
        StartDialogue(1);
        prevPos = player.transform.position;
        player.TeleportPlayer(battlePoint.transform.position);


    }
    void BattleReSetting()
    {
        Boss_AllObject.SetActive(false);
        player.TeleportPlayer(prevPos);
    }

    void Update()
    {
        HandleInteraction();
    }
    public void BattleStart(int eventNumber)
    {
        gameManager.GetPlayerData().player.GetComponent<PlayerMovement>().EnableSoul();
        gameManager.isBattle = true;
        // ���� ���
        SoundManager.Instance.SFXPlay("BattleStart", 0);
        // ���� �ִϸ��̼� ����
        battleAnimator.SetTrigger("BattleStart");

        // �ڷ�ƾ���� ������ �������� ����
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

        currentDialogueIndex = 0;
        DisplayNextDialogue();
    }

    private void DisplayNextDialogue()
    {
        if (currentBoss == null || currentDialogueIndex >= currentBoss.dialogues.Count)
        {
            Debug.Log("All dialogues completed.");
            return;
        }

        var dialogue = currentBoss.dialogues[currentDialogueIndex];
        Boss_Text.text = dialogue.text;

        // ǥ�� ����
        SetBossExpression(dialogue.expression);

        // ���� ���� ����
        if (!string.IsNullOrEmpty(dialogue.attack))
        {
            ExecuteAttack(dialogue.attack);
        }

        // Ư�� �̺�Ʈ ó��
        if (!string.IsNullOrEmpty(dialogue.eventType))
        {
            HandleSpecialEvent(dialogue.eventType);
        }


        // Ư�� ���� �ǳʶٱ�
        if (dialogue.skipToDialogue > 0)
        {
            currentDialogueIndex = dialogue.skipToDialogue - 1; // �ε����� 0���� ����
        }
        else
        {
            currentDialogueIndex++;
        }

       
    }
    private void HandleInteraction()
    {

        if (isTalking)
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
                DisplayNextSentence();
            }

            return; // ��ȭ ���̹Ƿ� ������ ��ȣ�ۿ��� ó������ ����
        }

      
    }
    public void StartDialogue(int id, bool isEvent = false)
    {
        boss_id = id;
        boss_sentences.Clear();


        DisplayNextSentence(id);
    }

    public void DisplayNextSentence(int eventNumber = -1)
    {
        if (boss_sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        SentenceData sentenceData = boss_sentences.Dequeue();

        // �ؽ�Ʈ ���
        currentTypeEffect.SetMsg(sentenceData.text, OnSentenceComplete, eventNumber, sentenceData.expression);
    }
    private void OnSentenceComplete()
    {
        Debug.Log("���� ������ �Ϸ�Ǿ����ϴ�.");
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
        private void SetBossExpression(string expression)
    {
        Debug.Log($"Setting boss expression to: {expression}");
        // �ִϸ��̼� Ʈ���� ����
        if (!string.IsNullOrEmpty(expression))
        {
            var animator = Boss_Face.GetComponent<Animator>();
            animator?.SetTrigger(expression);
        }
    }

    private void ExecuteAttack(string attack)
    {
        switch (attack)
        {
            case "Attack1":
                Debug.Log("Executing Attack 1");
                // Attack 1�� ���� ��ü���� ���� �߰�
                break;

            case "Attack2":
                Debug.Log("Executing Attack 2");
                // Attack 2�� ���� ��ü���� ���� �߰�
                break;

            default:
                Debug.LogWarning($"Unknown attack pattern: {attack}");
                break;
        }
    }

    private void HandleSpecialEvent(string eventType)
    {
        switch (eventType)
        {
            case "MoveToReceiveFriendly":
                Debug.Log("Moving to receive friendly items.");
                // �÷��̾� �̵� ���� ó�� ����
                break;

            case "EndDialogue":
                Debug.Log("Ending dialogue.");
                currentDialogueIndex = currentBoss.dialogues.Count; // ��ȭ ����
                break;

            default:
                Debug.LogWarning($"Unknown special event: {eventType}");
                break;
        }
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
        }
        UIManager.Instance.OnPlayerUI(); // ���� ���¿����� UI�� ������

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
        // ī�޶� ����



        // ���� ���� ����
        NextPatturn();
    }
    
    void NextPatturn()
    {

    }
    // �� ���� ����
    void SpawnEnemies()
    {
        if (currentRoom != null)
        {
            currentRoom.SpawnEnemies(enemyPrefabs);
        }
        else
        {
            Debug.LogWarning("Current Room is not set.");
        }
    }
}

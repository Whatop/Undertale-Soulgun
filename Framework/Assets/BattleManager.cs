using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleManager : MonoBehaviour
{
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

    //�׽�Ʈ�� �Ƹ��� ���߿��� �迭�� �ϵ��� �������� ���� ������� �ҵ�
    //�ϴ� �̰� Ʃ�� ������ 
    public GameObject Boss_AllObject;
    public GameObject Boss_Face;
    public TextMeshProUGUI Boss_Text;

    public GameObject Boss_Wall;

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

    private void Awake()
    {
        gameManager = GameManager.Instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        battleAnimator = battleObject.GetComponent<Animator>();
        battleObject.SetActive(true);
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

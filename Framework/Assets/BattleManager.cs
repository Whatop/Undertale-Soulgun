using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public Transform[] enemySpawnPoints;  // ���� ������ ��ġ��

    public GameObject bossRoomPrefab;  // ���� �� ������
    public Transform bossSpawnPoint;

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
    }

    public void BattleStart(int eventNumber)
    {
        battleAnimator.SetTrigger("BattleStart");
        gameManager.isBattle = true;
        SoundManager.Instance.SFXPlay("BattleStart", 1);
    }
    void SpawnEnemies()
    {
        foreach (var spawnPoint in enemySpawnPoints)
        {
            int randomEnemy = Random.Range(0, enemyPrefabs.Length);
            Instantiate(enemyPrefabs[randomEnemy], spawnPoint.position, Quaternion.identity);
        }
    }

}

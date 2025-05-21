using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    // �̱��� ����
    public static EffectManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject); // Scene ��ȯ �ÿ��� ����
    }

    [System.Serializable]
    public class EffectPool
    {
        public string effectName; // ����Ʈ �̸�
        public GameObject prefab; // ������ ����
        public int poolSize; // �ʱ� ���� ��
    }

    public List<EffectPool> effectPools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (var pool in effectPools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.poolSize; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.effectName, objectPool);
        }
    }

    public GameObject SpawnEffect(string effectName, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(effectName))
        {
            Debug.LogWarning($"Effect '{effectName}' not found in the pool!");
            return null;
        }

        var objectToSpawn = poolDictionary[effectName].Dequeue();
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        // ��ü Ǯ�� �ٽ� ��ȯ
        poolDictionary[effectName].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
    public GameObject SpawnEffect(string effectName, Vector3 position, Quaternion rotation, Color color)
    {
        if (!poolDictionary.ContainsKey(effectName))
        {
            Debug.LogWarning($"Effect '{effectName}' not found in the pool!");
            return null;
        }

        var obj = poolDictionary[effectName].Dequeue();
        obj.transform.position = position;
        obj.transform.rotation = rotation;

        var soulEffect = obj.GetComponent<SoulEffectObject>();
        if (soulEffect != null)
        {
            soulEffect.SetColor(color);
        }
        obj.SetActive(true);

        poolDictionary[effectName].Enqueue(obj);
        return obj;

    }

}

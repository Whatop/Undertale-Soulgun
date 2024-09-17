using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    // ���� ������ ��ġ�� ��� �迭
    public Transform[] enemySpawnPoints;

    // �ش� �濡�� ���� �����ϴ� �޼���
    public void SpawnEnemies(GameObject[] enemyPrefabs)
    {
        foreach (var spawnPoint in enemySpawnPoints)
        {
            // �� �������� �������� ����
            int randomIndex = Random.Range(0, enemyPrefabs.Length);
            // ���õ� �������� ���� ��ġ�� ����
            Instantiate(enemyPrefabs[randomIndex], spawnPoint.position, Quaternion.identity);
        }
    }

    // ��� �� ���� ����Ʈ�� ��ȯ (�ʿ�� ���)
    public Transform[] GetSpawnPoints()
    {
        return enemySpawnPoints;
    }
}

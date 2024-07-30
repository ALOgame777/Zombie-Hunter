using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnZombie : MonoBehaviour
{
    public GameObject enemyPrefab; // ������ ������
    public Vector3 patrolCenter; // ���� �߽�
    public float patrolRadius; // ���� ������

    void Start()
    {
        SpawnEnemy();
    }

    void SpawnEnemy()
    {
        // �� ���� ���� ��ġ ���
        Vector2 newPos = Random.insideUnitCircle * patrolRadius;
        Vector3 spawnPosition = patrolCenter + new Vector3(newPos.x, 0, newPos.y);

        // ������ ����
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}
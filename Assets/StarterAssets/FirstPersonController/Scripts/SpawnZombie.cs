using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnZombie : MonoBehaviour
{
    public GameObject enemyPrefab; // 생성할 프리팹
    public Vector3 patrolCenter; // 원의 중심
    public float patrolRadius; // 원의 반지름

    void Start()
    {
        SpawnEnemy();
    }

    void SpawnEnemy()
    {
        // 원 안의 랜덤 위치 계산
        Vector2 newPos = Random.insideUnitCircle * patrolRadius;
        Vector3 spawnPosition = patrolCenter + new Vector3(newPos.x, 0, newPos.y);

        // 프리팹 생성
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}
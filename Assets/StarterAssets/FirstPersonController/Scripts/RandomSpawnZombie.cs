using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawnZombie : MonoBehaviour
{
    public GameObject enemyPrefab; // 생성할 프리팹
    public float delayTime = 10.0f; // 생성 주기

    float currentTime = 0;

    public Vector3 patrolCenter; // 원의 중심
    public float patrolRadius; // 원의 반지름

    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime > delayTime)
        {
            // 원 안의 랜덤 위치 계산
            Vector2 newPos = Random.insideUnitCircle * patrolRadius;
            Vector3 spawnPosition = patrolCenter + new Vector3(newPos.x, 0, newPos.y);

            // 프리팹 생성
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

            // 시간 초기화
            currentTime = 0;
        }
    }
   
}
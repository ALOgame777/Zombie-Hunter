using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawnTree : MonoBehaviour
{
    public GameObject enemyPrefab; // ������ ������
    public float delayTime = 30.0f; // ���� �ֱ�

    float currentTime = 0;

    public Vector3 patrolCenter; // ���� �߽�
    public float patrolRadius; // ���� ������

    private void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime > delayTime)
        {
            // �� ���� ���� ��ġ ���
            Vector2 newPos = Random.insideUnitCircle * patrolRadius;
            Vector3 spawnPosition = patrolCenter + new Vector3(newPos.x, 0, newPos.y);

            // ������ ����
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

            // �ð� �ʱ�ȭ
             currentTime = 0;
        }
    }

}

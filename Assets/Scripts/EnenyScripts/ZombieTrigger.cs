// 상자 없어지게 시도... 성공~~~~~~~
using UnityEngine;

public class ZombieTrigger : MonoBehaviour
{
    public GameObject zombiePrefab; // 생성할 좀비 프리팹
    public Transform[] spawnPoints; // 좀비가 생성될 위치들

    public float triggerRadius = 2.0f; // 플레이어가 트리거에 들어왔는지 확인할 반경
    private Transform playerTransform; // 플레이어의 위치

    void Start()
    {
        // 플레이어의 Transform을 찾기
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        // 플레이어가 트리거 반경 내에 있는지 확인
        if (playerTransform != null && Vector3.Distance(playerTransform.position, transform.position) < triggerRadius)
        {
            // 좀비 생성
            foreach (Transform spawnPoint in spawnPoints)
            {
                Instantiate(zombiePrefab, spawnPoint.position, spawnPoint.rotation);
            }

            // 트리거 박스 제거
            Debug.Log("Destroying ZombieTrigger");
            Destroy(gameObject);
        }
    }
}




//// 좀비는 생기는데, 상자가 안없어짐ㅠㅠ

//using UnityEngine;

//public class ZombieTrigger : MonoBehaviour
//{
//    public GameObject zombiePrefab; // 생성할 좀비 프리팹

//    public Transform[] SpawnPoint;
//    public Transform spawnPoint1;    // 좀비가 생성될 위치
//    public Transform spawnPoint2;   // 좀비 생성 위치 2
//    public Transform spawnPoint3;
//    public Transform spawnPoint4;
//    public Transform spawnPoint5;
//    public Transform spawnPoint6;
//    public Transform spawnPoint7;

//    void OnTriggerEnter(Collider other)
//    {
//        // 플레이어와 충돌했을 때
//        if (other.CompareTag("Player"))
//        {
//            // 좀비 생성
//            //Instantiate()
//            Instantiate(zombiePrefab,spawnPoint1.position, spawnPoint1.rotation);
//            Instantiate(zombiePrefab,spawnPoint2.position, spawnPoint2.rotation);
//            Instantiate(zombiePrefab,spawnPoint3.position, spawnPoint3.rotation);
//            Instantiate(zombiePrefab,spawnPoint4.position, spawnPoint4.rotation);
//            Instantiate(zombiePrefab,spawnPoint5.position, spawnPoint5.rotation);
//            Instantiate(zombiePrefab,spawnPoint6.position, spawnPoint6.rotation);
//            Instantiate(zombiePrefab,spawnPoint7.position, spawnPoint7.rotation);

//            // 트리거 박스 제거
//           // print("제거 완");
//            Destroy(gameObject);

//        }
//    }
//}

// ���� �������� �õ�... ����~~~~~~~
using UnityEngine;

public class ZombieTrigger : MonoBehaviour
{
    public GameObject zombiePrefab; // ������ ���� ������
    public Transform[] spawnPoints; // ���� ������ ��ġ��

    public float triggerRadius = 2.0f; // �÷��̾ Ʈ���ſ� ���Դ��� Ȯ���� �ݰ�
    private Transform playerTransform; // �÷��̾��� ��ġ

    void Start()
    {
        // �÷��̾��� Transform�� ã��
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        // �÷��̾ Ʈ���� �ݰ� ���� �ִ��� Ȯ��
        if (playerTransform != null && Vector3.Distance(playerTransform.position, transform.position) < triggerRadius)
        {
            // ���� ����
            foreach (Transform spawnPoint in spawnPoints)
            {
                Instantiate(zombiePrefab, spawnPoint.position, spawnPoint.rotation);
            }

            // Ʈ���� �ڽ� ����
            Debug.Log("Destroying ZombieTrigger");
            Destroy(gameObject);
        }
    }
}




//// ����� ����µ�, ���ڰ� �Ⱦ������Ф�

//using UnityEngine;

//public class ZombieTrigger : MonoBehaviour
//{
//    public GameObject zombiePrefab; // ������ ���� ������

//    public Transform[] SpawnPoint;
//    public Transform spawnPoint1;    // ���� ������ ��ġ
//    public Transform spawnPoint2;   // ���� ���� ��ġ 2
//    public Transform spawnPoint3;
//    public Transform spawnPoint4;
//    public Transform spawnPoint5;
//    public Transform spawnPoint6;
//    public Transform spawnPoint7;

//    void OnTriggerEnter(Collider other)
//    {
//        // �÷��̾�� �浹���� ��
//        if (other.CompareTag("Player"))
//        {
//            // ���� ����
//            //Instantiate()
//            Instantiate(zombiePrefab,spawnPoint1.position, spawnPoint1.rotation);
//            Instantiate(zombiePrefab,spawnPoint2.position, spawnPoint2.rotation);
//            Instantiate(zombiePrefab,spawnPoint3.position, spawnPoint3.rotation);
//            Instantiate(zombiePrefab,spawnPoint4.position, spawnPoint4.rotation);
//            Instantiate(zombiePrefab,spawnPoint5.position, spawnPoint5.rotation);
//            Instantiate(zombiePrefab,spawnPoint6.position, spawnPoint6.rotation);
//            Instantiate(zombiePrefab,spawnPoint7.position, spawnPoint7.rotation);

//            // Ʈ���� �ڽ� ����
//           // print("���� ��");
//            Destroy(gameObject);

//        }
//    }
//}

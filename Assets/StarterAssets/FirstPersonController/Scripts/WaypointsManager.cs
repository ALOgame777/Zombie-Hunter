////1��° ��� - ��Ȱ��ȭ/Ȱ��ȭ --- �׳� ��ü Ȱ��ȭ�� ��������
//using UnityEngine;

//public class WayPointParent : MonoBehaviour
//{
//    // 'way' ������Ʈ�� �迭�� ����
//    public GameObject[] waypoints; // 'way1'~'way20'�� �ִ� �迭
//    private int currentWaypointIndex = 0; // ���� Ȱ��ȭ�� ��������Ʈ �ε���

//    void Start()
//    {
//        // ó������ 'way1'�� Ȱ��ȭ
//        ActivateWaypoint(currentWaypointIndex);
//    }

//    void OnTriggerEnter(Collider other) // �÷��̾ ��������Ʈ�� ���� �� ȣ���
//    {
//        if (other.CompareTag("Player")) // �÷��̾����� Ȯ��
//        {
//            // ���� ��������Ʈ�� ��Ȱ��ȭ
//            DeactivateWaypoint(currentWaypointIndex);
//            currentWaypointIndex++; // ���� ��������Ʈ�� �̵�

//            // ���� ��������Ʈ�� �ִ��� Ȯ��
//            if (currentWaypointIndex < waypoints.Length)
//            {
//                ActivateWaypoint(currentWaypointIndex); // ���� ��������Ʈ Ȱ��ȭ
//            }
//        }
//    }

//    void ActivateWaypoint(int index)
//    {
//        waypoints[index].SetActive(true); // �ش� �ε����� ��������Ʈ Ȱ��ȭ
//    }

//    void DeactivateWaypoint(int index)
//    {
//        waypoints[index].SetActive(false); // �ش� �ε����� ��������Ʈ ��Ȱ��ȭ
//    }
//}

//// 2��° ��� - Ȱ��ȭ/��Ȱ��ȭ --- 1���� Ȱ��ȭ �Ǳ��ѵ� ��������
//using UnityEngine;

//public class WaypointsManager : MonoBehaviour
//{
//    public GameObject[] waypoints; // way1���� way20���� ���� �迭
//    private int currentWaypointIndex = 0; // ���� Ȱ��ȭ�� ��������Ʈ�� �ε���

//    void Start()
//    {
//        // ��� ��������Ʈ�� ��Ȱ��ȭ�ϰ� ù ��° ��������Ʈ�� Ȱ��ȭ
//        foreach (GameObject waypoint in waypoints)
//        {
//            waypoint.SetActive(false);
//        }
//        waypoints[currentWaypointIndex].SetActive(true);
//    }

//    void Update()
//    {
//        // ������ �׽�Ʈ�� �ڵ�: �÷��̾ �����̽� Ű�� ������ ���� ��������Ʈ�� ��Ҵٰ� ����
//        if (Input.GetKeyDown(KeyCode.Space))
//        {
//            OnWaypointReached();
//        }
//    }

//    void OnWaypointReached()
//    {
//        // ���� ��������Ʈ ��Ȱ��ȭ
//        waypoints[currentWaypointIndex].SetActive(false);

//        // ���� ��������Ʈ�� �ִ� ��� Ȱ��ȭ
//        currentWaypointIndex++;
//        if (currentWaypointIndex < waypoints.Length)
//        {
//            waypoints[currentWaypointIndex].SetActive(true);
//        }
//    }
//}

using System.Collections.Generic;
using UnityEngine;

public class WaypointsManager : MonoBehaviour
{
    public List<GameObject> waypoints; // way1���� way20���� ���� ����Ʈ
    private int currentWaypointIndex = 0; // ���� Ȱ��ȭ�� ��������Ʈ�� �ε���

    void Start()
    {
        // ��� ��������Ʈ�� ��Ȱ��ȭ�ϰ� ù ��° ��������Ʈ�� Ȱ��ȭ
        foreach (GameObject waypoint in waypoints)
        {
            waypoint.SetActive(false);
        }
        if (waypoints.Count > 0)
        {
            waypoints[currentWaypointIndex].SetActive(true);
        }
    }

   

    public void OnWaypointReached()
    {
        // ���� ��������Ʈ�� ����
        waypoints[currentWaypointIndex].SetActive(false);

        // ���� ��������Ʈ�� �ִ� ��� Ȱ��ȭ
        currentWaypointIndex++;
        if (currentWaypointIndex < waypoints.Count)
        {
            waypoints[currentWaypointIndex].SetActive(true);
        }
    }
}



//// 4��° ��� - ������ �̵�
//using UnityEngine;

//public class WaypointsManager : MonoBehaviour
//{
//    public Transform[] waypointPositions; // way1���� way20���� ��ġ�� ���� �迭
//    private int currentWaypointIndex = 0; // ���� Ȱ��ȭ�� ��������Ʈ�� �ε���
//    public GameObject waypoint; // �̵���ų ��������Ʈ ������Ʈ

//    void Start()
//    {
//        // ù ��° ��������Ʈ ��ġ�� �̵�
//        waypoint.transform.position = waypointPositions[currentWaypointIndex].position;
//    }

//    void Update()
//    {
//        // ������ �׽�Ʈ�� �ڵ�: �÷��̾ �����̽� Ű�� ������ ���� ��������Ʈ�� ��Ҵٰ� ����
//        if (Input.GetKeyDown(KeyCode.Space))
//        {
//            OnWaypointReached();
//        }
//    }

//    void OnWaypointReached()
//    {
//        // ���� ��������Ʈ�� �ִ� ��� ��ġ�� �̵�
//        currentWaypointIndex++;
//        if (currentWaypointIndex < waypointPositions.Length)
//        {
//            waypoint.transform.position = waypointPositions[currentWaypointIndex].position;
//        }
//        else
//        {
//            waypoint.SetActive(false); // ��� ��������Ʈ�� �� ����� ��� ��Ȱ��ȭ
//        }
//    }
//}

// 5��° ��� - ��Ȱ��ȭ/Ȱ��ȭ
//using UnityEngine;

//public class WaypointsManager : MonoBehaviour
//{
//    public GameObject[] waypoints; // way1���� way20���� ���� �迭
//    private int currentWaypointIndex = 0; // ���� Ȱ��ȭ�� ��������Ʈ�� �ε���

//    void Start()
//    {
//        // ��� ��������Ʈ�� ��Ȱ��ȭ�ϰ� ù ��° ��������Ʈ�� Ȱ��ȭ
//        foreach (GameObject waypoint in waypoints)
//        {
//            waypoint.SetActive(false);
//        }
//        waypoints[currentWaypointIndex].SetActive(true);
//    }

//    public void OnTriggerEnter(Collider other)
//    {
//        // �÷��̾ ��������Ʈ�� ��Ҵ��� Ȯ��
//        if (other.gameObject.CompareTag("Player") && other.gameObject == waypoints[currentWaypointIndex])
//        {
//            OnWaypointReached();
//        }
//    }

//    void OnWaypointReached()
//    {
//        // ���� ��������Ʈ ��Ȱ��ȭ
//        waypoints[currentWaypointIndex].SetActive(false);

//        // ���� ��������Ʈ�� �ִ� ��� Ȱ��ȭ
//        currentWaypointIndex++;
//        if (currentWaypointIndex < waypoints.Length)
//        {
//            waypoints[currentWaypointIndex].SetActive(true);
//        }
//    }
//}
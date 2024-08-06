// ��������Ʈ ȸ�� �� �Ԥ�
using UnityEngine; // ����Ƽ���� �ʿ��� �͵��� ������

public class WayPoint : MonoBehaviour // ť�긦 �����ϴ� Ŭ������
{
    public float rotationSpeed = 50f; // ť�갡 ȸ���ϴ� �ӵ���
    public float moveSpeed = 1f; // ť�갡 ���Ʒ��� �����̴� �ӵ���
    public float moveAmount = 0.5f; // ť�갡 ���Ʒ��� �󸶳� �����̴����� ����
    private Vector3 startPosition; // ť���� �ʱ� ��ġ�� �����ϴ� ������

    // 'way' ������Ʈ�� �迭�� ����
    public GameObject[] waypoints; // 'way1'~'way20'�� �ִ� �迭
    private int currentWaypointIndex = 0; // ���� Ȱ��ȭ�� ��������Ʈ �ε���

    void Start() // ������ ���۵� �� �� �� �����
    {
        startPosition = transform.position; // ť���� ó�� ��ġ�� ������
        
        // ó������ 'way1'�� Ȱ��ȭ
        ActivateWaypoint(currentWaypointIndex);
    }

    void Update() // �� �����Ӹ��� �����
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime); // ť�긦 ��� ȸ������

        float newY = startPosition.y + Mathf.Sin(Time.time * moveSpeed) * moveAmount; // ť���� ���ο� y ��ġ�� �����
        transform.position = new Vector3(transform.position.x, newY, transform.position.z); // ť�긦 ���ο� ��ġ�� �Ű�
    }
    void OnTriggerEnter(Collider other) // �÷��̾ ��������Ʈ�� ���� �� ȣ���
    {
        if (other.CompareTag("Player")) // �÷��̾����� Ȯ��
        {
            // ���� ��������Ʈ�� ��Ȱ��ȭ
            DeactivateWaypoint(currentWaypointIndex);
            currentWaypointIndex++; // ���� ��������Ʈ�� �̵�

            // ���� ��������Ʈ�� �ִ��� Ȯ��
            if (currentWaypointIndex < waypoints.Length)
            {
                ActivateWaypoint(currentWaypointIndex); // ���� ��������Ʈ Ȱ��ȭ
            }
        }
    }

    void ActivateWaypoint(int index)
    {
        waypoints[index].SetActive(true); // �ش� �ε����� ��������Ʈ Ȱ��ȭ
    }

    void DeactivateWaypoint(int index)
    {
        waypoints[index].SetActive(false); // �ش� �ε����� ��������Ʈ ��Ȱ��ȭ
    }
}


//// ��������Ʈ ȸ�� �� �Ԥ�
//using UnityEngine; // ����Ƽ���� �ʿ��� �͵��� ������

//public class WayPoint : MonoBehaviour // ť�긦 �����ϴ� Ŭ������
//{
//    public float rotationSpeed = 50f; // ť�갡 ȸ���ϴ� �ӵ���
//    public float moveSpeed = 1f; // ť�갡 ���Ʒ��� �����̴� �ӵ���
//    public float moveAmount = 0.5f; // ť�갡 ���Ʒ��� �󸶳� �����̴����� ����
//    private Vector3 startPosition; // ť���� �ʱ� ��ġ�� �����ϴ� ������

//    void Start() // ������ ���۵� �� �� �� �����
//    {
//        startPosition = transform.position; // ť���� ó�� ��ġ�� ������
//    }

//    void Update() // �� �����Ӹ��� �����
//    {
//        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime); // ť�긦 ��� ȸ������

//        float newY = startPosition.y + Mathf.Sin(Time.time * moveSpeed) * moveAmount; // ť���� ���ο� y ��ġ�� �����
//        transform.position = new Vector3(transform.position.x, newY, transform.position.z); // ť�긦 ���ο� ��ġ�� �Ű�
//    }
//}

// 1��° ��� - ��Ȱ��ȭ/Ȱ��ȭ
//using UnityEngine;

//public class WaypointController : MonoBehaviour
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

// 2��° ��� - Ȱ��ȭ/��Ȱ��ȭ
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

// 3��° ��� - ������ ����
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
//        // ���� ��������Ʈ�� ����
//        Destroy(waypoints[currentWaypointIndex]);

//        // ���� ��������Ʈ�� �ִ� ��� Ȱ��ȭ
//        currentWaypointIndex++;
//        if (currentWaypointIndex < waypoints.Length)
//        {
//            waypoints[currentWaypointIndex].SetActive(true);
//        }
//    }
//}


// 4��° ��� - ������ �̵�
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

////1번째 방법 - 비활성화/활성화 --- 그냥 전체 활성화에 안지워짐
//using UnityEngine;

//public class WayPointParent : MonoBehaviour
//{
//    // 'way' 오브젝트를 배열로 저장
//    public GameObject[] waypoints; // 'way1'~'way20'을 넣는 배열
//    private int currentWaypointIndex = 0; // 현재 활성화된 웨이포인트 인덱스

//    void Start()
//    {
//        // 처음에는 'way1'만 활성화
//        ActivateWaypoint(currentWaypointIndex);
//    }

//    void OnTriggerEnter(Collider other) // 플레이어가 웨이포인트에 닿을 때 호출됨
//    {
//        if (other.CompareTag("Player")) // 플레이어인지 확인
//        {
//            // 현재 웨이포인트를 비활성화
//            DeactivateWaypoint(currentWaypointIndex);
//            currentWaypointIndex++; // 다음 웨이포인트로 이동

//            // 다음 웨이포인트가 있는지 확인
//            if (currentWaypointIndex < waypoints.Length)
//            {
//                ActivateWaypoint(currentWaypointIndex); // 다음 웨이포인트 활성화
//            }
//        }
//    }

//    void ActivateWaypoint(int index)
//    {
//        waypoints[index].SetActive(true); // 해당 인덱스의 웨이포인트 활성화
//    }

//    void DeactivateWaypoint(int index)
//    {
//        waypoints[index].SetActive(false); // 해당 인덱스의 웨이포인트 비활성화
//    }
//}

//// 2번째 방법 - 활성화/비활성화 --- 1번만 활성화 되긴한데 안지워짐
//using UnityEngine;

//public class WaypointsManager : MonoBehaviour
//{
//    public GameObject[] waypoints; // way1부터 way20까지 담을 배열
//    private int currentWaypointIndex = 0; // 현재 활성화된 웨이포인트의 인덱스

//    void Start()
//    {
//        // 모든 웨이포인트를 비활성화하고 첫 번째 웨이포인트만 활성화
//        foreach (GameObject waypoint in waypoints)
//        {
//            waypoint.SetActive(false);
//        }
//        waypoints[currentWaypointIndex].SetActive(true);
//    }

//    void Update()
//    {
//        // 간단한 테스트용 코드: 플레이어가 스페이스 키를 누르면 현재 웨이포인트에 닿았다고 가정
//        if (Input.GetKeyDown(KeyCode.Space))
//        {
//            OnWaypointReached();
//        }
//    }

//    void OnWaypointReached()
//    {
//        // 현재 웨이포인트 비활성화
//        waypoints[currentWaypointIndex].SetActive(false);

//        // 다음 웨이포인트가 있는 경우 활성화
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
    public List<GameObject> waypoints; // way1부터 way20까지 담을 리스트
    private int currentWaypointIndex = 0; // 현재 활성화된 웨이포인트의 인덱스

    void Start()
    {
        // 모든 웨이포인트를 비활성화하고 첫 번째 웨이포인트만 활성화
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
        // 현재 웨이포인트를 제거
        waypoints[currentWaypointIndex].SetActive(false);

        // 다음 웨이포인트가 있는 경우 활성화
        currentWaypointIndex++;
        if (currentWaypointIndex < waypoints.Count)
        {
            waypoints[currentWaypointIndex].SetActive(true);
        }
    }
}



//// 4번째 방법 - 닿으면 이동
//using UnityEngine;

//public class WaypointsManager : MonoBehaviour
//{
//    public Transform[] waypointPositions; // way1부터 way20까지 위치를 담을 배열
//    private int currentWaypointIndex = 0; // 현재 활성화된 웨이포인트의 인덱스
//    public GameObject waypoint; // 이동시킬 웨이포인트 오브젝트

//    void Start()
//    {
//        // 첫 번째 웨이포인트 위치로 이동
//        waypoint.transform.position = waypointPositions[currentWaypointIndex].position;
//    }

//    void Update()
//    {
//        // 간단한 테스트용 코드: 플레이어가 스페이스 키를 누르면 현재 웨이포인트에 닿았다고 가정
//        if (Input.GetKeyDown(KeyCode.Space))
//        {
//            OnWaypointReached();
//        }
//    }

//    void OnWaypointReached()
//    {
//        // 다음 웨이포인트가 있는 경우 위치를 이동
//        currentWaypointIndex++;
//        if (currentWaypointIndex < waypointPositions.Length)
//        {
//            waypoint.transform.position = waypointPositions[currentWaypointIndex].position;
//        }
//        else
//        {
//            waypoint.SetActive(false); // 모든 웨이포인트를 다 통과한 경우 비활성화
//        }
//    }
//}

// 5번째 방법 - 비활성화/활성화
//using UnityEngine;

//public class WaypointsManager : MonoBehaviour
//{
//    public GameObject[] waypoints; // way1부터 way20까지 담을 배열
//    private int currentWaypointIndex = 0; // 현재 활성화된 웨이포인트의 인덱스

//    void Start()
//    {
//        // 모든 웨이포인트를 비활성화하고 첫 번째 웨이포인트만 활성화
//        foreach (GameObject waypoint in waypoints)
//        {
//            waypoint.SetActive(false);
//        }
//        waypoints[currentWaypointIndex].SetActive(true);
//    }

//    public void OnTriggerEnter(Collider other)
//    {
//        // 플레이어가 웨이포인트에 닿았는지 확인
//        if (other.gameObject.CompareTag("Player") && other.gameObject == waypoints[currentWaypointIndex])
//        {
//            OnWaypointReached();
//        }
//    }

//    void OnWaypointReached()
//    {
//        // 현재 웨이포인트 비활성화
//        waypoints[currentWaypointIndex].SetActive(false);

//        // 다음 웨이포인트가 있는 경우 활성화
//        currentWaypointIndex++;
//        if (currentWaypointIndex < waypoints.Length)
//        {
//            waypoints[currentWaypointIndex].SetActive(true);
//        }
//    }
//}
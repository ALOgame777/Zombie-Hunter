// 일단 돈으로 문 열기 + e 키로 문 열기
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FirstDOOR : MonoBehaviour
{
    public GameObject ButtonUI;
    public GameObject DOOR;
    public Button opendoor; // 문 여는 버튼
    public float showDistance = 7.0f; // 버튼이 보이는 거리
    private Transform playerposition; // 플레이어 위치 추적
    private CanvasGroup Doorcanvasgroup; // 투명도와 상호작용 관리하는 캔버스

    private int playerMoney = 1000; // 플레이어의 초기 돈, 예시로 1000원 설정

    // Start is called before the first frame update
    void Start()
    {
        Doorcanvasgroup = opendoor.GetComponent<CanvasGroup>(); // 문 열기에서 캔버스그룹 찾기

        GameObject player = GameObject.FindGameObjectWithTag("Player"); // 플레이어를 찾아서 플레이어트랜스폼에 저장
        if (player != null)
        {
            playerposition = player.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerposition != null)
        {
            // 플레이어와 오브젝트 사이 거리를 계산
            float distance = Vector3.Distance(playerposition.position, transform.position);

            // 거리가 쇼거리보다 낮으면
            if (distance <= showDistance)
            {
                // 보여주기
                ShowSlider();

                // 'E' 키를 누르면 문을 열도록 함
                if (Input.GetKeyDown(KeyCode.E))
                {
                    TryOpenDoor();
                    HideSlider();
                }
            }
            else
            {
                // 숨기기
                HideSlider();
            }
        }
    }

    private void ShowSlider() // 슬라이더 보여주기
    {
        Doorcanvasgroup.alpha = 1;
        Doorcanvasgroup.interactable = true;
        Doorcanvasgroup.blocksRaycasts = true;
    }

    private void HideSlider() // 슬라이더 숨기기
    {
        Doorcanvasgroup.alpha = 0;
        Doorcanvasgroup.interactable = false;
        Doorcanvasgroup.blocksRaycasts = false;
    }

    private void TryOpenDoor()
    {
        // 플레이어의 돈이 700원 이상인지 확인
        if (playerMoney >= 700)
        {
            // 700원 차감
            playerMoney -= 700;

            // 문 열기
            DestroyDoor();
        }
        else
        {
            Debug.Log("돈이 부족합니다. 문을 열 수 없습니다.");
        }
    }

    public void DestroyDoor()
    {
        // 문 오브젝트 제거
        Destroy(gameObject);
    }
}


//// 문을 통과하는 버그 고치고(trigger) 문 버튼 안뜨는거 수정했는데 (거리 문제) 문 버튼을 누를 수가 없네~~~~~
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class FirstDOOR : MonoBehaviour
//{
//    public GameObject ButtonUI;
//    public GameObject DOOR;
//    public Button opendoor; // 문 여는 버튼
//    public float showDistance = 7.0f; // 버튼이 보이는 거리
//    private Transform playerposition; // 플레이어 위치 추적
//    private CanvasGroup Doorcanvasgroup; // 투명도와 상호작용 관리하는 캔버스


//    // Start is called before the first frame update
//    void Start()
//    {
//        Doorcanvasgroup = opendoor.GetComponent<CanvasGroup>(); // 문 열기에서 캔버스그룹 찾기

//        GameObject player = GameObject.FindGameObjectWithTag("Player"); // 플레이어를 찾아서 플레이어트랜스폼에 저장
//        if (player != null)
//        {
//            playerposition = player.transform;
//        }
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (playerposition != null)
//        {

//            // 플레이어와 오브젝트 사이 거리를 계산
//            float distance = Vector3.Distance(playerposition.position, transform.position);

//            // 거리가 쇼거리 보다 낮으면
//            if (distance <= showDistance)
//            {
//                // 보여주기
//                ShowSlider();
//            }
//            else
//            {
//                //숨기기
//                HideSlider();
//            }
//        }
//    }
//    private void ShowSlider() // 슬라이더 보여주고 창문 고치기
//    {
//        Doorcanvasgroup.alpha = 1;
//        Doorcanvasgroup.interactable = true;
//        Doorcanvasgroup.blocksRaycasts = true;
//    }

//    private void HideSlider() // 슬라이더 숨기기
//    {
//        Doorcanvasgroup.alpha = 0;
//        Doorcanvasgroup.interactable = false;
//        Doorcanvasgroup.blocksRaycasts = false;
//    }
//    private void OnCollisionStay(Collision asdf)
//    {
//        if (asdf.gameObject.CompareTag("Player"))
//        {
//            IncreaseSliderOverTime();
//        }
//    }


//    private void IncreaseSliderOverTime()
//    {
//        // 버튼 ui 생성
//        ButtonUI.SetActive(true);
//    }

//    public void DestroyDoor()
//    {
//        Destroy(gameObject);
//    }

//}

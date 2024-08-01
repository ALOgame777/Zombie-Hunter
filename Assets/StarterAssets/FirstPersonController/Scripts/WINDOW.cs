// 음 한번 다시 해보자 ---------- 가까이 가면 슬라이더 생성 + 1칸씩 회복 UI 구현 완료~~~
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class WINDOW : MonoBehaviour
{
    public Slider progressSlider; // UI 슬라이더
    public float incrementAmount = 1.0f; // 슬라이더 증가량
     float incrementDuration = 1f; // 증가 시간
    public float showDistance = 2.0f; // 슬라이더가 보이는 거리

    private CanvasGroup sliderCanvasGroup; // 투명도와 상호작용 관리하는 캔버스
    private Transform playerTransform; // 플레이어 위치 추적

    void Start()
    {
        sliderCanvasGroup = progressSlider.GetComponent<CanvasGroup>(); // 슬라이더에서 슬라이더캔버스그룹 찾기
        if (sliderCanvasGroup == null)
        {
            Debug.LogError("CanvasGroup component not found on the Slider.");
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player"); // 플레이어를 찾아서 플레이어트랜스폼에 저장
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player object not found. Make sure the player has the 'Player' tag.");
        }
    }

    void Update()
    {

        if (playerTransform != null) 
        {
            
        // 플레이어와 오브젝트 사이 거리를 계산
            float distance = Vector3.Distance(playerTransform.position, transform.position);
            
            // 거리가 쇼거리 보다 낮으면
            if (distance <= showDistance)
            { 
                // 보여주기
                ShowSlider();
            }
            else
            { 
                //숨기기
                HideSlider();
            }
        }
    }

    private void ShowSlider() // 슬라이더 보여주고 창문 고치기
    {
        sliderCanvasGroup.alpha = 1;
        sliderCanvasGroup.interactable = true;
        sliderCanvasGroup.blocksRaycasts = true;
    }

    private void HideSlider() // 슬라이더 숨기기
    {
        sliderCanvasGroup.alpha = 0;
        sliderCanvasGroup.interactable = false;
        sliderCanvasGroup.blocksRaycasts = false;
    }

    private void OnTriggerStay(Collider other) // 플레이어가 트리거에 있으면 계속해서 코루틴 시작
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(IncreaseSliderOverTime());
        }
    }

    IEnumerator IncreaseSliderOverTime() // 슬라이더 값을 증가
    {
        // 현재 슬라이더 값에 targetvalue와 incrementamount를 더한 값으로 설정 
        float targetValue = progressSlider.value + incrementAmount;
        float elapsedTime = 0;

        // elapsedtime을 증가 시키면서 슬라이더 값을 러프로 서서히 targetvalue로 변경
        while (elapsedTime < incrementDuration)
        {
            progressSlider.value = Mathf.Lerp(progressSlider.value, targetValue, elapsedTime / incrementDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        progressSlider.value = targetValue;
    }
}


////슬라이더 가까이가면 올라간다 얍얍얍!
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class WINDOW : MonoBehaviour
//{
//    public Slider progressSlider; // UI 슬라이더
//    public float incrementAmount = 1f; // 슬라이더 증가량
//    public float incrementDuration = 1.0f; // 증가 시간

//    private void OnTriggerStay(Collider other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            StartCoroutine(IncreaseSliderOverTime());
//        }
//    }

//    IEnumerator IncreaseSliderOverTime()
//    {
//        float targetValue = progressSlider.value + incrementAmount;
//        float elapsedTime = 0;

//        while (elapsedTime < incrementDuration)
//        {
//            progressSlider.value = Mathf.Lerp(progressSlider.value, targetValue, elapsedTime / incrementDuration);
//            elapsedTime += Time.deltaTime;
//            yield return null;
//        }

//        progressSlider.value = targetValue;
//    }

//}



// 아무것도 마음대로 안돼요
//public float speed = 6.0f; // 이동 속도
//public Slider progressSlider; // UI 슬라이더
//public float incrementAmount = 0.1f; // 슬라이더 증가량
//public float stopDuration = 1.0f; // 멈추는 시간
//private bool isStopped = false; // 플레이어가 멈춰있는지 여부
//private CharacterController characterController;

//void Start()
//{
//    characterController = GetComponent<CharacterController>();
//}

//void Update()
//{
//    if (!isStopped)
//    {
//        // 플레이어의 이동 로직
//        float moveHorizontal = Input.GetAxis("Horizontal");
//        float moveVertical = Input.GetAxis("Vertical");

//        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
//        characterController.Move(movement * speed * Time.deltaTime);
//    }
//}

//void OnTriggerEnter(Collider other)
//{
//    if (other.CompareTag("Box"))
//    {
//        StartCoroutine(IncreaseSliderOverTime());
//    }
//}

//IEnumerator IncreaseSliderOverTime()
//{
//    isStopped = true;
//    float targetValue = progressSlider.value + incrementAmount;
//    float elapsedTime = 0;

//    while (elapsedTime < stopDuration)
//    {
//        progressSlider.value = Mathf.Lerp(progressSlider.value, targetValue, (elapsedTime / stopDuration));
//        elapsedTime += Time.deltaTime;
//        yield return null;
//    }

//    progressSlider.value = targetValue;
//    isStopped = false;
//}
//private void OnTriggerStay(Collider other)
//{

//    if (other.gameObject.CompareTag("Player"))
//    {
//        REPAIR();  // 플레이어와 충돌 시 수리 메서드 호출
//    }
//}
//void REPAIR()
//{





//}

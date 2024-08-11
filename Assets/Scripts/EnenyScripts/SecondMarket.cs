//// 2번째 자판기 해보자... // 구매 할거 깜빡거리는거 추가 // 러프 추가? 아쉽게도 안됨.
//using System.Collections;
//using UnityEngine;
//using UnityEngine.UI;

//public class SecondMarket : MonoBehaviour
//{
//    // 버튼 UI와 도어 오브젝트를 위한 변수
//    public GameObject ButtonUI;
//   // public GameObject DOOR;

//    // 상점 입장 버튼 및 UI 요소들
//    public Button Enter; // 상점 입장 버튼
//    public GameObject StoreUI; // 상점 UI
//    public GameObject supply; // 보급품 오브젝트
//    public GameObject BUYCON; // 구매 확인 UI

//    // 버튼이 보이는 거리와 플레이어 위치 추적을 위한 변수
//    public float showDistance = 3.0f;
//    private Transform playerposition; // 플레이어 위치 추적
//    private CanvasGroup SecondMarketCanvas; // 상점 입장 버튼의 CanvasGroup

//    // 구매 여부 확인 변수
//    private bool hasPurchased = false;

//    // 깜빡거리는 효과를 위한 RawImage
//    [SerializeField] private RawImage rawImage;

//    void Start()
//    {
//        // UI 비활성화
//        StoreUI.gameObject.SetActive(false);
//        BUYCON.gameObject.SetActive(false);

//        // 상점 입장 버튼의 CanvasGroup 컴포넌트 찾기
//        SecondMarketCanvas = Enter.GetComponent<CanvasGroup>();

//        // 플레이어를 찾아서 플레이어 트랜스폼에 저장
//        GameObject player = GameObject.FindGameObjectWithTag("Player");
//        if (player != null)
//        {
//            playerposition = player.transform;
//        }
//        // CanvasGroup 초기화
//        SecondMarketCanvas.alpha = 0;
//        SecondMarketCanvas.interactable = false;
//        SecondMarketCanvas.blocksRaycasts = false;
//    }

//    void Update()
//    {
//        if (playerposition != null)
//        {
//            // 플레이어와 상점 사이의 거리 계산
//            float distance = Vector3.Distance(playerposition.position, transform.position);

//            // 거리가 showDistance 이내이고, 아직 구매하지 않은 경우
//            if (distance <= showDistance && !hasPurchased)
//            {
//                ShowSlider(SecondMarketCanvas); // 상점 입장 버튼 보이기

//                // 'E' 키를 눌렀을 때 상점 UI 활성화 및 깜빡이는 효과 시작
//                if (Input.GetKeyDown(KeyCode.E))
//                {
//                    if (!StoreUI.activeInHierarchy)
//                    {
//                        StoreUI.SetActive(true);
//                        supply.SetActive(false);
//                        StartCoroutine(FlashIcons()); // 깜빡이는 효과 시작
//                    }
//                    else
//                    {
//                        Buythis(); // 구매 처리
//                        hasPurchased = true; // 구매 완료 상태로 전환
//                    }
//                }
//            }
//            else
//            {
//                // 거리가 멀어지면 모든 UI 숨기기
//                HideSlider(SecondMarketCanvas);
//                StoreUI.SetActive(false);
//                BUYCON.gameObject.SetActive(false);
//            }
//        }
//    }

//    // UI를 보이도록 설정하는 함수
//    private void ShowSlider(CanvasGroup canvasGroup)
//    {
//        canvasGroup.alpha = 1;
//        canvasGroup.interactable = true;
//        canvasGroup.blocksRaycasts = true;
//    }

//    // UI를 숨기는 함수
//    private void HideSlider(CanvasGroup canvasGroup)
//    {
//        canvasGroup.alpha = 0;
//        canvasGroup.interactable = false;
//        canvasGroup.blocksRaycasts = false;
//    }

//    // 구매 처리 함수
//    public void Buythis()
//    {
//        if (ScoreManager.Instance.BuyAK(3000)) // 점수 차감 후 아이템 구매
//        {
//            BUYCON.SetActive(true); // 구매 확인 UI 활성화
//        }
//    }

//    // 깜빡이는 효과를 구현하는 코루틴 함수
//    private IEnumerator FlashIcons()
//    {
//        float duration = 0.5f; // 색상 전환 시간
//        float t = 0; // 시간 추적 변수

//        Color startColor = Color.white; // 시작 색상
//        Color endColor = Color.yellow; // 끝 색상

//        while (true)
//        {
//            // 시간에 따라 색상을 선형 보간
//            t += Time.deltaTime / duration;
//            rawImage.color = Color.Lerp(startColor, endColor, t);

//            // 보간이 끝나면 시작 색상과 끝 색상을 교체하여 역순으로 변경
//            if (t >= 1.0f)
//            {
//                Color temp = startColor;
//                startColor = endColor;
//                endColor = temp;
//                t = 0; // 시간 변수 초기화
//            }

//            yield return null; // 다음 프레임까지 기다림
//        }
//    }
//}


// 2번째 자판기 해보자...  구매 할거 깜빡거리는거 추가
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SecondMarket : MonoBehaviour
{
    public GameObject ButtonUI;
    //public GameObject DOOR;
    public Button Enter; // 보급품 들어가기 버튼
    public GameObject StoreUI;
    public GameObject supply;
    public GameObject BUYCON;

    public float showDistance = 4.0f; // 버튼이 보이는 거리
    private Transform playerposition; // 플레이어 위치 추적
    private CanvasGroup SecondMarketCanvas; // 2번째 자판기의 CanvasGroup


    //public ScoreManager ScoreManager;
    //public UIManager UIManager;

    private bool hasPurchased = false; // 구매 여부 확인

    [SerializeField] private RawImage rawImage;

    void Start()
    {
        StoreUI.gameObject.SetActive(false);
        BUYCON.gameObject.SetActive(false);
        // 각각의 버튼에서 CanvasGroup 컴포넌트 찾기
        SecondMarketCanvas = Enter.GetComponent<CanvasGroup>();


        GameObject player = GameObject.FindGameObjectWithTag("Player"); // 플레이어를 찾아서 플레이어 트랜스폼에 저장
        if (player != null)
        {
            playerposition = player.transform;
        }

        //rawImage = gameObject. GetComponent<RawImage>();
    }

    void Update()
    {
        if (playerposition != null)
        {
            // 플레이어와 오브젝트 사이 거리를 계산
            float distance = Vector3.Distance(playerposition.position, transform.position);

            if (distance <= showDistance)
            {
                if (!hasPurchased)
                {
                    ShowSlider(SecondMarketCanvas);
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        if (!StoreUI.activeInHierarchy)
                        {
                            StoreUI.SetActive(true);
                            supply.SetActive(false);
                            StartCoroutine(FlashIcons());

                        }
                        else
                        {
                            Buythis();
                            hasPurchased = true;
                        }
                    }
                }
            }
            else
            {
                // 거리가 멀어지면 모든 슬라이더 숨기기
                HideSlider(SecondMarketCanvas);
                StoreUI.SetActive(false);
                BUYCON.gameObject.SetActive(false);
            }
        }
    }

    private void ShowSlider(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private void HideSlider(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
    public void Buythis()
    {
        if (ScoreManager.Instance.BuyAK(3000))
        {
            BUYCON.SetActive(true);
            //Debug.Log("구매 완료");
            //Debug.Log("적 사망 전 점수: " + ScoreManager.Instance.GetScore());
            //Debug.Log("적 사망 후 점수: " + ScoreManager.Instance.GetScore());
        }
    }
    private IEnumerator FlashIcons()
    {
        while (true)
        {
            rawImage.color = Color.white;
            yield return new WaitForSeconds(0.5f);
            rawImage.color = Color.yellow;
            yield return new WaitForSeconds(0.5f);
        }
    }

}

//// 2번째 자판기 해보자...(처음)
//using System.Collections;
//using UnityEngine;
//using UnityEngine.UI;

//public class SecondMarket : MonoBehaviour
//{
//    public GameObject ButtonUI;
//    public GameObject DOOR;
//    public Button Enter; // 보급품 들어가기 버튼
//    public GameObject StoreUI;
//    public GameObject supply;
//    public GameObject BUYCON;

//    public float showDistance = 3.0f; // 버튼이 보이는 거리
//    private Transform playerposition; // 플레이어 위치 추적
//    private CanvasGroup SecondMarketCanvas; // 2번째 자판기의 CanvasGroup


//    //public ScoreManager ScoreManager;
//    //public UIManager UIManager;

//    private bool hasPurchased = false; // 구매 여부 확인


//    void Start()
//    {
//        StoreUI.gameObject.SetActive(false);
//        BUYCON.gameObject.SetActive(false);
//        // 각각의 버튼에서 CanvasGroup 컴포넌트 찾기
//        SecondMarketCanvas = Enter.GetComponent<CanvasGroup>();


//        GameObject player = GameObject.FindGameObjectWithTag("Player"); // 플레이어를 찾아서 플레이어 트랜스폼에 저장
//        if (player != null)
//        {
//            playerposition = player.transform;
//        }
//    }

//    void Update()
//    {
//        if (playerposition != null)
//        {
//            // 플레이어와 오브젝트 사이 거리를 계산
//            float distance = Vector3.Distance(playerposition.position, transform.position);

//            if (distance <= showDistance)
//            {
//                if (!hasPurchased)
//                {
//                    ShowSlider(SecondMarketCanvas);
//                    if (Input.GetKeyDown(KeyCode.E))
//                    {
//                        if (!StoreUI.activeInHierarchy)
//                        {
//                            StoreUI.SetActive(true);
//                            supply.SetActive(false);

//                        }
//                        else
//                        {
//                            Buythis();
//                            hasPurchased = true;
//                        }
//                    }
//                }
//            }
//            else
//            {
//                // 거리가 멀어지면 모든 슬라이더 숨기기
//                HideSlider(SecondMarketCanvas);
//                StoreUI.SetActive(false);
//                BUYCON.gameObject.SetActive(false);
//            }
//        }
//    }

//    private void ShowSlider(CanvasGroup canvasGroup)
//    {
//        canvasGroup.alpha = 1;
//        canvasGroup.interactable = true;
//        canvasGroup.blocksRaycasts = true;
//    }

//    private void HideSlider(CanvasGroup canvasGroup)
//    {
//        canvasGroup.alpha = 0;
//        canvasGroup.interactable = false;
//        canvasGroup.blocksRaycasts = false;
//    }
//    public void Buythis()
//    {
//        if (ScoreManager.Instance.BuyAK(3000))
//        {
//            BUYCON.SetActive(true);
//            //Debug.Log("구매 완료");
//            //Debug.Log("적 사망 전 점수: " + ScoreManager.Instance.GetScore());
//            //Debug.Log("적 사망 후 점수: " + ScoreManager.Instance.GetScore());
//        }
//    }
//}
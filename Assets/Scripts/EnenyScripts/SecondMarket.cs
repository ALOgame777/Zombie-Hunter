// 2번째 자판기 해보자...
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SecondMarket : MonoBehaviour
{
    public GameObject ButtonUI;
    public GameObject DOOR;
    public Button Enter; // 보급품 들어가기 버튼
    public GameObject StoreUI;
    public GameObject supply;
    public GameObject BUYCON;

    public float showDistance = 3.0f; // 버튼이 보이는 거리
    private Transform playerposition; // 플레이어 위치 추적
    private CanvasGroup SecondMarketCanvas; // 2번째 자판기의 CanvasGroup


    //public ScoreManager ScoreManager;
    //public UIManager UIManager;

    private bool hasPurchased = false; // 구매 여부 확인

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

}
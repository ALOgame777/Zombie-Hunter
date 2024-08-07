// 2번째 자판기 해보자...
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SecondMarket : MonoBehaviour
{
    public GameObject ButtonUI;
    public GameObject DOOR;
    public Button Enter; // 보급품 들어가기 버튼
    public Button Imiboyouzong; // 이미 보유중 표시
    public float showDistance = 3.0f; // 버튼이 보이는 거리
    private Transform playerposition; // 플레이어 위치 추적
    private CanvasGroup buyDoubleShotCanvasGroup; // BuyDoubleShot의 CanvasGroup
    private CanvasGroup imiboyouzongCanvasGroup; // Imiboyouzong의 CanvasGroup

    public ScoreManager ScoreManager;
    public UIManager UIManager;

    private bool hasPurchased = false; // 구매 여부 확인

    void Start()
    {
        // 각각의 버튼에서 CanvasGroup 컴포넌트 찾기
        buyDoubleShotCanvasGroup = Enter.GetComponent<CanvasGroup>();
        imiboyouzongCanvasGroup = Imiboyouzong.GetComponent<CanvasGroup>();

        GameObject player = GameObject.FindGameObjectWithTag("Player"); // 플레이어를 찾아서 플레이어 트랜스폼에 저장
        if (player != null)
        {
            playerposition = player.transform;
        }

        // 초기에는 Imiboyouzong 버튼을 숨김
        Imiboyouzong.gameObject.SetActive(false);
    }

    void Update()
    {
        if (playerposition != null)
        {
            // 플레이어와 오브젝트 사이 거리를 계산
            float distance = Vector3.Distance(playerposition.position, transform.position);

            if (distance <= showDistance)
            {
                if (hasPurchased)
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        // 이미 구매한 경우 Imiboyouzong 버튼 보여주기
                        ShowSlider(imiboyouzongCanvasGroup);
                    }
                }
                else
                {
                    // 구매하지 않은 경우 BuyDoubleShot 버튼 보여주기
                    ShowSlider(buyDoubleShotCanvasGroup);

                    // 'E' 키를 누르면 구매 함
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        DoubleShoot();
                       
                        Imiboyouzong.gameObject.SetActive(true);
                        hasPurchased = true;
                    }
                }
            }
            else
            {
                // 거리가 멀어지면 모든 슬라이더 숨기기
                HideSlider(buyDoubleShotCanvasGroup);
                HideSlider(imiboyouzongCanvasGroup);
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

    public void DoubleShoot()
    {
        if (ScoreManager.Instance.BuyDoubleShoot(2000))
        {
            Debug.Log("적 사망 전 점수: " + ScoreManager.Instance.GetScore());
            Debug.Log("적 사망 후 점수: " + ScoreManager.Instance.GetScore());
        }
    }
}
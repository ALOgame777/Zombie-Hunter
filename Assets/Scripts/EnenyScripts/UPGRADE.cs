// 3번째 자판기 해보자...
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UPGRADE : MonoBehaviour
{
    public GameObject third; // 3번째 애 넣으면 됨.
    public Weapon[] playerWeapons; // 플레이어가 가지고 있는 모든 무기들
    


    public float showDistance = 3.0f; // 버튼이 보이는 거리
    private Transform playerposition; // 플레이어 위치 추적
    private CanvasGroup UPGRADEcanvas; // 3번째 자판기의 CanvasGroup

    //public ScoreManager ScoreManager;
    //public UIManager UIManager;
    private bool hasPurchased = false; // 구매 여부 확인


    void Start()
    {
        third.gameObject.SetActive(false);
        // 각각의 버튼에서 CanvasGroup 컴포넌트 찾기
        UPGRADEcanvas = third.GetComponent<CanvasGroup>();


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
                    third.SetActive(true);
                    ShowSlider(UPGRADEcanvas);
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        UpgrdeWeapon();
                        HideSlider(UPGRADEcanvas);
                        hasPurchased = true;
                    }
                }
               
            }
            else
            {
                HideSlider(UPGRADEcanvas);
               
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
    public void UpgrdeWeapon()
    {
        if (ScoreManager.Instance.UpgradeWeapon(2000))
        {
            Debug.Log("구매 완료");

            // 모든 무기의 공격력을 50% 증가시킴
            foreach (Weapon weapon in playerWeapons)
            {
                weapon.IncreaseAttackPower(50); // 공격력을 50% 증가
            }
            //Debug.Log("적 사망 전 점수: " + ScoreManager.Instance.GetScore());
            //Debug.Log("적 사망 후 점수: " + ScoreManager.Instance.GetScore());
        }
    }

}
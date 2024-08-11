// 보스 근처에 가면 보스 체력이 보이도록 해보자.
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BOSSHPUIVIEW : MonoBehaviour
{
    //public GameObject ButtonUI;
    //public GameObject DOOR;

    public Slider HPsliderBOSS; // 보스 체력바 
    public float showDistance = 200.0f; // 체력바 보이는 거리
    private Transform playerposition; // 플레이어 위치 추적
    private CanvasGroup HPsliderBOSScv; // 체력바의 CanvasGroup

    //public ScoreManager ScoreManager;
    //public UIManager UIManager;


    void Start()
    {
        // 각각의 버튼에서 CanvasGroup 컴포넌트 찾기
        HPsliderBOSScv = HPsliderBOSS.GetComponent<CanvasGroup>();

        GameObject player = GameObject.FindGameObjectWithTag("Player"); // 플레이어를 찾아서 플레이어 트랜스폼에 저장
        if (player != null)
        {
            playerposition = player.transform;
        }
        Debug.Log(HPsliderBOSScv != null ? "CanvasGroup initialized" : "CanvasGroup not found!");


    }

    void Update()
    {
        if (playerposition != null)
        {
            // 플레이어와 오브젝트 사이 거리를 계산
            float distance = Vector3.Distance(playerposition.position, transform.position);

            if (distance <= showDistance)
            {
                ShowSlider(HPsliderBOSScv);


            }
            else
            {
                // 거리가 멀어지면 모든 슬라이더 숨기기
                HideSlider(HPsliderBOSScv);
                
            }
        }
        Debug.Log("Boss Position: " + transform.position + " | Player Position: " + playerposition.position);

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

  
}


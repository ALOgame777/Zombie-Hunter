// 2��° ���Ǳ� �غ���...
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SecondMarket : MonoBehaviour
{
    public GameObject ButtonUI;
    public GameObject DOOR;
    public Button Enter; // ����ǰ ���� ��ư
    public GameObject StoreUI;
    public GameObject supply;
    public GameObject BUYCON;

    public float showDistance = 3.0f; // ��ư�� ���̴� �Ÿ�
    private Transform playerposition; // �÷��̾� ��ġ ����
    private CanvasGroup SecondMarketCanvas; // 2��° ���Ǳ��� CanvasGroup


    //public ScoreManager ScoreManager;
    //public UIManager UIManager;

    private bool hasPurchased = false; // ���� ���� Ȯ��

    void Start()
    {
        StoreUI.gameObject.SetActive(false);
        BUYCON.gameObject.SetActive(false);
        // ������ ��ư���� CanvasGroup ������Ʈ ã��
        SecondMarketCanvas = Enter.GetComponent<CanvasGroup>();
       

        GameObject player = GameObject.FindGameObjectWithTag("Player"); // �÷��̾ ã�Ƽ� �÷��̾� Ʈ�������� ����
        if (player != null)
        {
            playerposition = player.transform;
        }
    }

    void Update()
    {
        if (playerposition != null)
        {
            // �÷��̾�� ������Ʈ ���� �Ÿ��� ���
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
                // �Ÿ��� �־����� ��� �����̴� �����
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
            //Debug.Log("���� �Ϸ�");
            //Debug.Log("�� ��� �� ����: " + ScoreManager.Instance.GetScore());
            //Debug.Log("�� ��� �� ����: " + ScoreManager.Instance.GetScore());
        }
    }

}
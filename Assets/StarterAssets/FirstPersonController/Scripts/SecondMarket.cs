// 2��° ���Ǳ� �غ���...
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SecondMarket : MonoBehaviour
{
    public GameObject ButtonUI;
    public GameObject DOOR;
    public Button Enter; // ����ǰ ���� ��ư
    public Button Imiboyouzong; // �̹� ������ ǥ��
    public float showDistance = 3.0f; // ��ư�� ���̴� �Ÿ�
    private Transform playerposition; // �÷��̾� ��ġ ����
    private CanvasGroup buyDoubleShotCanvasGroup; // BuyDoubleShot�� CanvasGroup
    private CanvasGroup imiboyouzongCanvasGroup; // Imiboyouzong�� CanvasGroup

    public ScoreManager ScoreManager;
    public UIManager UIManager;

    private bool hasPurchased = false; // ���� ���� Ȯ��

    void Start()
    {
        // ������ ��ư���� CanvasGroup ������Ʈ ã��
        buyDoubleShotCanvasGroup = Enter.GetComponent<CanvasGroup>();
        imiboyouzongCanvasGroup = Imiboyouzong.GetComponent<CanvasGroup>();

        GameObject player = GameObject.FindGameObjectWithTag("Player"); // �÷��̾ ã�Ƽ� �÷��̾� Ʈ�������� ����
        if (player != null)
        {
            playerposition = player.transform;
        }

        // �ʱ⿡�� Imiboyouzong ��ư�� ����
        Imiboyouzong.gameObject.SetActive(false);
    }

    void Update()
    {
        if (playerposition != null)
        {
            // �÷��̾�� ������Ʈ ���� �Ÿ��� ���
            float distance = Vector3.Distance(playerposition.position, transform.position);

            if (distance <= showDistance)
            {
                if (hasPurchased)
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        // �̹� ������ ��� Imiboyouzong ��ư �����ֱ�
                        ShowSlider(imiboyouzongCanvasGroup);
                    }
                }
                else
                {
                    // �������� ���� ��� BuyDoubleShot ��ư �����ֱ�
                    ShowSlider(buyDoubleShotCanvasGroup);

                    // 'E' Ű�� ������ ���� ��
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
                // �Ÿ��� �־����� ��� �����̴� �����
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
            Debug.Log("�� ��� �� ����: " + ScoreManager.Instance.GetScore());
            Debug.Log("�� ��� �� ����: " + ScoreManager.Instance.GetScore());
        }
    }
}
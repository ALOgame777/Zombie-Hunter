// �����ϰ� ���� �ٰ����� �ٸ��� �߰� �ϴ� ��,.,.+++++���~~~~
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FirstMarket : MonoBehaviour
{
    public GameObject ButtonUI;
    public GameObject DOOR;
    public Button BuyDoubleShot; // ���� �� ���� ��ư
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
        buyDoubleShotCanvasGroup = BuyDoubleShot.GetComponent<CanvasGroup>();
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
                        BuyDoubleShot.gameObject.SetActive(false);
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

//// ���������� ���� ���� 'e' ������ ���Ű� �����ѵ�, ������ �ѹ� ��� �ƿ� ����.
//using System.Collections;
//using System.Security.Cryptography;
//using UnityEngine;
//using UnityEngine.UI;

//public class FirstMarket : MonoBehaviour
//{
//    public GameObject ButtonUI;
//    public GameObject DOOR;
//    public Button BuyDoubleShot; // ���� �� ���� ��ư
//    public Button Imiboyouzong; // �̹� ������ ǥ��
//    public float showDistance = 3.0f; // ��ư�� ���̴� �Ÿ�
//    private Transform playerposition; // �÷��̾� ��ġ ����
//    private CanvasGroup Doorcanvasgroup; // ������ ��ȣ�ۿ� �����ϴ� ĵ����

//    public ScoreManager ScoreManager;
//    public UIManager UIManager;



//    void Start()
//    {
//        Doorcanvasgroup = BuyDoubleShot.GetComponent<CanvasGroup>(); // �� ���⿡�� ĵ�����׷� ã��
//        Doorcanvasgroup = Imiboyouzong.GetComponent<CanvasGroup>(); // �� ���⿡�� ĵ�����׷� ã��

//        GameObject player = GameObject.FindGameObjectWithTag("Player"); // �÷��̾ ã�Ƽ� �÷��̾�Ʈ�������� ����
//        if (player != null)
//        {
//            playerposition = player.transform;
//        }
//    }

//    void Update()
//    {
//        if (playerposition != null)
//        {
//            // �÷��̾�� ������Ʈ ���� �Ÿ��� ���
//            float distance = Vector3.Distance(playerposition.position, transform.position);

//            // �Ÿ��� ��Ÿ����� ������
//            if (distance <= showDistance)
//            {
//                // �����ֱ�
//                ShowSlider();

//                // 'E' Ű�� ������ ���� ������ ��
//                if (Input.GetKeyDown(KeyCode.E))
//                {
//                    DoubleShoot();
//                    HideSlider();
//                    BuyDoubleShot.gameObject.SetActive(false);
//                    Imiboyouzong.gameObject.SetActive(true);
//                }
//            }
//            else
//            {
//                // �����
//                HideSlider();
//            }
//        }
//    }

//    private void ShowSlider() // �����̴� �����ֱ�
//    {
//        Doorcanvasgroup.alpha = 1;
//        Doorcanvasgroup.interactable = true;
//        Doorcanvasgroup.blocksRaycasts = true;
//    }

//    private void HideSlider() // �����̴� �����
//    {
//        Doorcanvasgroup.alpha = 0;
//        Doorcanvasgroup.interactable = false;
//        Doorcanvasgroup.blocksRaycasts = false;
//    }

//    public void DoubleShoot()
//    {
//        if (ScoreManager.Instance.BuyDoubleShoot(2000))
//        {
//            Debug.Log("�� ��� �� ����: " + ScoreManager.Instance.GetScore());
//            Debug.Log("�� ��� �� ����: " + ScoreManager.Instance.GetScore());

//        }
//    }
//}

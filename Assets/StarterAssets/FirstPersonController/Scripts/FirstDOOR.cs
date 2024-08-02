// ���� ���� ������ ����? ����~~~~ + �� 5�� �ڿ� ������� ��.
using System.Collections;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class FirstDOOR : MonoBehaviour
{
    public GameObject ButtonUI;
    public GameObject DOOR;
    public Button opendoor; // �� ���� ��ư
    public float showDistance = 7.0f; // ��ư�� ���̴� �Ÿ�
    private Transform playerposition; // �÷��̾� ��ġ ����
    private CanvasGroup Doorcanvasgroup; // ������ ��ȣ�ۿ� �����ϴ� ĵ����

    public float DoorSpeed = 2; // ���� �ö󰡴� �ӵ�

    private int playerMoney = 1000; // �÷��̾��� �ʱ� ��, ���÷� 1000�� ����
    private bool isDoorOpening = false; // ���� ������ �ִ� ���¸� ����

    void Start()
    {
        Doorcanvasgroup = opendoor.GetComponent<CanvasGroup>(); // �� ���⿡�� ĵ�����׷� ã��

        GameObject player = GameObject.FindGameObjectWithTag("Player"); // �÷��̾ ã�Ƽ� �÷��̾�Ʈ�������� ����
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

            // �Ÿ��� ��Ÿ����� ������
            if (distance <= showDistance && !isDoorOpening)
            {
                // �����ֱ�
                ShowSlider();

                // 'E' Ű�� ������ ���� ������ ��
                if (Input.GetKeyDown(KeyCode.E))
                {
                    TryOpenDoor();
                    HideSlider();
                }
            }
            else
            {
                // �����
                HideSlider();
            }
        }

        // ���� ������ ���¶��, ���� ���� �̵���Ŵ
        if (isDoorOpening)
        {
            //OpenDoor();
            StartCoroutine(OpenDoor());
        }
    }

    private void ShowSlider() // �����̴� �����ֱ�
    {
        Doorcanvasgroup.alpha = 1;
        Doorcanvasgroup.interactable = true;
        Doorcanvasgroup.blocksRaycasts = true;
    }

    private void HideSlider() // �����̴� �����
    {
        Doorcanvasgroup.alpha = 0;
        Doorcanvasgroup.interactable = false;
        Doorcanvasgroup.blocksRaycasts = false;
    }

    private void TryOpenDoor()
    {
        // �÷��̾��� ���� 700�� �̻����� Ȯ��
        if (playerMoney >= 700)
        {
            // 700�� ����
            playerMoney -= 700;

            // ���� ������ ���·� ����
            isDoorOpening = true;
        }
        else
        {
            Debug.Log("���� �����մϴ�. ���� �� �� �����ϴ�.");
        }
    }

    IEnumerator OpenDoor()
    {
        // ���� ���� �̵���Ŵ
        transform.position += Vector3.up * DoorSpeed * Time.deltaTime;

        yield return new WaitForSeconds(5f);
        print("�Ҹ�");
        Destroy(gameObject);
    }
}

//// �ϴ� ������ �� ���� + e Ű�� �� ����
//using System.Collections;
//using UnityEngine;
//using UnityEngine.UI;

//public class FirstDOOR : MonoBehaviour
//{
//    public GameObject ButtonUI;
//    public GameObject DOOR;
//    public Button opendoor; // �� ���� ��ư
//    public float showDistance = 7.0f; // ��ư�� ���̴� �Ÿ�
//    private Transform playerposition; // �÷��̾� ��ġ ����
//    private CanvasGroup Doorcanvasgroup; // ������ ��ȣ�ۿ� �����ϴ� ĵ����

//    float DoorSpeed = 2;

//    private int playerMoney = 1000; // �÷��̾��� �ʱ� ��, ���÷� 1000�� ����

//    // Start is called before the first frame update
//    void Start()
//    {
//        Doorcanvasgroup = opendoor.GetComponent<CanvasGroup>(); // �� ���⿡�� ĵ�����׷� ã��

//        GameObject player = GameObject.FindGameObjectWithTag("Player"); // �÷��̾ ã�Ƽ� �÷��̾�Ʈ�������� ����
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
//                    TryOpenDoor();
//                    HideSlider();
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

//    private void TryOpenDoor()
//    {
//        // �÷��̾��� ���� 700�� �̻����� Ȯ��
//        if (playerMoney >= 700)
//        {
//            // 700�� ����
//            playerMoney -= 700;

//            // �� ����
//            DestroyDoor();
//        }
//        else
//        {
//            Debug.Log("���� �����մϴ�. ���� �� �� �����ϴ�.");
//        }
//    }

//    public void DestroyDoor()
//    {
//        //�� ������Ʈ ����
//         Destroy(gameObject);


//    }
//}


//// ���� ����ϴ� ���� ��ġ��(trigger) �� ��ư �ȶߴ°� �����ߴµ� (�Ÿ� ����) �� ��ư�� ���� ���� ����~~~~~
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class FirstDOOR : MonoBehaviour
//{
//    public GameObject ButtonUI;
//    public GameObject DOOR;
//    public Button opendoor; // �� ���� ��ư
//    public float showDistance = 7.0f; // ��ư�� ���̴� �Ÿ�
//    private Transform playerposition; // �÷��̾� ��ġ ����
//    private CanvasGroup Doorcanvasgroup; // ������ ��ȣ�ۿ� �����ϴ� ĵ����


//    // Start is called before the first frame update
//    void Start()
//    {
//        Doorcanvasgroup = opendoor.GetComponent<CanvasGroup>(); // �� ���⿡�� ĵ�����׷� ã��

//        GameObject player = GameObject.FindGameObjectWithTag("Player"); // �÷��̾ ã�Ƽ� �÷��̾�Ʈ�������� ����
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

//            // �÷��̾�� ������Ʈ ���� �Ÿ��� ���
//            float distance = Vector3.Distance(playerposition.position, transform.position);

//            // �Ÿ��� ��Ÿ� ���� ������
//            if (distance <= showDistance)
//            {
//                // �����ֱ�
//                ShowSlider();
//            }
//            else
//            {
//                //�����
//                HideSlider();
//            }
//        }
//    }
//    private void ShowSlider() // �����̴� �����ְ� â�� ��ġ��
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
//    private void OnCollisionStay(Collision asdf)
//    {
//        if (asdf.gameObject.CompareTag("Player"))
//        {
//            IncreaseSliderOverTime();
//        }
//    }


//    private void IncreaseSliderOverTime()
//    {
//        // ��ư ui ����
//        ButtonUI.SetActive(true);
//    }

//    public void DestroyDoor()
//    {
//        Destroy(gameObject);
//    }

//}

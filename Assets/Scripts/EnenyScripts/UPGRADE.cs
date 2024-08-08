// 3��° ���Ǳ� �غ���...
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UPGRADE : MonoBehaviour
{
    public GameObject third; // 3��° �� ������ ��.
    public Weapon[] playerWeapons; // �÷��̾ ������ �ִ� ��� �����
    


    public float showDistance = 3.0f; // ��ư�� ���̴� �Ÿ�
    private Transform playerposition; // �÷��̾� ��ġ ����
    private CanvasGroup UPGRADEcanvas; // 3��° ���Ǳ��� CanvasGroup

    //public ScoreManager ScoreManager;
    //public UIManager UIManager;
    private bool hasPurchased = false; // ���� ���� Ȯ��


    void Start()
    {
        third.gameObject.SetActive(false);
        // ������ ��ư���� CanvasGroup ������Ʈ ã��
        UPGRADEcanvas = third.GetComponent<CanvasGroup>();


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
            Debug.Log("���� �Ϸ�");

            // ��� ������ ���ݷ��� 50% ������Ŵ
            foreach (Weapon weapon in playerWeapons)
            {
                weapon.IncreaseAttackPower(50); // ���ݷ��� 50% ����
            }
            //Debug.Log("�� ��� �� ����: " + ScoreManager.Instance.GetScore());
            //Debug.Log("�� ��� �� ����: " + ScoreManager.Instance.GetScore());
        }
    }

}
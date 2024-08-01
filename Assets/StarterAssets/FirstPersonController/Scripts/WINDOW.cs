// �� �ѹ� �ٽ� �غ��� ---------- ������ ���� �����̴� ���� + 1ĭ�� ȸ�� UI ���� �Ϸ�~~~
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class WINDOW : MonoBehaviour
{
    public Slider progressSlider; // UI �����̴�
    public float incrementAmount = 1.0f; // �����̴� ������
     float incrementDuration = 1f; // ���� �ð�
    public float showDistance = 2.0f; // �����̴��� ���̴� �Ÿ�

    private CanvasGroup sliderCanvasGroup; // ������ ��ȣ�ۿ� �����ϴ� ĵ����
    private Transform playerTransform; // �÷��̾� ��ġ ����

    void Start()
    {
        sliderCanvasGroup = progressSlider.GetComponent<CanvasGroup>(); // �����̴����� �����̴�ĵ�����׷� ã��
        if (sliderCanvasGroup == null)
        {
            Debug.LogError("CanvasGroup component not found on the Slider.");
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player"); // �÷��̾ ã�Ƽ� �÷��̾�Ʈ�������� ����
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player object not found. Make sure the player has the 'Player' tag.");
        }
    }

    void Update()
    {

        if (playerTransform != null) 
        {
            
        // �÷��̾�� ������Ʈ ���� �Ÿ��� ���
            float distance = Vector3.Distance(playerTransform.position, transform.position);
            
            // �Ÿ��� ��Ÿ� ���� ������
            if (distance <= showDistance)
            { 
                // �����ֱ�
                ShowSlider();
            }
            else
            { 
                //�����
                HideSlider();
            }
        }
    }

    private void ShowSlider() // �����̴� �����ְ� â�� ��ġ��
    {
        sliderCanvasGroup.alpha = 1;
        sliderCanvasGroup.interactable = true;
        sliderCanvasGroup.blocksRaycasts = true;
    }

    private void HideSlider() // �����̴� �����
    {
        sliderCanvasGroup.alpha = 0;
        sliderCanvasGroup.interactable = false;
        sliderCanvasGroup.blocksRaycasts = false;
    }

    private void OnTriggerStay(Collider other) // �÷��̾ Ʈ���ſ� ������ ����ؼ� �ڷ�ƾ ����
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(IncreaseSliderOverTime());
        }
    }

    IEnumerator IncreaseSliderOverTime() // �����̴� ���� ����
    {
        // ���� �����̴� ���� targetvalue�� incrementamount�� ���� ������ ���� 
        float targetValue = progressSlider.value + incrementAmount;
        float elapsedTime = 0;

        // elapsedtime�� ���� ��Ű�鼭 �����̴� ���� ������ ������ targetvalue�� ����
        while (elapsedTime < incrementDuration)
        {
            progressSlider.value = Mathf.Lerp(progressSlider.value, targetValue, elapsedTime / incrementDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        progressSlider.value = targetValue;
    }
}


////�����̴� �����̰��� �ö󰣴� ����!
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class WINDOW : MonoBehaviour
//{
//    public Slider progressSlider; // UI �����̴�
//    public float incrementAmount = 1f; // �����̴� ������
//    public float incrementDuration = 1.0f; // ���� �ð�

//    private void OnTriggerStay(Collider other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            StartCoroutine(IncreaseSliderOverTime());
//        }
//    }

//    IEnumerator IncreaseSliderOverTime()
//    {
//        float targetValue = progressSlider.value + incrementAmount;
//        float elapsedTime = 0;

//        while (elapsedTime < incrementDuration)
//        {
//            progressSlider.value = Mathf.Lerp(progressSlider.value, targetValue, elapsedTime / incrementDuration);
//            elapsedTime += Time.deltaTime;
//            yield return null;
//        }

//        progressSlider.value = targetValue;
//    }

//}



// �ƹ��͵� ������� �ȵſ�
//public float speed = 6.0f; // �̵� �ӵ�
//public Slider progressSlider; // UI �����̴�
//public float incrementAmount = 0.1f; // �����̴� ������
//public float stopDuration = 1.0f; // ���ߴ� �ð�
//private bool isStopped = false; // �÷��̾ �����ִ��� ����
//private CharacterController characterController;

//void Start()
//{
//    characterController = GetComponent<CharacterController>();
//}

//void Update()
//{
//    if (!isStopped)
//    {
//        // �÷��̾��� �̵� ����
//        float moveHorizontal = Input.GetAxis("Horizontal");
//        float moveVertical = Input.GetAxis("Vertical");

//        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
//        characterController.Move(movement * speed * Time.deltaTime);
//    }
//}

//void OnTriggerEnter(Collider other)
//{
//    if (other.CompareTag("Box"))
//    {
//        StartCoroutine(IncreaseSliderOverTime());
//    }
//}

//IEnumerator IncreaseSliderOverTime()
//{
//    isStopped = true;
//    float targetValue = progressSlider.value + incrementAmount;
//    float elapsedTime = 0;

//    while (elapsedTime < stopDuration)
//    {
//        progressSlider.value = Mathf.Lerp(progressSlider.value, targetValue, (elapsedTime / stopDuration));
//        elapsedTime += Time.deltaTime;
//        yield return null;
//    }

//    progressSlider.value = targetValue;
//    isStopped = false;
//}
//private void OnTriggerStay(Collider other)
//{

//    if (other.gameObject.CompareTag("Player"))
//    {
//        REPAIR();  // �÷��̾�� �浹 �� ���� �޼��� ȣ��
//    }
//}
//void REPAIR()
//{





//}

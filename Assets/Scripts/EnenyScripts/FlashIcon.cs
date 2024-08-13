using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FlashIcon : MonoBehaviour
{
    public RawImage buttonImage; // `BuyThis` 오브젝트의 `Image` 컴포넌트

    void Start()
    {
        // `BuyThis` 오브젝트의 `Image` 컴포넌트를 가져옵니다.
        buttonImage = GetComponent<RawImage>();

        if (buttonImage == null)
        {
            Debug.LogError("Image component is missing on BuyThis!");
        }
    }

    // UI가 활성화될 때 호출되는 메서드
    void OnEnable()
    {
        if (buttonImage != null)
        {
            StartCoroutine(FlashIcons());
        }
    }

    // UI가 비활성화될 때 코루틴 중지
    void OnDisable()
    {
        StopCoroutine(FlashIcons());
        buttonImage.color = Color.white; // 비활성화될 때 기본 색상으로 초기화
    }

    private IEnumerator FlashIcons()
    {
        while (true)
        {
            // 기본 색상(흰색)으로 변경
            buttonImage.color = Color.white;
            yield return new WaitForSeconds(1.0f);

            // 노란색으로 변경
            buttonImage.color = Color.yellow;
            yield return new WaitForSeconds(1.0f);
        }
    }
}

// 갑자기 안ㄴ됨

//using System.Collections;
//using UnityEngine;
//using UnityEngine.UI;

//public class FlashIcon : MonoBehaviour
//{
//    public Image rawImage;

//    void Start()
//    {
//        rawImage = GetComponent<Image>();

//        if (rawImage == null)
//        {
//            Debug.LogError("RawImage component is missing!");
//        }
//    }

//    // UI가 활성화될 때 호출되는 메서드
//    void OnEnable()
//    {
//        if (rawImage != null)
//        {
//            StartCoroutine(FlashIcons());
//        }
//    }

//    // UI가 비활성화될 때 코루틴 중지
//    void OnDisable()
//    {
//        StopCoroutine(FlashIcons());
//        rawImage.color = Color.white; // 비활성화될 때 기본 색상으로 초기화
//    }

//    private IEnumerator FlashIcons()
//    {
//        while (true)
//        {
//            rawImage.color = Color.white;
//            yield return new WaitForSeconds(1.0f);
//            rawImage.color = Color.yellow;
//            yield return new WaitForSeconds(1.0f);
//        }
//    }
//}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FlashIcon : MonoBehaviour
{
    private RawImage rawImage;

    void Start()
    {
        rawImage = GetComponent<RawImage>();

        if (rawImage == null)
        {
            Debug.LogError("RawImage component is missing!");
        }
    }

    // UI가 활성화될 때 호출되는 메서드
    void OnEnable()
    {
        if (rawImage != null)
        {
            StartCoroutine(FlashIcons());
        }
    }

    // UI가 비활성화될 때 코루틴 중지
    void OnDisable()
    {
        StopCoroutine(FlashIcons());
        rawImage.color = Color.white; // 비활성화될 때 기본 색상으로 초기화
    }

    private IEnumerator FlashIcons()
    {
        while (true)
        {
            rawImage.color = Color.white;
            yield return new WaitForSeconds(1.0f);
            rawImage.color = Color.yellow;
            yield return new WaitForSeconds(1.0f);
        }
    }
}

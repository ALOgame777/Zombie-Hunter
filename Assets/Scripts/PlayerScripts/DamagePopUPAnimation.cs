using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopUPAnimation : MonoBehaviour
{
    public AnimationCurve opacityCurve;
    public AnimationCurve scaleCurve;
    public AnimationCurve heightcurve;
    public AnimationCurve fontSizeCurve; // 새로운 AnimationCurve 추가
    public float baseFontSize = 23f; // 기본 폰트 크기 설정

    private TextMeshProUGUI tmp;
    private float time = 0;
    private Vector3 origin;

    private void Awake()
    {
        tmp = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        origin = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        tmp.color = new Color(1, 1, 1, opacityCurve.Evaluate(time));
        transform.localScale= Vector3.one * scaleCurve.Evaluate(time);
        transform.position = origin + new Vector3(0, 1 + heightcurve.Evaluate(time), 0);
        // 폰트 크기 업데이트
        tmp.fontSize = baseFontSize * fontSizeCurve.Evaluate(time);
        time += Time.deltaTime;
    }
}

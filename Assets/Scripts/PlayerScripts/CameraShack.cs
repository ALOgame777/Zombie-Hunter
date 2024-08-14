using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShack : MonoBehaviour
{
    public float shakeAmount = 0.1f; // 흔들림의 세기
    public float shakeDuration = 0.1f; // 흔들림의 지속 시간
    private Vector3 originalPosition;
    private float shakeTime;
    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (shakeTime > 0)
        {
            transform.position = originalPosition + Random.insideUnitSphere * shakeAmount;
            shakeTime -= Time.deltaTime;
        }
        else
        {
            shakeTime = 0;
            transform.position = originalPosition;
        }
    }
    public void TriggerShake(float duration)
    {
        shakeTime = duration;
    }
}

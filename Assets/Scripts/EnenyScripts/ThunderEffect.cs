using UnityEngine;
using System;
using System.Collections;


public class ThunderEffect : MonoBehaviour
{
    public AudioSource thunderAudioSource; // 천둥 소리가 나올 오디오 소스
    public Light lightningLight;           // 번개를 표현할 라이트
    public float lightningDuration = 0.2f; // 번개의 지속 시간
    public float thunderDelay = 1.0f;      // 번개 이후 천둥 소리가 나는 시간 지연

    void Start()
    {
        lightningLight.enabled = false; // 초기에는 라이트를 꺼두기
    }

    public void TriggerThunderEffect()
    {
        StartCoroutine(ThunderCoroutine());
    }

    IEnumerator ThunderCoroutine()
    {
        // 번개 효과 시작
        lightningLight.enabled = true;
        yield return new WaitForSeconds(lightningDuration);
        lightningLight.enabled = false;

        // 천둥 소리 재생 (번개 후 약간의 지연을 줄 수 있음)
        yield return new WaitForSeconds(thunderDelay);
        thunderAudioSource.Play();
    }
}

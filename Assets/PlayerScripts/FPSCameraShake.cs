using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCameraShake : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public float shakeIntensity = 1f;
    public float shakeTime = 0.2f;

    private CinemachineBasicMultiChannelPerlin noise;
    private float shakeTimer;

    void Start()
    {
        if (virtualCamera != null)
        {
            noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }
    }

    void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0)
            {
                noise.m_AmplitudeGain = 0f;
            }
        }

        // 테스트를 위해 스페이스바를 누르면 카메라 흔들림 효과 적용
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShakeCamera();
        }
    }

    public void ShakeCamera()
    {
        noise.m_AmplitudeGain = shakeIntensity;
        shakeTimer = shakeTime;
    }
}

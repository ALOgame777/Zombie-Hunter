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

        // �׽�Ʈ�� ���� �����̽��ٸ� ������ ī�޶� ��鸲 ȿ�� ����
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

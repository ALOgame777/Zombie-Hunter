using UnityEngine;
using Cinemachine;

public class FPSCameraShake : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public float shakeIntensity = 1f;
    public float shakeTime = 0.2f;

    [Header("Walking Effect")]
    public float walkingBobbingSpeed = 14f;
    public float bobbingAmount = 0.05f;
    public float horizontalBobbingAmount = 0.02f;

    private CinemachineBasicMultiChannelPerlin noise;
    private float shakeTimer;
    private float defaultPosY = 0;
    private float timer = 0;

    void Start()
    {
        if (virtualCamera != null)
        {
            noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }
        defaultPosY = virtualCamera.transform.localPosition.y;
    }

    void Update()
    {
        HandleShake();
        HandleWalkingEffect();
    }

    void HandleShake()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0)
            {
                noise.m_AmplitudeGain = 0f;
            }
        }
    }

    void HandleWalkingEffect()
    {
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f)
        {
            // Player is moving
            timer += Time.deltaTime * walkingBobbingSpeed;
            virtualCamera.transform.localPosition = new Vector3(
                Mathf.Sin(timer) * horizontalBobbingAmount,
                defaultPosY + Mathf.Sin(timer * 2) * bobbingAmount,
                virtualCamera.transform.localPosition.z);
        }
        else
        {
            // Idle
            timer = 0;
            virtualCamera.transform.localPosition = new Vector3(
                Mathf.Lerp(virtualCamera.transform.localPosition.x, 0, Time.deltaTime * walkingBobbingSpeed),
                Mathf.Lerp(virtualCamera.transform.localPosition.y, defaultPosY, Time.deltaTime * walkingBobbingSpeed),
                virtualCamera.transform.localPosition.z);
        }
    }

    public void ShakeCamera(float intensity, float time)
    {
        noise.m_AmplitudeGain = intensity;
        shakeTimer = time;
    }
}
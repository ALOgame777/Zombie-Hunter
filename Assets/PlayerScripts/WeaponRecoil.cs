using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    [Header("Rotation Recoil")]
    public float recoilX = 2f;
    public float recoilY = 2f;
    public float recoilZ = 0.35f;

    [Header("Position Recoil")]
    public float positionalRecoilZ = 0.1f;
    public float positionalRecoilY = 0.05f;

    [Header("Recoil Settings")]
    public float snappiness = 6f;
    public float returnSpeed = 2f;

    private Vector3 currentRotation;
    private Vector3 targetRotation;
    private Quaternion initialRotation;

    private Vector3 currentPosition;
    private Vector3 targetPosition;
    private Vector3 initialPosition;

    void Start()
    {
        initialRotation = transform.localRotation;
        initialPosition = transform.localPosition;
        currentRotation = initialRotation.eulerAngles;
        currentPosition = initialPosition;
        targetRotation = currentRotation;
        targetPosition = currentPosition;
    }

    void Update()
    {
        // 회전 처리
        targetRotation = Vector3.Lerp(targetRotation, initialRotation.eulerAngles, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.fixedDeltaTime);
        transform.localRotation = initialRotation * Quaternion.Euler(currentRotation - initialRotation.eulerAngles);

        // 위치 처리
        targetPosition = Vector3.Lerp(targetPosition, initialPosition, returnSpeed * Time.deltaTime);
        currentPosition = Vector3.Lerp(currentPosition, targetPosition, snappiness * Time.fixedDeltaTime);
        transform.localPosition = currentPosition;
    }

    public void Recoil()
    {
        // 회전 반동
        targetRotation += new Vector3(
            -recoilX,
            Random.Range(-recoilY, recoilY),
            Random.Range(-recoilZ, recoilZ)
        );

        // 위치 반동
        targetPosition += new Vector3(
            0,
            Random.Range(-positionalRecoilY, positionalRecoilY),
            -positionalRecoilZ
        );
    }
}
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class WeaponZoom : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float zoomedOutFOV = 60f;
    [SerializeField] private float zoomedInFOV = 20f;
    [SerializeField] private float zoomSpeed = 10f;

    private bool isZoomed = false;
    private float targetFOV;

    private void Start()
    {
        if (virtualCamera == null)
        {
            Debug.LogError("Virtual Camera is not assigned to WeaponZoom script!");
            return;
        }

        targetFOV = zoomedOutFOV;
        virtualCamera.m_Lens.FieldOfView = zoomedOutFOV;
    }
    private void Update()
    {
        if (virtualCamera == null) return;

        if (Input.GetMouseButtonDown(1))
        {
            ToggleZoom();
        }

        // Smoothly adjust FOV
        virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(virtualCamera.m_Lens.FieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
    }

    private void ToggleZoom()
    {
        isZoomed = !isZoomed;
        targetFOV = isZoomed ? zoomedInFOV : zoomedOutFOV;
    }

    private void OnDisable()
    {
        if (virtualCamera != null)
        {
            // Reset when disabled
            virtualCamera.m_Lens.FieldOfView = zoomedOutFOV;
            isZoomed = false;
        }
    }
}

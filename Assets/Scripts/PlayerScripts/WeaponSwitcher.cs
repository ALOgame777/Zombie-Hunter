using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI 요소 사용을 위해 추가

public class WeaponSwitcher : MonoBehaviour
{
    public int currentWeapon = 0;
    public RawImage[] weaponImages; // UI에 표시할 무기 사진들

    void Start()
    {
        SetWeaponActive();
        UpdateWeaponUI(); // 초기 상태에서 UI 설정
    }

    void Update()
    {
        int previousWeapon = currentWeapon;

        ProcessKeyInput();
        ProcessScrollWheel();

        if (previousWeapon != currentWeapon)
        {
            SetWeaponActive();
            UpdateWeaponUI(); // 무기 변경 시 UI 업데이트
        }
    }

    private void ProcessScrollWheel()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (currentWeapon >= transform.childCount - 1)
            {
                currentWeapon = 0;
            }
            else
            {
                currentWeapon++;
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (currentWeapon <= 0)
            {
                currentWeapon = transform.childCount - 1;
            }
            else
            {
                currentWeapon--;
            }
        }
    }

    private void ProcessKeyInput()
    {
        // 1번 무기 선택
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentWeapon = 0;
        }

        // 2번 무기 선택
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentWeapon = 1;
        }

        // 3번 무기 선택
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentWeapon = 2;
        }
    }

    private void SetWeaponActive()
    {
        int weaponIndex = 0;

        foreach (Transform weapon in transform)
        {
            Weapon weaponComponent = weapon.GetComponent<Weapon>();

            if (weaponIndex == currentWeapon)
            {
                weapon.gameObject.SetActive(true);
                if (weaponComponent != null)
                {
                    weaponComponent.displayAmmo = true; // DisplayAmmo 활성화
                    weaponComponent.DisplayAmmo(); // 현재 무기의 탄약 정보를 업데이트
                }
            }
            else
            {
                weapon.gameObject.SetActive(false);
                if (weaponComponent != null)
                {
                    weaponComponent.displayAmmo = false; // DisplayAmmo 비활성화
                }
            }
            weaponIndex++;
        }
    }

    private void UpdateWeaponUI()
    {
        // 모든 무기 이미지를 비활성화
        foreach (RawImage img in weaponImages)
        {
            img.gameObject.SetActive(false);
        }

        // 현재 선택된 무기의 이미지만 활성화
        if (currentWeapon < weaponImages.Length)
        {
            weaponImages[currentWeapon].gameObject.SetActive(true);
        }
    }
}



//기존 식
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class WeaponSwitcher : MonoBehaviour
//{
//    public int currentWeapon = 0;

//    void Start()
//    {
//        SetWeaponActive();
//    }

//    void Update()
//    {
//        int previousWeapon = currentWeapon;

//        ProcessKeyInput();
//        ProcessScrollWheel();

//        if (previousWeapon != currentWeapon)
//        {
//            SetWeaponActive();
//        }
//    }

//    private void ProcessScrollWheel()
//    {
//        if (Input.GetAxis("Mouse ScrollWheel") < 0)
//        {
//            if (currentWeapon >= transform.childCount - 1)
//            {
//                currentWeapon = 0;
//            }
//            else
//            {
//                currentWeapon++;
//            }
//        }

//        if (Input.GetAxis("Mouse ScrollWheel") > 0)
//        {
//            if (currentWeapon <= 0)
//            {
//                currentWeapon = transform.childCount - 1;
//            }
//            else
//            {
//                currentWeapon--;
//            }
//        }
//    }

//    private void ProcessKeyInput()
//    {
//        if (Input.GetKeyDown(KeyCode.Alpha1))
//        {
//            currentWeapon = 0;
//        }
//        if (Input.GetKeyDown(KeyCode.Alpha2))
//        {
//            currentWeapon = 1;
//        }
//        if (Input.GetKeyDown(KeyCode.Alpha3))
//        {
//            currentWeapon = 2;
//        }
//    }

//    private void SetWeaponActive()
//    {
//        int weaponIndex = 0;

//        foreach (Transform weapon in transform)
//        {
//            Weapon weaponComponent = weapon.GetComponent<Weapon>();

//            if (weaponIndex == currentWeapon)
//            {
//                weapon.gameObject.SetActive(true);
//                if (weaponComponent != null)
//                {
//                    weaponComponent.displayAmmo = true; // DisplayAmmo 활성화
//                    weaponComponent.DisplayAmmo(); // 현재 무기의 탄약 정보를 업데이트
//                }
//            }
//            else
//            {
//                weapon.gameObject.SetActive(false);
//                if (weaponComponent != null)
//                {
//                    weaponComponent.displayAmmo = false; // DisplayAmmo 비활성화
//                }
//            }
//            weaponIndex++;
//        }
//    }
//}



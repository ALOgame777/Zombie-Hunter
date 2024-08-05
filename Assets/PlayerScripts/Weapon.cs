using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    public Camera FPCamera;
    public float range = 100.0f;
    public int damage = 50;
    public ParticleSystem muzzle;
    public GameObject hitEffet;
    public Ammo ammoSlot;
    public AmmoType ammoType;
    public float timeBetweenShots = 0.5f;
    public Text ammoText;
    public Text magazineText;  // 기존 Text 컴포넌트를 사용
    bool canShoot = true;
    public FPSCameraShake cameraShake;

    public int maxMagazineSize = 30;
    private int currentMagazineAmmo;

    private void Awake()
    {
        // If FPSCameraShake is attached to the same GameObject
        cameraShake = GetComponent<FPSCameraShake>();

        // If FPSCameraShake is attached to the Camera or another GameObject
        if (cameraShake == null)
        {
            cameraShake = FindObjectOfType<FPSCameraShake>();
            if (cameraShake == null)
            {
                Debug.LogError("FPSCameraShake script is not found in the scene. Please attach the script to a GameObject.");
            }
        }
        currentMagazineAmmo = maxMagazineSize;
    }
    private void OnEnable()
    {
        canShoot = true;
        DisplayAmmo();
    }

    void Update()
    {
        DisplayAmmo();

        if (Input.GetMouseButtonDown(0) && canShoot)
        {
            StartCoroutine(Shoot());
        }

        if (Input.GetKeyDown(KeyCode.R) && canShoot)
        {
            StartCoroutine(Reload());
        }
    }

    private void DisplayAmmo()
    {
        int currentAmmo = ammoSlot.GetCurrentAmmo(ammoType);
        ammoText.text = currentAmmo.ToString();
        magazineText.text = currentMagazineAmmo.ToString(); // 탄창의 남은 탄약을 표시
    }
    IEnumerator Shoot()
    {
        canShoot = false;

        if (currentMagazineAmmo > 0)
        {
            if (cameraShake != null)
            {
                cameraShake.ShakeCamera();
            }

            PlayMuzzleFlash();
            ProcessRayCast();
            currentMagazineAmmo--;
        }
        else
        {
            Debug.Log("No ammo in magazine");
        }

        yield return new WaitForSeconds(timeBetweenShots);
        canShoot = true;
    }

    private void PlayMuzzleFlash()
    {
        muzzle.Play();
    }

    private void ProcessRayCast()
    {
        RaycastHit hit;
        if( Physics.Raycast(FPCamera.transform.position, FPCamera.transform.forward, out hit, range))
        {
            CreateHitImpact(hit);
            EnemyFSM target = hit.transform.GetComponent<EnemyFSM>();
            BOSS target1 = hit.transform.GetComponent<BOSS>();
            TREE target2 = hit.transform.GetComponent<TREE>();
            if (target != null)
            {
                target.HitEnemy(damage);
            }
            if (target1 != null)
            {
                target1.HitEnemy(damage);
            }
            if (target2 != null)
            {
                target2.TakeDamage(damage);
            }


        }
        else
        {
            return;
        }

        
    }
    IEnumerator Reload()
    {
        canShoot = false;
        Debug.Log("Reloading...");

        yield return new WaitForSeconds(2.0f); // Assuming reload takes 2 seconds

        int ammoNeeded = maxMagazineSize - currentMagazineAmmo;
        int currentAmmo = ammoSlot.GetCurrentAmmo(ammoType);

        if (currentAmmo >= ammoNeeded)
        {
            ammoSlot.ReduceCurrentAmmo(ammoType, ammoNeeded);
            currentMagazineAmmo = maxMagazineSize;
        }
        else
        {
            ammoSlot.ReduceCurrentAmmo(ammoType, currentAmmo);
            currentMagazineAmmo += currentAmmo;
        }

        canShoot = true;
        Debug.Log("Reloaded");
        DisplayAmmo();
    }

    private void CreateHitImpact(RaycastHit hit)
    {
        GameObject impact = Instantiate(hitEffet, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(impact, 4);
    }
}

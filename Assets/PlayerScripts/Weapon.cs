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
    bool canShoot = true;

    private void OnEnable()
    {
        canShoot = true;
    }

    void Update()
    {
        DisplayAmmo();
        if (Input.GetMouseButtonDown(0) && canShoot == true)
        {
            StartCoroutine(Shoot());
        }
    }

    private void DisplayAmmo()
    {
        int currentAmmo = ammoSlot.GetCurrentAmmo(ammoType);
        ammoText.text = currentAmmo.ToString();
    }
    IEnumerator Shoot()
    {
        canShoot = false;
        if (ammoSlot.GetCurrentAmmo(ammoType) > 0)
        {
            PlayMuzzleFlash();
            ProcessRayCast();
            ammoSlot.ReduceCurrentAmmo(ammoType);
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

    private void CreateHitImpact(RaycastHit hit)
    {
        GameObject impact = Instantiate(hitEffet, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(impact, 4);
    }
}

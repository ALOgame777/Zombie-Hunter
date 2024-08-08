using System;
using System.Collections;
using System.Collections.Generic;
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
    public Text magazineText;  // ���� Text ������Ʈ�� ���
    bool canShoot = true;
    public bool displayAmmo = true; // DisplayAmmo �޼��带 Ȱ��ȭ/��Ȱ��ȭ�ϴ� �÷���
    public Text reloadText; // ���ε� �߰��� �κ�
    public WeaponRecoil recoil;

    public int fireCount = 1;
    
    
    public int maxMagazineSize = 30;
    private int currentMagazineAmmo;
    private void Awake()
    {
        recoil = GetComponent<WeaponRecoil>();
        currentMagazineAmmo = maxMagazineSize;
    }
    private void OnEnable()
    {
        canShoot = true;
        DisplayAmmo();
        reloadText.gameObject.SetActive(false); // ���ε� �ؽ�Ʈ �ʱ� ��Ȱ��ȭ
    }

    void Update()
    {
        if (displayAmmo)
        {
            DisplayAmmo();
        }
        if (Input.GetMouseButton(0) && canShoot)
        {
            StartCoroutine(Shoot());
        }

        if (Input.GetKeyDown(KeyCode.R) && canShoot)
        {
            StartCoroutine(Reload());
        }
    }

    public void DisplayAmmo()
    {
        int currentAmmo = ammoSlot.GetCurrentAmmo(ammoType);
        ammoText.text = currentAmmo.ToString();
        magazineText.text = currentMagazineAmmo.ToString();  // ���� ������ ź�� ��
    }
    IEnumerator Shoot()
    {
        for (int i = 0; i < fireCount; i++)
        {
            canShoot = false;
            if (ammoType == AmmoType.Rockets)
            {
                PlayMuzzleFlash();
                ProcessRayCast();
                currentMagazineAmmo--;
                ammoText.text = "0";
            }
            if (currentMagazineAmmo > 0)
            {
                PlayMuzzleFlash();
                ProcessRayCast();
                recoil.Recoil();
                currentMagazineAmmo--;

            }
            else
            {
                StartCoroutine(Reload());
            }

            yield return new WaitForSeconds(timeBetweenShots);
            canShoot = true;
        }
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
        reloadText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2.0f);

        int ammoNeeded = maxMagazineSize - currentMagazineAmmo;
        int currentAmmo = ammoSlot.GetCurrentAmmo(ammoType);

        if (currentAmmo > ammoNeeded)
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
        reloadText.gameObject.SetActive(false);
        DisplayAmmo();
    }
    private void CreateHitImpact(RaycastHit hit)
    {
        GameObject impact = Instantiate(hitEffet, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(impact, 4);
    }

    public void IncreaseAttackPower(int percentage)
    {
        // ���ݷ� ���� �� �Ҽ��� ���ϸ� ���� (���������� ó��)
        damage = Mathf.FloorToInt(damage * (1 + percentage / 100.0f));
    }
}

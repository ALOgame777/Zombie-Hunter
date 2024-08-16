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
    public Text magazineText;  // 기존 Text 컴포넌트를 사용
    bool canShoot = true;
    public bool displayAmmo = true; // DisplayAmmo 메서드를 활성화/비활성화하는 플래그
    public Text reloadText; // 리로드 추가된 부분
    public WeaponRecoil recoil;

    public int fireCount = 1;

    public RectTransform crosshair; // 크로스헤어의 RectTransform 참조
    public float shakeAmount = 5f; // 흔들림의 정도

    public float enlargeAmount = 1.2f; // 크로스헤어 크기 확대 비율
    public float enlargeDuration = 0.1f; // 크로스헤어 확대 지속 시간

    public int maxMagazineSize = 30;
    private int currentMagazineAmmo;
    public AudioClip CarbineshootSound;
    public AudioClip CarbinereloadAudio;
    public AudioClip AK47shootSound;
    public AudioClip AK47reloadAudio;
    public AudioClip RPG7shootSound;
    public AudioClip RPG7reloadAudio;

    private bool isCrosshairEnlarging = false; // 크로스헤어 확대 여부를 확인하는 변수

    private AudioSource audioSource;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        recoil = GetComponent<WeaponRecoil>();
        currentMagazineAmmo = maxMagazineSize;
    }
    private void OnEnable()
    {
        canShoot = true;
        DisplayAmmo();
        reloadText.gameObject.SetActive(false); // 리로드 텍스트 초기 비활성화
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
        magazineText.text = currentMagazineAmmo.ToString();  // 현재 장전된 탄약 수
    }
    IEnumerator Shoot()
    {
        for (int i = 0; i < fireCount; i++)
        {
            canShoot = false;
            if (ammoType == AmmoType.Rockets && currentMagazineAmmo > 0)
            {
                PlayMuzzleFlash();
                ProcessRayCast();
                currentMagazineAmmo--;
                recoil.Recoil();
                if (CompareTag("RPG"))
                {
                    // 크로스헤어 흔들림 코루틴 시작
                    StartCoroutine(ShakeCrosshair());
                    audioSource.clip = RPG7shootSound;
                    audioSource.Play();
                }
            }
            if (currentMagazineAmmo > 0)
            {
                PlayMuzzleFlash();
                ProcessRayCast();
                recoil.Recoil();
                currentMagazineAmmo--;
                // 크로스헤어 흔들림 코루틴 시작
                StartCoroutine(ShakeCrosshair());
                if (CompareTag("Carbine"))
                {
                    // 크로스헤어 흔들림 코루틴 시작
                    StartCoroutine(ShakeCrosshair());
                    audioSource.clip = CarbineshootSound;
                    audioSource.Play();
                }
                if(CompareTag("AK47"))
                {
                    audioSource.clip = AK47shootSound;
                    audioSource.Play();
                }
                
            }
            else
            {
                StartCoroutine(Reload());

            }
            // 크로스헤어 크기 확대 코루틴 실행
            if (crosshair != null && !isCrosshairEnlarging)
            {
                StartCoroutine(EnlargeCrosshair());
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
        // 크로스헤어 비활성화
        if (crosshair != null)
        {
            crosshair.gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(1.0f);

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
        if (CompareTag("Carbine"))
        {
            audioSource.clip = CarbinereloadAudio;
            audioSource.Play();
        }
        if (CompareTag("AK47"))
        {
            audioSource.clip = AK47reloadAudio;
            audioSource.Play();
        }
        if (CompareTag("RPG"))
        {
            audioSource.clip = RPG7reloadAudio;
            audioSource.Play();
        }
        reloadText.gameObject.SetActive(false);
        // 크로스헤어 다시 활성화
        if (crosshair != null)
        {
            crosshair.gameObject.SetActive(true);
        }
        DisplayAmmo();
        
    }

    IEnumerator ShakeCrosshair()
    {
        Vector2 originalPosition = crosshair.anchoredPosition;

        // 짧은 시간 동안 크로스헤어가 흔들리게 함
        for (float t = 0; t < 0.1f; t += Time.deltaTime)
        {
            crosshair.anchoredPosition = originalPosition + UnityEngine.Random.insideUnitCircle * shakeAmount;
            yield return null;
        }

        // 원래 위치로 복구
        crosshair.anchoredPosition = originalPosition;
    }

    IEnumerator EnlargeCrosshair()
    {
        isCrosshairEnlarging = true; // 크로스헤어 확대 시작

        Vector3 originalScale = crosshair.localScale; // 원래 크기 저장
        Vector3 targetScale = originalScale * enlargeAmount; // 목표 크기 설정

        // 크로스헤어 크기를 부드럽게 확대
        float elapsedTime = 0f;
        while (elapsedTime < enlargeDuration)
        {
            crosshair.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / enlargeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 목표 크기까지 확대
        crosshair.localScale = targetScale;

        // 원래 크기로 다시 축소
        elapsedTime = 0f;
        while (elapsedTime < enlargeDuration)
        {
            crosshair.localScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / enlargeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 원래 크기로 복구
        crosshair.localScale = originalScale;

        isCrosshairEnlarging = false; // 크로스헤어 확대 종료
    }
    private void CreateHitImpact(RaycastHit hit)
    {
        GameObject impact = Instantiate(hitEffet, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(impact, 4);
    }

    public void IncreaseAttackPower(int percentage)
    {
        // 공격력 증가 후 소수점 이하를 버림 (정수형으로 처리)
        damage = Mathf.FloorToInt(damage * (1 + percentage / 100.0f));
    }
}

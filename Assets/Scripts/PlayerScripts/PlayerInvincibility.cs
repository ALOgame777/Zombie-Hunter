using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI 네임스페이스 추가

public class PlayerInvincibility : MonoBehaviour
{
    public static PlayerInvincibility pi;
    public float invincibilityDuration = 2f;
    private bool isInvincible = false;
    private CapsuleCollider[] capsuleColliders;

    // UI 관련 변수 추가
    public Text invincibilityTimerText;
    private float remainingInvincibilityTime;

    private void Awake()
    {
        if (pi == null)
        {
            pi = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartCoroutine(ApplyInvincibility());
    }

    public void Respawn()
    {
        StartCoroutine(ApplyInvincibility());
        CharacterStats.cs.setHealthTo(300);
    }

    IEnumerator ApplyInvincibility()
    {
        isInvincible = true;
        capsuleColliders = GetComponentsInChildren<CapsuleCollider>(true);
        foreach (CapsuleCollider col in capsuleColliders)
        {
            col.enabled = false;
        }

        // 무적 시간 카운트다운 및 UI 업데이트
        remainingInvincibilityTime = invincibilityDuration;
        while (remainingInvincibilityTime > 0)
        {
            UpdateInvincibilityTimerUI();
            yield return new WaitForSeconds(0.1f); // 0.1초마다 업데이트
            remainingInvincibilityTime -= 0.1f;
        }

        isInvincible = false;
        foreach (CapsuleCollider col in capsuleColliders)
        {
            col.enabled = true;
        }

        // UI 텍스트 초기화
        UpdateInvincibilityTimerUI();
    }

    void UpdateInvincibilityTimerUI()
    {
        if (invincibilityTimerText != null)
        {
            if (isInvincible)
            {
                invincibilityTimerText.text = $"무적: {remainingInvincibilityTime:F1}초";
            }
            else
            {
                invincibilityTimerText.text = ""; // 무적 상태가 아닐 때는 텍스트를 비움
            }
        }
    }

    public bool IsInvincible()
    {
        return isInvincible;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isInvincible)
        {
            Debug.Log("무적 상태로 충돌 무시");
            return;
        }
        Debug.Log("충돌 발생");
    }
}
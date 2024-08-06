using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInvincibility : MonoBehaviour
{
    public static PlayerInvincibility pi;
    public float invincibilityDuration = 2f;
    private bool isInvincible = false;
    private CapsuleCollider[] capsuleColliders; // Array to store CapsuleColliders
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
        // 게임 시작 시 무적 상태 적용
        StartCoroutine(ApplyInvincibility());
    }

    public void Respawn()
    {
        // 리스폰 시 호출될 메서드
        StartCoroutine(ApplyInvincibility());
        CharacterStats.cs.setHealthTo(300);
    }

    IEnumerator ApplyInvincibility()
    {
        isInvincible = true;
        Debug.Log("무적 상태 시작");

        // 자신과 자식 오브젝트에 있는 모든 CapsuleCollider를 가져옴
        capsuleColliders = GetComponentsInChildren<CapsuleCollider>(true);


        // 모든 CapsuleCollider를 비활성화
        foreach (CapsuleCollider col in capsuleColliders)
        {
            col.enabled = false;
        }

        yield return new WaitForSeconds(invincibilityDuration);

        isInvincible = false;
        Debug.Log("무적 상태 종료");

        // 무적 상태 종료 후 모든 CapsuleCollider를 다시 활성화
        foreach (CapsuleCollider col in capsuleColliders)
        {
            col.enabled = true;
        }
    }

    // 다른 스크립트에서 무적 상태를 확인할 때 사용할 수 있는 메서드
    public bool IsInvincible()
    {
        return isInvincible;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isInvincible)
        {
            // 무적 상태인 경우 충돌 무시
            Debug.Log("무적 상태로 충돌 무시");
            return;
        }

        // 충돌 처리 코드
        Debug.Log("충돌 발생");
    }
}

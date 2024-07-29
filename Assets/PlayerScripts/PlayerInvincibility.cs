using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInvincibility : MonoBehaviour
{
    public float invincibilityDuration = 2f;
    private bool isInvincible = false;
    private Collider playerCollider;
    void Start()
    {

        // 게임 시작 시 무적 상태 적용
        StartCoroutine(ApplyInvincibility());
    }

    public void Respawn()
    {
        // 리스폰 시 호출될 메서드
        StartCoroutine(ApplyInvincibility());
    }

    IEnumerator ApplyInvincibility()
    {
        isInvincible = true;
        Debug.Log("무적 상태 시작");

        // 무적 상태 동안 충돌체를 비활성화
        if (playerCollider != null)
        {
            playerCollider.enabled = false;
        }

        yield return new WaitForSeconds(invincibilityDuration);

        isInvincible = false;
        Debug.Log("무적 상태 종료");
        // 무적 상태 종료 후 충돌체를 다시 활성화
        if (playerCollider != null)
        {
            playerCollider.enabled = true;
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

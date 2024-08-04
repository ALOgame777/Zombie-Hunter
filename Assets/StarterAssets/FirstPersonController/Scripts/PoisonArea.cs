using System.Collections;
using UnityEngine;

public class PoisonArea : MonoBehaviour
{
    private float radius;
    private float duration;
    private BOSS boss;

    public void Initialize(float radius, float duration, BOSS boss)
    {
        this.radius = radius;
        this.duration = duration;
        this.boss = boss;

        StartCoroutine(PoisonAreaEffect());
    }

    IEnumerator PoisonAreaEffect()
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Player"))
                {
                    print("플레이어 독극물 영역에 있음");
                    boss.ApplyPoisonEffect();
                }
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}

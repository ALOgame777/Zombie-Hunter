
// 색깔 변경 해보자
using System.Collections; // 다른 유용한 기능들을 사용하기 위해
using UnityEngine; // Unity 관련 기능을 사용하기 위해

public class PoisonArea : MonoBehaviour
{
    private float radius; // 독극물 영역의 반경
    private float duration; // 독극물 효과의 지속 시간
    private BOSS boss; // BOSS 클래스의 객체를 저장할 변수
    private Renderer rend; // Renderer 컴포넌트를 저장할 변수

    public void Initialize(float radius, float duration, BOSS boss)
    {
        this.radius = radius; // 독극물 영역의 반경 설정
        this.duration = duration; // 독극물 효과의 지속 시간 설정
        this.boss = boss; // boss 객체 설정

        rend = GetComponent<Renderer>(); // 현재 오브젝트의 Renderer 컴포넌트를 가져옴

        StartCoroutine(PoisonAreaEffect()); // PoisonAreaEffect라는 코루틴을 시작해
    }

    IEnumerator PoisonAreaEffect()
    {
        float elapsedTime = 0f; // 경과 시간을 저장하는 변수
        bool isBlue = true; // 색깔이 파랑인지 보라색인지 구분할 변수

        while (elapsedTime < duration)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Player"))
                {
                    print("플레이어 독극물 영역에 있음"); // 콘솔에 메시지 출력
                    boss.ApplyPoisonEffect(); // boss 객체의 ApplyPoisonEffect 함수를 호출해
                }
            }

            // 색깔을 파랑과 보라색으로 바꾸기
            if (isBlue)
            {
                rend.material.color = Color.blue; // 색깔을 파랑으로 설정
            }
            else
            {
                rend.material.color = new Color(0.5f, 0f, 0.5f); // 색깔을 보라색으로 설정
            }

            isBlue = !isBlue; // 색깔을 바꾸기 위해 변수 값을 반전시킴

            elapsedTime += Time.deltaTime; // 경과 시간을 업데이트해
            yield return new WaitForSeconds(0.5f); // 0.5초마다 색깔을 바꿈
        }

        // 코루틴이 끝나면 색깔을 초기화
        rend.material.color = Color.clear; // 색깔을 투명으로 설정
    }
}


//using System.Collections; // 다른 유용한 기능들을 사용하기 위해
//using UnityEngine; // Unity 관련 기능을 사용하기 위해

//public class PoisonArea : MonoBehaviour // PoisonArea라는 새로운 클래스를 만들고, Unity의 MonoBehaviour를 상속받아
//{
//    private float radius; // 독극물 영역의 반경
//    private float duration; // 독극물 효과의 지속 시간
//    private BOSS boss; // BOSS 클래스의 객체를 저장할 변수

//    // Initialize 함수는 PoisonArea를 설정하는 함수야
//    public void Initialize(float radius, float duration, BOSS boss)
//    {
//        this.radius = radius; // 독극물 영역의 반경 설정
//        this.duration = duration; // 독극물 효과의 지속 시간 설정
//        this.boss = boss; // boss 객체 설정

//        StartCoroutine(PoisonAreaEffect()); // PoisonAreaEffect라는 코루틴을 시작해
//    }

//    // PoisonAreaEffect는 독극물 영역의 효과를 구현하는 코루틴
//    IEnumerator PoisonAreaEffect()
//    {
//        float elapsedTime = 0f; // 경과 시간을 저장하는 변수

//        // 독극물 효과가 지속 시간보다 짧을 때까지 반복해
//        while (elapsedTime < duration)
//        {
//            // 현재 위치를 중심으로 반경만큼의 영역에서 충돌체를 찾음
//            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
//            foreach (Collider hitCollider in hitColliders)
//            {
//                // 충돌체의 태그가 "Player"일 때
//                if (hitCollider.CompareTag("Player"))
//                {
//                    print("플레이어 독극물 영역에 있음"); // 콘솔에 메시지 출력
//                    boss.ApplyPoisonEffect(); // boss 객체의 ApplyPoisonEffect 함수를 호출해
//                }
//            }

//            elapsedTime += Time.deltaTime; // 경과 시간을 업데이트해
//            yield return null; // 다음 프레임까지 기다려
//        }
//    }
//}

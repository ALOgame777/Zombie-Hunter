
// 이거 보스 죽이고 동영상 테스트 
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BOSS : MonoBehaviour
{

    // 에너미 상태 상수
    public enum BossState
    {
        Idle,
        Move,
        Attack,
        Return,
        Damaged,
        Die
    }
    // 에너미 상태 변수
    public BossState m_state;

    // 플레이어 발견 범위
    public float findDistance = 80f;

    // 플레이어 트랜스폼
    public Transform player;

    // 공격 가능 범위
    public float attackDistance = 200f;

    // 이동 속도
    public float moveSpeed = 2f;

    // 캐릭터 컨트롤러 컴포넌트
    CharacterController cc;

    // 누적 시간
    float currentTime = 0;

    // 공격 딜레이 시간
    public float attackDelay = 2f;

    // 에너미의 공격력
    public int attackPower = 30;

    // 이동 가능 범위
    public float moveDistance = 200f;

    public GameObject Endingvideo;

    // 에너미의 체력
    public int hp = 50000;

   


    void Start()
    {
        // 최초의 에너미  상태는 대기(Idle)로 한다.
        m_state = BossState.Idle;

        // 플레이어의 트랜스폼 컴포넌트 받아오기
        player = GameObject.Find("PlayerCapsule").transform;

        // 캐릭터 컨트롤러 컴포넌트 받아오기
        cc = GetComponent<CharacterController>();


    }

    void Update()
    {
        // 현재 상태를 체크해 해당 상태별로 정해진 기능을 수행하게 하고싶다.
        switch (m_state)
        {
            case BossState.Idle:
                Idle();
                break;
            case BossState.Move:
                Move();
                break;
            case BossState.Attack:
                Attack();
                break;
            case BossState.Damaged:
                //Damaged();
                break;
            case BossState.Die:
                //Die();
                break;
        }

    }

    // 데미지 실행 함수
    public void HitEnemy(int hitPower)
    {
        // 만일 이미 피격 상태이거나 사망 상태 또는 복귀 상태라면 아무런 처리도 하지 않고 함수를 종료한다.
        if (m_state == BossState.Damaged || m_state == BossState.Die || m_state == BossState.Return)
        {
            return;
        }

        // 플레이어의 공격력 만큼 에너미 체력을 감소
        hp -= hitPower;
        // 에너미의 체력이 0보다 크면 피격 상태로 전환
        if (hp > 0)
        {
            m_state = BossState.Damaged;
            print("상태 전환 : any state -> damaged");
            Damaged();
        }
        // 그렇지 않다면 죽음 상태로 전환
        else
        {
            m_state = BossState.Die;
            print("상태 전환 : any state -> die");
            Die();
        }
    }

    private void Damaged()
    {
        // 피격 상태를 처리하기 위한 코루틴 실행
        StartCoroutine(DamageProcess());
    }
    // 데미지 처리용 코루틴 함수
    IEnumerator DamageProcess()
    {
        // 피격 모션 시간만큼 기다린다.
        yield return new WaitForSeconds(0.5f);

        // 현재 상태를 이동 상태로 전환
        m_state = BossState.Move;
        print("상태 전환 : damaged -> move");
    }

    void Idle()
    {
        // 만일, 플레이어와의 거리가 액션 시작 범위 이내라면 Move 상태로 전환한다.
        if (Vector3.Distance(transform.position, player.position) < findDistance)
        {
            m_state = BossState.Move;
            print("상태 전환 : Idle -> Move");
        }
    }

    void Move()
    {
        print(player.position);
        // 만일 플레이어와의 거리가 공격 범위 밖이라면 플레이어를 향해 이동한다.
        if (Vector3.Distance(transform.position, player.position) > attackDistance)
        {
            // 이동 방향 설정
            Vector3 dir = (player.position - transform.position).normalized;

            // 캐릭터 컨트롤러를 이용해 이동하기
            cc.Move(dir * moveSpeed * Time.deltaTime);

            // 플레이어와의 거리가 공격범위 안이라면 현재 상태를 공격으로 전환한다.
            if (Vector3.Distance(transform.position, player.position) < attackDistance)
            {
                print("상태 전환 : Move -> Attack");
                m_state = BossState.Attack;
            }


        }
        //그렇지 않다면 현재 상태를 공격으로 전환한다.
        else
        {
            m_state = BossState.Attack;
            print("상태 전환 : Move -> Attack");

            // 누적 시간을 공격 딜레이 시간 만큼 미리 진행시켜 놓는다.
            currentTime = attackDelay;
        }
    }
    void Attack()
    {
        //만일 플레이어가 공격 범위 이내에 있다면 플레이어를 공격한다.
        if (Vector3.Distance(transform.position, player.position) < attackDistance)
        {
            // 일정 시간마다 플레이어를 공격한다.
            currentTime += Time.deltaTime;
            if (currentTime > attackDelay)
            {
                player.GetComponent<CharacterStats>().TakeDamage(attackPower);
                currentTime = 0;
            }
        }
        //그렇지 않다면 현재 상태를 이동으로 전환한다(추격)
        else
        {
            m_state = BossState.Move;
            print("상태 전환 : Attack -> Move");
            currentTime = 0;
        }
    }


    private void OnTriggerStay(Collider other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            Attack();  // 플레이어와 충돌 시 Attack 메서드 호출
        }
    }
    // 죽음 상태 함수
    void Die()
    {
        // 진행 중인 피격 코루틴을 중지
        StopAllCoroutines();

        // 죽음 상태를 처리하기 위한 코루틴
        StartCoroutine(DieProcess());
    }

    IEnumerator DieProcess()
    {
        // 캐릭터 컨트롤러 컴포넌트를 비활성화시킨다
        cc.enabled = false;

        // 2초 동안 기다린 후에 자기 자신을 제거한다
        yield return new WaitForSeconds(5f);
        print("소멸");
        Destroy(gameObject);
        Endingvideo.SetActive(true);
        Time.timeScale = 0;

    }
   
    //Vector2 newPos = Random.insideUnitCircle * initPreferences.patrolRadius;
    // patrolNext = patrolCenter + new Vector3(newPos.x, 0, newPos.y);
    // myState = EnemyState.Idle;
    // idleTime = Random.Range(2.0f, 3.0f);

}



//// 이거 보스 정상 작동함
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class BOSS : MonoBehaviour
//{

//    // 에너미 상태 상수
//    public enum BossState
//    {
//        Idle,
//        Move,
//        Attack,
//        Return,
//        Damaged,
//        Die
//    }
//    // 에너미 상태 변수
//    public BossState m_state;

//    // 플레이어 발견 범위
//    public float findDistance = 80f;

//    // 플레이어 트랜스폼
//    public Transform player;

//    // 공격 가능 범위
//    public float attackDistance = 200f;

//    // 이동 속도
//    public float moveSpeed = 2f;

//    // 캐릭터 컨트롤러 컴포넌트
//    CharacterController cc;

//    // 누적 시간
//    float currentTime = 0;

//    // 공격 딜레이 시간
//    public float attackDelay = 2f;

//    // 에너미의 공격력
//    public int attackPower = 30;

//    // 이동 가능 범위
//    public float moveDistance = 200f;

//    // 에너미의 체력
//    public int hp = 50000;

//    public CharacterStats playerStats;

//    void Start()
//    {
//        // 최초의 에너미  상태는 대기(Idle)로 한다.
//        m_state = BossState.Idle;

//        // 플레이어의 트랜스폼 컴포넌트 받아오기
//        player = GameObject.Find("PlayerCapsule").transform;

//        // 캐릭터 컨트롤러 컴포넌트 받아오기
//        cc = GetComponent<CharacterController>();

//        playerStats = CharacterStats.cs;
//    }

//    void Update()
//    {
//        // 현재 상태를 체크해 해당 상태별로 정해진 기능을 수행하게 하고싶다.
//        switch (m_state)
//        {
//            case BossState.Idle:
//                Idle();
//                break;
//            case BossState.Move:
//                Move();
//                break;
//            case BossState.Attack:
//                Attack();
//                break;
//            case BossState.Damaged:
//                //Damaged();
//                break;
//            case BossState.Die:
//                //Die();
//                break;
//        }

//    }

//    // 데미지 실행 함수
//    public void HitEnemy(int hitPower)
//    {
//        // 만일 이미 피격 상태이거나 사망 상태 또는 복귀 상태라면 아무런 처리도 하지 않고 함수를 종료한다.
//        if (m_state == BossState.Damaged || m_state == BossState.Die || m_state == BossState.Return)
//        {
//            return;
//        }

//        // 플레이어의 공격력 만큼 에너미 체력을 감소
//        hp -= hitPower;
//        // 에너미의 체력이 0보다 크면 피격 상태로 전환
//        if (hp > 0)
//        {
//            m_state = BossState.Damaged;
//            print("상태 전환 : any state -> damaged");
//            Damaged();
//        }
//        // 그렇지 않다면 죽음 상태로 전환
//        else
//        {
//            m_state = BossState.Die;
//            print("상태 전환 : any state -> die");
//            Die();
//        }
//    }

//    private void Damaged()
//    {
//        // 피격 상태를 처리하기 위한 코루틴 실행
//        StartCoroutine(DamageProcess());
//    }
//    // 데미지 처리용 코루틴 함수
//    IEnumerator DamageProcess()
//    {
//        // 피격 모션 시간만큼 기다린다.
//        yield return new WaitForSeconds(0.5f);

//        // 현재 상태를 이동 상태로 전환
//        m_state = BossState.Move;
//        print("상태 전환 : damaged -> move");
//    }

//    void Idle()
//    {
//        // 만일, 플레이어와의 거리가 액션 시작 범위 이내라면 Move 상태로 전환한다.
//        if (Vector3.Distance(transform.position, player.position) < findDistance)
//        {
//            m_state = BossState.Move;
//            print("상태 전환 : Idle -> Move");
//        }
//    }

//    void Move()
//    {
//        print(player.position);
//        // 만일 플레이어와의 거리가 공격 범위 밖이라면 플레이어를 향해 이동한다.
//        if (Vector3.Distance(transform.position, player.position) > attackDistance)
//        {
//            // 이동 방향 설정
//            Vector3 dir = (player.position - transform.position).normalized;

//            // 캐릭터 컨트롤러를 이용해 이동하기
//            cc.Move(dir * moveSpeed * Time.deltaTime);

//            // 플레이어와의 거리가 공격범위 안이라면 현재 상태를 공격으로 전환한다.
//            if(Vector3.Distance(transform.position, player.position) < attackDistance)
//            {
//                print("상태 전환 : Move -> Attack");
//                m_state = BossState.Attack;
//            }


//        }
//        //그렇지 않다면 현재 상태를 공격으로 전환한다.
//        else
//        {
//            m_state = BossState.Attack;
//            print("상태 전환 : Move -> Attack");

//            // 누적 시간을 공격 딜레이 시간 만큼 미리 진행시켜 놓는다.
//            currentTime = attackDelay;
//        }
//    }
//    void Attack()
//    {
//        //만일 플레이어가 공격 범위 이내에 있다면 플레이어를 공격한다.
//        if (Vector3.Distance(transform.position, player.position) < attackDistance)
//        {
//            // 일정 시간마다 플레이어를 공격한다.
//            currentTime += Time.deltaTime;
//            if (currentTime > attackDelay)
//            {
//                player.GetComponent<CharacterStats>().TakeDamage(attackPower);
//                currentTime = 0;
//            }
//        }
//        //그렇지 않다면 현재 상태를 이동으로 전환한다(추격)
//        else
//        {
//            m_state = BossState.Move;
//            print("상태 전환 : Attack -> Move");
//            currentTime = 0;
//        }
//    }


//    private void OnTriggerStay(Collider other)
//    {
        
//        if (other.gameObject.CompareTag("Player"))
//        {
//            Attack();  // 플레이어와 충돌 시 Attack 메서드 호출
//        }
//    }
//    // 죽음 상태 함수
//    void Die()
//    {
//        // 진행 중인 피격 코루틴을 중지
//        StopAllCoroutines();

//        // 죽음 상태를 처리하기 위한 코루틴
//        StartCoroutine(DieProcess());
//    }

//    IEnumerator DieProcess()
//    {
//        // 캐릭터 컨트롤러 컴포넌트를 비활성화시킨다
//        cc.enabled = false;

//        // 2초 동안 기다린 후에 자기 자신을 제거한다
//        yield return new WaitForSeconds(2f);
//        print("소멸");
//        Destroy(gameObject);

//    }
//    //Vector2 newPos = Random.insideUnitCircle * initPreferences.patrolRadius;
//    // patrolNext = patrolCenter + new Vector3(newPos.x, 0, newPos.y);
//    // myState = EnemyState.Idle;
//    // idleTime = Random.Range(2.0f, 3.0f);

//}

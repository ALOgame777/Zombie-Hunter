// 좀비 중력 추가? + ++ 이동 방향으로 회전 추가
using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemyFSM : MonoBehaviour
{
    // 에너미 상태 상수
    enum EnemyState
    {
        Idle,
        Move,
        Attack,
        Return,
        Damaged,
        Die
    }
    // 에너미 상태 변수
    EnemyState m_state;

    // 플레이어 발견 범위
    public float findDistance = 8f;

    // 플레이어 트랜스폼
    Transform player;

    // 공격 가능 범위
    public float attackDistance = 2f;

    // 이동 속도
    public float moveSpeed = 2f;

    // 캐릭터 컨트롤러 컴포넌트
    CharacterController cc;

    // 누적 시간
    float currentTime = 0;

    // 공격 딜레이 시간
    float attackDelay = 2f;

    // 에너미의 공격력
    public int attackPower = 5;

    // 초기 위치 저장용 변수
    Vector3 orginPos;

    // 이동 가능 범위
    public float moveDistance = 50f;

    // 에너미의 체력
    public int hp = 200;
    private int maxHp = 10000;

    // 캐릭터 스테이터스 스크립트  불러오기
    private CharacterStats playerStats;

    // 중력 적용을 위한 변수
    private Vector3 velocity;
    public float gravity = -9.81f;

    public float patrolSpeed = 4;

    void Start()
    {
        // 최초의 에너미  상태는 대기(Idle)로 한다.
        m_state = EnemyState.Idle;

        // 플레이어의 트랜스폼 컴포넌트 받아오기
        player = GameObject.Find("PlayerCapsule").transform;

        // 캐릭터 컨트롤러 컴포넌트 받아오기
        cc = GetComponent<CharacterController>();

        // 자신의 초기 위치 저장하기
        orginPos = transform.position;

        playerStats = CharacterStats.cs;
    }

    // Update is called once per frame
    void Update()
    {
        // 현재 상태를 체크해 해당 상태별로 정해진 기능을 수행하게 하고싶다.
        switch (m_state)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Move:
                Move();
                break;
            case EnemyState.Attack:
                Attack();
                break;
            case EnemyState.Return:
                Return();
                break;
            case EnemyState.Damaged:
                //Damaged();
                break;
            case EnemyState.Die:
                //Die();
                break;
        }

    }

    // 데미지 실행 함수
    public void HitEnemy(int hitPower)
    {
        // 만일 이미 피격 상태이거나 사망 상태 또는 복귀 상태라면 아무런 처리도 하지 않고 함수를 종료한다.
        if (m_state == EnemyState.Damaged || m_state == EnemyState.Die || m_state == EnemyState.Return)
        {
            return;
        }

        // 플레이어의 공격력 만큼 에너미 체력을 감소
        hp -= hitPower;
        // 에너미의 체력이 0보다 크면 피격 상태로 전환
        if (hp > 0)
        {
            m_state = EnemyState.Damaged;
            print("상태 전환 : any state -> damaged");
            Damaged();
        }
        // 그렇지 않다면 죽음 상태로 전환
        else
        {
            m_state = EnemyState.Die;
            print("상태 전환 : any state -> die");
            Die();
        }
    }
    //// 상대방이 나에게 데미지를 부여하는 함수
    //public void TakeDamage(float atkPower, Vector3 hitDir, Transform attacker)
    //{
       

    //    // 1. 현재 체력에 상대의 공격력만큼을 감소시킨다. (min 0 ~ max 100)
    //    hp = Mathf.Clamp(hp - hitPower, 0, maxHP);

    //    // 2. 만일 그 결과 현재 체력이 0 이하라면
    //    if (currentHP <= 0)
    //    {
    //        // 2-1. 나의 상태를 죽음 상태로 전환
    //        myState = EnemyState.Dead;
    //        print("My state : any -> dead");
    //        currentTime = 0;

    //        // 2-2. 콜라이더 컴포넌트 비활성화
    //        GetComponent<CapsuleCollider>().enabled = false;
    //        GetComponent<CharacterController>().enabled = false;

    //    }
    //    // 3. 그렇지 않다면
    //    else
    //    {
    //        // 3-1. 나의 상태를 데미지 상태로 전환
    //        myState = EnemyState.Damaged;
    //        print("My state : any -> damaged");
    //        // 3-2. 타격 방향으로 일정 거리만큼을 넉백 위치로 지정한다.
    //        hitDirection = transform.position + hitDir * 1.5f;
    //        // 3-3. 공격자를 타겟으로 설정
    //        target = attacker;
    //        //hitDirection = transform.position + hitDir * 
    //    }
    //}

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
        m_state = EnemyState.Move;
        print("상태 전환 : damaged -> move");
    }
   


    private void Return()
    {
        // 만일 초기 위치에서의 거리가 0.1f 이상이면 초기 위치 쪽으로 이동
        if (Vector3.Distance(transform.position, orginPos) > 0.1f)
        {
            Vector3 dir = (orginPos - transform.position).normalized;
            cc.Move(dir * moveSpeed * Time.deltaTime);
        }
        // 그렇지 않다면 자신의 위치를 초기 위치로 조정하고 현재 상태를 대기로 전환
        else
        {
            transform.position = orginPos;
            // hp를 다시 회복
            hp = maxHp;
            m_state = EnemyState.Idle;
            print("상태 전환 : return -> Idle");
        }

    }

    void Idle()
    {
        // 만일, 플레이어와의 거리가 액션 시작 범위 이내라면 Move 상태로 전환한다.
        if (Vector3.Distance(transform.position, player.position) < findDistance)
        {
            m_state = EnemyState.Move;
            print("상태 전환 : Idle -> Move");
        }   
    }

    void Move()
    {
        // 만일 현재 위치가 초기 위치에서 이동 가능 범위를 넘어간다면
        if (Vector3.Distance(transform.position, orginPos) > moveDistance)
        {
            // 현재 상태를 복귀(return)로 전환
            m_state = EnemyState.Return;
            print("상태 전환 : move -> return");

        }

        // 만일 플레이어와의 거리가 공격 범위 밖이라면 플레이어를 향해 이동한다.
        else if (Vector3.Distance(transform.position, player.position) > attackDistance)
        {
            // 이동 방향 설정
            Vector3 dir = (player.position - transform.position).normalized;

            // y 축 이동 제거
            dir.y = 0;

            // 캐릭터 컨트롤러를 이용해 이동하기
            //cc.Move(dir * moveSpeed * Time.deltaTime);

            if (dir.magnitude > 0.1f)
            {
                // 캐릭터 컨트롤러를 이용해 이동하기
                cc.Move(dir * moveSpeed * Time.deltaTime);

                // 이동하려는 방향으로 회전한다.
                transform.rotation = Quaternion.LookRotation(dir.normalized);
            }

            // 중력 적용
            if (!cc.isGrounded)
            {
                velocity.y += gravity * Time.deltaTime;
                cc.Move(velocity * Time.deltaTime);
            }
            else
            {
                velocity.y = 0;
            }

            // 캐릭터 컨트롤러를 이용해 이동하기
            cc.Move(dir * moveSpeed * Time.deltaTime);
        }
        //그렇지 않다면 현재 상태를 공격으로 전환한다.
        else
        {
            m_state = EnemyState.Attack;
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
            PlayerInvincibility playerInvincibility = player.GetComponent<PlayerInvincibility>();
            if (playerInvincibility != null && playerInvincibility.IsInvincible())
            {
                // Player is invincible, do not attack
                print("플레이어가 무적 상태입니다. 공격을 무시합니다.");
                return;
            }
            // 일정 시간마다 플레이어를 공격한다.
            currentTime += Time.deltaTime;
            if (currentTime > attackDelay)
            {
                player.GetComponent<CharacterStats>().TakeDamage(attackPower);
                print("공격");
                currentTime = 0;
            }

        }
        //그렇지 않다면 현재 상태를 이동으로 전환한다(추격)
        else
        {
            m_state = EnemyState.Move;
            print("상태 전환 : Attack -> Move");
            currentTime = 0;
        }
    }
    private void OnCollisionStay(Collision collision)
{
    if (collision.gameObject.CompareTag("Player"))
    {
        Attack();  // 플레이어와 충돌 시 Attack 메서드 호출
    }
}
    // 죽음 상태 함수
    void Die()
    {
        // 진행 중인 피격 코루틴을 중지
        StopAllCoroutines();
        Debug.Log("적 사망 전 점수: " + ScoreManager.Instance.GetScore());
        ScoreManager.Instance.AddScore(10000);
        Debug.Log("적 사망 후 점수: " + ScoreManager.Instance.GetScore());
        // 죽음 상태를 처리하기 위한 코루틴
        StartCoroutine(DieProcess());
    }

    IEnumerator DieProcess()
    {
        // 캐릭터 컨트롤러 컴포넌트를 비활성화시킨다
        cc.enabled = false;

        // 2초 동안 기다린 후에 자기 자신을 제거한다
        yield return new WaitForSeconds(1f);
        print("소멸");
        Destroy(gameObject);
    }



}

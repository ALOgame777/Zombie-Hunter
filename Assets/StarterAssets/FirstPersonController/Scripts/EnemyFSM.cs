using StarterAssets;
using System.Collections;
using System.Collections.Generic;
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
    public float moveSpeed = 5f;

    // 캐릭터 컨트롤러 컴포넌트
    CharacterController cc;

    // 누적 시간
    float currentTime = 0;

    // 공격 딜레이 시간
    float attackDelay = 2f;

    // 에너미의 공격력
    public int attackPower = 5;

    void Start()
    {
        // 최초의 에너미  상태는 대기(Idle)로 한다.
        m_state = EnemyState.Idle;

        // 플레이어의 트랜스폼 컴포넌트 받아오기
        player = GameObject.Find("PlayerCapsule").transform;

        // 캐릭터 컨트롤러 컴포넌트 받아오기
        cc = GetComponent<CharacterController>();
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
                //Return();
                break;
            case EnemyState.Damaged:
                //Damaged();
                break;
            case EnemyState.Die:
                //Die();
                break;
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
        // 만일 플레이어와의 거리가 공격 범위 밖이라면 플레이어를 향해 이동한다.
        if (Vector3.Distance(transform.position, player.position) > attackDistance)
        {
            // 이동 방향 설정
            Vector3 dir = (player.position - transform.position).normalized;

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
            // 일정 시간마다 플레이어를 공격한다.
            currentTime += Time.deltaTime;
            if (currentTime > attackDelay)
            {
                player.GetComponent<FirstPersonController>();
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


}

// 널뛰기 버그 수정중...
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

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

    // 에너미의 체력
    public int hp = 50000;

    // 실행할 엔딩비디오 오브젝트
    public GameObject Endingvideo;

    // 실행할 버튼 오브젝트
    public GameObject GoStartButton;

    // 비디오 플레이어
    private VideoPlayer vid;
    private bool isVideoPlaying = false;

    // 이동 타이머
    private float moveTimer = 0f;
    public float moveInterval = 10f; // 10초마다 이동

    // 빠르게 이동할 시간
    public float fastMoveDuration = 2f;

    // 빠르게 이동할 때의 속도
    public float fastMoveSpeed = 10f;

    // 중력 적용을 위한 변수
    private Vector3 velocity;
    public float gravity = -9.81f;

    // 독극물 관련 변수
    public float poisonInterval = 16f; // 16초마다 독극물 뿌리기
    private float poisonTimer = 0f;
    public GameObject poisonAreaPrefab; // 독극물 영역 프리팹
    public float poisonRadius = 5f; // 독극물 범위
    public float poisonDuration = 10f; // 독극물 지속 시간

    // 검은색 화면 UI
    public GameObject blackScreenUI;


    void Start()
    {
        // 최초의 에너미 상태는 대기(Idle)로 한다.
        m_state = BossState.Idle;

        // 플레이어의 트랜스폼 컴포넌트 받아오기
        player = GameObject.Find("PlayerCapsule").transform;

        // 캐릭터 컨트롤러 컴포넌트 받아오기
        cc = GetComponent<CharacterController>();
        vid = Endingvideo.GetComponent<VideoPlayer>();
        vid.loopPointReached += OnVideoEnd;
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

        // 이동 타이머 갱신
        moveTimer += Time.deltaTime;
        if (moveTimer >= moveInterval)
        {
            // 5초마다 플레이어의 옆으로 이동
            StartCoroutine(FastMove());
            moveTimer = 0f; // 타이머 초기화
        }

        // 독극물 타이머 갱신
        poisonTimer += Time.deltaTime;
        if (poisonTimer >= poisonInterval)
        {
            // 16초마다 독극물 뿌리기
            StartCoroutine(ReleasePoison());
            poisonTimer = 0f; // 타이머 초기화
        }

        // 'E' 키를 누르면 씬을 로드
        if (GoStartButton.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene(0);
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
        ScoreManager.Instance.AddScore(10000);
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

            // y 축 이동 제거
            dir.y = 0;

            // 캐릭터 컨트롤러를 이용해 이동하기
            cc.Move(dir * moveSpeed * Time.deltaTime);

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


            // 플레이어와의 거리가 공격범위 안이라면 현재 상태를 공격으로 전환한다.
            if (Vector3.Distance(transform.position, player.position) < attackDistance)
            {
                print("상태 전환 : Move -> Attack");
                m_state = BossState.Attack;
            }
        }
        // 그렇지 않다면 현재 상태를 공격으로 전환한다.
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
        // 만일 플레이어가 공격 범위 이내에 있다면 플레이어를 공격한다.
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
        // 그렇지 않다면 현재 상태를 이동으로 전환한다(추격)
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

        // 5초 동안 기다린 후에 자기 자신을 제거한다
        yield return new WaitForSeconds(0f); // 애니메이션 넣으면 5초로 변경
        print("소멸");
        Destroy(gameObject);

        // 엔딩 비디오 재생
        Endingvideo.SetActive(true);
        vid.Play(); // 비디오 재생
        isVideoPlaying = true;
        Time.timeScale = 0; // 시간 정지
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        // 비디오가 끝났을 때 호출되는 함수
        isVideoPlaying = false;
        GoStartButton.SetActive(true);
        Time.timeScale = 1; // 시간 다시 진행
    }

    //// 2초 동안 빠르게 이동하는 코루틴
    //IEnumerator FastMove()
    //{
    //    float originalSpeed = moveSpeed;
    //    moveSpeed = fastMoveSpeed;

    //    yield return new WaitForSeconds(fastMoveDuration);

    //    moveSpeed = originalSpeed;
    //}
    // 2초 동안 빠르게 이동하는 코루틴
    IEnumerator FastMove()
    {
        // 플레이어 위치로 순간이동
        Vector3 targetPosition = player.position + Random.onUnitSphere * 5f;
        targetPosition.y = transform.position.y; // Y축 높이는 현재 높이로 유지

        float elapsedTime = 0f;
        while (elapsedTime < fastMoveDuration)
        {
            Vector3 dir = (targetPosition - transform.position).normalized;
            dir.y = 0; // Y축 이동 제거

            cc.Move(dir * fastMoveSpeed * Time.deltaTime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    IEnumerator ReleasePoison()
    {
        print("독극물 뿌리기 시작");
        // 독극물 영역 생성
        GameObject poisonArea = Instantiate(poisonAreaPrefab, transform.position, Quaternion.identity);
        PoisonArea poisonScript = poisonArea.GetComponent<PoisonArea>();
        poisonScript.Initialize(poisonRadius, poisonDuration, this);

        yield return new WaitForSeconds(poisonDuration);

        Destroy(poisonArea);
        print("독극물 뿌리기 종료");
    }

    public void ApplyPoisonEffect()
    {
        StartCoroutine(PoisonEffectCoroutine());
    }

    IEnumerator PoisonEffectCoroutine()
    {
        // 검은색 화면 UI 활성화
        blackScreenUI.SetActive(true);

        yield return new WaitForSeconds(3f);

        // 검은색 화면 UI 비활성화
        blackScreenUI.SetActive(false);
    }
}


//// 널뛰기 버그 수정중... 성공...?
//using System.Collections;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.Video;

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

//    // 에너미의 체력
//    public int hp = 50000;

//    // 실행할 엔딩비디오 오브젝트
//    public GameObject Endingvideo;

//    // 실행할 버튼 오브젝트
//    public GameObject GoStartButton;

//    // 비디오 플레이어
//    private VideoPlayer vid;
//    private bool isVideoPlaying = false;

//    // 이동 타이머
//    private float moveTimer = 0f;
//    public float moveInterval = 10f; // 10초마다 이동

//    // 빠르게 이동할 시간
//    public float fastMoveDuration = 2f;

//    // 빠르게 이동할 때의 속도
//    public float fastMoveSpeed = 10f;

//    // 중력 적용을 위한 변수
//    private Vector3 velocity;
//    public float gravity = -9.81f;

//    void Start()
//    {
//        // 최초의 에너미 상태는 대기(Idle)로 한다.
//        m_state = BossState.Idle;

//        // 플레이어의 트랜스폼 컴포넌트 받아오기
//        player = GameObject.Find("PlayerCapsule").transform;

//        // 캐릭터 컨트롤러 컴포넌트 받아오기
//        cc = GetComponent<CharacterController>();
//        vid = Endingvideo.GetComponent<VideoPlayer>();
//        vid.loopPointReached += OnVideoEnd;
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

//        // 이동 타이머 갱신
//        moveTimer += Time.deltaTime;
//        if (moveTimer >= moveInterval)
//        {
//            // 5초마다 플레이어의 옆으로 이동
//            StartCoroutine(FastMove());
//            moveTimer = 0f; // 타이머 초기화
//        }

//        // 'E' 키를 누르면 씬을 로드
//        if (GoStartButton.activeSelf && Input.GetKeyDown(KeyCode.E))
//        {
//            SceneManager.LoadScene(0);
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

//            // y 축 이동 제거
//            dir.y = 0;

//            // 캐릭터 컨트롤러를 이용해 이동하기
//            cc.Move(dir * moveSpeed * Time.deltaTime);

//            // 중력 적용
//            if (!cc.isGrounded)
//            {
//                velocity.y += gravity * Time.deltaTime;
//                cc.Move(velocity * Time.deltaTime);
//            }
//            else
//            {
//                velocity.y = 0;
//            }


//            // 플레이어와의 거리가 공격범위 안이라면 현재 상태를 공격으로 전환한다.
//            if (Vector3.Distance(transform.position, player.position) < attackDistance)
//            {
//                print("상태 전환 : Move -> Attack");
//                m_state = BossState.Attack;
//            }
//        }
//        // 그렇지 않다면 현재 상태를 공격으로 전환한다.
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
//        // 만일 플레이어가 공격 범위 이내에 있다면 플레이어를 공격한다.
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
//        // 그렇지 않다면 현재 상태를 이동으로 전환한다(추격)
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

//        // 5초 동안 기다린 후에 자기 자신을 제거한다
//        yield return new WaitForSeconds(5f);
//        print("소멸");
//        Destroy(gameObject);

//        // 엔딩 비디오 재생
//        Endingvideo.SetActive(true);
//        vid.Play(); // 비디오 재생
//        isVideoPlaying = true;
//        Time.timeScale = 0; // 시간 정지
//    }

//    void OnVideoEnd(VideoPlayer vp)
//    {
//        // 비디오가 끝났을 때 호출되는 함수
//        isVideoPlaying = false;
//        GoStartButton.SetActive(true);
//        Time.timeScale = 1; // 시간 다시 진행
//    }

//    // 2초 동안 빠르게 이동하는 코루틴
//    IEnumerator FastMove()
//    {
//        float originalSpeed = moveSpeed;
//        moveSpeed = fastMoveSpeed;

//        yield return new WaitForSeconds(fastMoveDuration);

//        moveSpeed = originalSpeed;
//    }
//}


// 빠르게 이동 성공~~~~ 근데 보스가 자꾸 널뛰기를 하는데...
//using System.Collections;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.Video;

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

//    // 에너미의 체력
//    public int hp = 50000;

//    // 실행할 엔딩비디오 오브젝트
//    public GameObject Endingvideo;

//    // 실행할 버튼 오브젝트
//    public GameObject GoStartButton;

//    // 비디오 플레이어
//    private VideoPlayer vid;
//    private bool isVideoPlaying = false;

//    // 이동 타이머
//    private float moveTimer = 0f;
//    public float moveInterval = 10f; // 10초마다 이동

//    // 빠르게 이동할 시간
//    public float fastMoveDuration = 2f;

//    // 빠르게 이동할 때의 속도
//    public float fastMoveSpeed = 10f;

//    void Start()
//    {
//        // 최초의 에너미 상태는 대기(Idle)로 한다.
//        m_state = BossState.Idle;

//        // 플레이어의 트랜스폼 컴포넌트 받아오기
//        player = GameObject.Find("PlayerCapsule").transform;

//        // 캐릭터 컨트롤러 컴포넌트 받아오기
//        cc = GetComponent<CharacterController>();
//        vid = Endingvideo.GetComponent<VideoPlayer>();
//        vid.loopPointReached += OnVideoEnd;
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

//        // 이동 타이머 갱신
//        moveTimer += Time.deltaTime;
//        if (moveTimer >= moveInterval)
//        {
//            // 5초마다 플레이어의 옆으로 이동
//            StartCoroutine(FastMove());
//            moveTimer = 0f; // 타이머 초기화
//        }

//        // 'E' 키를 누르면 씬을 로드
//        if (GoStartButton.activeSelf && Input.GetKeyDown(KeyCode.E))
//        {
//            SceneManager.LoadScene(0);
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
//            if (Vector3.Distance(transform.position, player.position) < attackDistance)
//            {
//                print("상태 전환 : Move -> Attack");
//                m_state = BossState.Attack;
//            }
//        }
//        // 그렇지 않다면 현재 상태를 공격으로 전환한다.
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
//        // 만일 플레이어가 공격 범위 이내에 있다면 플레이어를 공격한다.
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
//        // 그렇지 않다면 현재 상태를 이동으로 전환한다(추격)
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

//        // 5초 동안 기다린 후에 자기 자신을 제거한다
//        yield return new WaitForSeconds(5f);
//        print("소멸");
//        Destroy(gameObject);

//        // 엔딩 비디오 재생
//        Endingvideo.SetActive(true);
//        vid.Play(); // 비디오 재생
//        isVideoPlaying = true;
//        Time.timeScale = 0; // 시간 정지
//    }

//    void OnVideoEnd(VideoPlayer vp)
//    {
//        // 비디오가 끝났을 때 호출되는 함수
//        isVideoPlaying = false;
//        GoStartButton.SetActive(true);
//        Time.timeScale = 1; // 시간 다시 진행
//    }

//    // 2초 동안 빠르게 이동하는 코루틴
//    IEnumerator FastMove()
//    {
//        float originalSpeed = moveSpeed;
//        moveSpeed = fastMoveSpeed;

//        yield return new WaitForSeconds(fastMoveDuration);

//        moveSpeed = originalSpeed;
//    }
//}


//// 보스의 빠른 이동 공격 구현... 을 하려고 했는데 그냥 순간이동을 해버리는데 ㅋㅋㅋㅋㅋ
//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.Video;

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

//    // 실행할 엔딩비디오 오브젝트
//    public GameObject Endingvideo;

//    // 실행할 버튼 오브젝트
//    public GameObject GoStartButton;

//    // 비디오 플레이어
//    private VideoPlayer vid;
//    private bool isVideoPlaying = false;

//    // 이동 타이머
//    private float moveTimer = 0f;
//    public float moveInterval = 5f; // 5초마다 이동

//    void Start()
//    {
//        // 최초의 에너미  상태는 대기(Idle)로 한다.
//        m_state = BossState.Idle;

//        // 플레이어의 트랜스폼 컴포넌트 받아오기
//        player = GameObject.Find("PlayerCapsule").transform;

//        // 캐릭터 컨트롤러 컴포넌트 받아오기
//        cc = GetComponent<CharacterController>();
//        vid = Endingvideo.GetComponent<VideoPlayer>();
//        vid.loopPointReached += OnVideoEnd;
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

//        // 이동 타이머 갱신
//        moveTimer += Time.deltaTime;
//        if (moveTimer >= moveInterval)
//        {
//            // 5초마다 플레이어의 옆으로 이동
//            TeleportNextToPlayer();
//            moveTimer = 0f; // 타이머 초기화
//        }

//        // 'E' 키를 누르면 씬을 로드
//        if (GoStartButton.activeSelf && Input.GetKeyDown(KeyCode.E))
//        {
//            SceneManager.LoadScene(0);
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
//            if (Vector3.Distance(transform.position, player.position) < attackDistance)
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

//        // 5초 동안 기다린 후에 자기 자신을 제거한다
//        yield return new WaitForSeconds(5f);
//        print("소멸");
//        Destroy(gameObject);

//        // 엔딩 비디오 재생
//        Endingvideo.SetActive(true);
//        vid.Play(); // 비디오 재생
//        isVideoPlaying = true;
//        Time.timeScale = 0; // 시간 정지
//    }

//    void OnVideoEnd(VideoPlayer vp)
//    {
//        // 비디오가 끝났을 때 호출되는 함수
//        isVideoPlaying = false;
//        GoStartButton.SetActive(true);
//        Time.timeScale = 1; // 시간 다시 진행
//    }

//    // 플레이어 옆으로 빠르게 이동하는 함수
//    void TeleportNextToPlayer()
//    {
//        Vector3 offset = new Vector3(5.0f, 0, 5.0f); // 플레이어 옆으로 이동할 오프셋
//        Vector3 newPosition = player.position + offset;
//        transform.position = newPosition;
//    }

//// 동영상 끝나면 버튼 나오고 e 누르면 첫 씬으로 가게끔 도전...... 성공 ㅋㅋ 근데 여기 씬에서 암만 해봐도 안되길래 그냥 새 스크립트 파서 캔버스에 할당하니 바로됨...
//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.Video;

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

//    // 실행할 엔딩비디오 오브젝트
//    public GameObject Endingvideo;

//    // 실행할 버튼 오브젝트
//    public GameObject GoStartButton;

//    // 비디오 플레이어
//    private VideoPlayer vid;
//    private bool isVideoPlaying = false;


//    void Start()
//    {
//        // 최초의 에너미  상태는 대기(Idle)로 한다.
//        m_state = BossState.Idle;

//        // 플레이어의 트랜스폼 컴포넌트 받아오기
//        player = GameObject.Find("PlayerCapsule").transform;

//        // 캐릭터 컨트롤러 컴포넌트 받아오기
//        cc = GetComponent<CharacterController>();
//        vid = Endingvideo.GetComponent<VideoPlayer>();
//        vid.loopPointReached += OnVideoEnd;
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
//        // 'E' 키를 누르면 씬을 로드
//        if (GoStartButton.activeSelf && Input.GetKeyDown(KeyCode.E))
//        {
//            SceneManager.LoadScene(0);
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
//            if (Vector3.Distance(transform.position, player.position) < attackDistance)
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

//        // 5초 동안 기다린 후에 자기 자신을 제거한다
//        yield return new WaitForSeconds(5f);
//        print("소멸");
//        Destroy(gameObject);

//        // 엔딩 비디오 재생
//        Endingvideo.SetActive(true);
//        vid.Play(); // 비디오 재생
//        isVideoPlaying = true;
//        Time.timeScale = 0; // 시간 정지
//    }

//    void OnVideoEnd(VideoPlayer vp)
//    {
//        // 비디오가 끝났을 때 호출되는 함수
//        isVideoPlaying = false;
//        GoStartButton.SetActive(true);
//        Time.timeScale = 1; // 시간 다시 진행
//    }


//// 동영상 끝나면 버튼 나오고 e 누르면 첫 씬으로 가게끔 도전...... 성공 ㅋㅋ 근데 여기 씬에서 암만 해봐도 안되길래 그냥 새 스크립트 파서 캔버스에 할당하니 바로됨...
//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.Video;

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

//    // 실행할 엔딩비디오 오브젝트
//    public GameObject Endingvideo;

//    // 실행할 버튼 오브젝트
//    public GameObject GoStartButton;

//    // 비디오 플레이어
//    private VideoPlayer vid;
//    private bool isVideoPlaying = false;


//    void Start()
//    {
//        // 최초의 에너미  상태는 대기(Idle)로 한다.
//        m_state = BossState.Idle;

//        // 플레이어의 트랜스폼 컴포넌트 받아오기
//        player = GameObject.Find("PlayerCapsule").transform;

//        // 캐릭터 컨트롤러 컴포넌트 받아오기
//        cc = GetComponent<CharacterController>();
//        vid = Endingvideo.GetComponent<VideoPlayer>();
//        vid.loopPointReached += OnVideoEnd;


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
//        // 'E' 키를 누르면 씬을 로드
//        if (GoStartButton.activeSelf && Input.GetKeyDown(KeyCode.E))
//        {
//            SceneManager.LoadScene(0);
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
//            if (Vector3.Distance(transform.position, player.position) < attackDistance)
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

//        // 5초 동안 기다린 후에 자기 자신을 제거한다
//        yield return new WaitForSeconds(5f);
//        print("소멸");
//        Destroy(gameObject);

//        // 엔딩 비디오 재생
//        Endingvideo.SetActive(true);
//        vid.Play(); // 비디오 재생
//        isVideoPlaying = true;
//        Time.timeScale = 0; // 시간 정지
//    }

//    void OnVideoEnd(VideoPlayer vp)
//    {
//        // 비디오가 끝났을 때 호출되는 함수
//        isVideoPlaying = false;
//        GoStartButton.SetActive(true);
//        Time.timeScale = 1; // 시간 다시 진행
//    }

//// 이거 보스 죽이고 동영상 테스트  ... 보스 죽고 동영상 까지는 잘 됨.
//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;
//using UnityEngine.SceneManagement;

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

//    // 실행할 엔딩비디오 오브젝트
//    public GameObject Endingvideo;

//    // 실행할 버튼 오브젝트
//    public GameObject GoStartButton;





//    void Start()
//    {
//        // 최초의 에너미  상태는 대기(Idle)로 한다.
//        m_state = BossState.Idle;

//        // 플레이어의 트랜스폼 컴포넌트 받아오기
//        player = GameObject.Find("PlayerCapsule").transform;

//        // 캐릭터 컨트롤러 컴포넌트 받아오기
//        cc = GetComponent<CharacterController>();


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
//            if (Vector3.Distance(transform.position, player.position) < attackDistance)
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

//        // 5초 동안 기다린 후에 자기 자신을 제거한다
//        yield return new WaitForSeconds(5f);
//        print("소멸");
//        Destroy(gameObject);

//        // 엔딩 비디오 재생
//        Endingvideo.SetActive(true);

//        // 시간 정지
//        Time.timeScale = 1;

//        // 시간 정지 후에 게임 종료 UI까지
//        if (Time.timeScale == 1)
//        {
//            yield return new WaitForSeconds(8f);
//            GoStartButton.SetActive(true);

//            if (Input.GetKeyDown(KeyCode.E))
//            {
//                SceneManager.LoadScene(0);
//            }

//        }


//    }

//Vector2 newPos = Random.insideUnitCircle * initPreferences.patrolRadius;
// patrolNext = patrolCenter + new Vector3(newPos.x, 0, newPos.y);
// myState = EnemyState.Idle;
// idleTime = Random.Range(2.0f, 3.0f);
//}



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

// ������ ������ ��ư ������ e ������ ù ������ ���Բ� ����...... ���� ���� �ٵ� ���� ������ �ϸ� �غ��� �ȵǱ淡 �׳� �� ��ũ��Ʈ �ļ� ĵ������ �Ҵ��ϴ� �ٷε�...
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class BOSS : MonoBehaviour
{

    // ���ʹ� ���� ���
    public enum BossState
    {
        Idle,
        Move,
        Attack,
        Return,
        Damaged,
        Die
    }
    // ���ʹ� ���� ����
    public BossState m_state;

    // �÷��̾� �߰� ����
    public float findDistance = 80f;

    // �÷��̾� Ʈ������
    public Transform player;

    // ���� ���� ����
    public float attackDistance = 200f;

    // �̵� �ӵ�
    public float moveSpeed = 2f;

    // ĳ���� ��Ʈ�ѷ� ������Ʈ
    CharacterController cc;

    // ���� �ð�
    float currentTime = 0;

    // ���� ������ �ð�
    public float attackDelay = 2f;

    // ���ʹ��� ���ݷ�
    public int attackPower = 30;

    // �̵� ���� ����
    public float moveDistance = 200f;

    // ���ʹ��� ü��
    public int hp = 50000;

    // ������ �������� ������Ʈ
    public GameObject Endingvideo;

    // ������ ��ư ������Ʈ
    public GameObject GoStartButton;

    // ���� �÷��̾�
    private VideoPlayer vid;
    private bool isVideoPlaying = false;


    void Start()
    {
        // ������ ���ʹ�  ���´� ���(Idle)�� �Ѵ�.
        m_state = BossState.Idle;

        // �÷��̾��� Ʈ������ ������Ʈ �޾ƿ���
        player = GameObject.Find("PlayerCapsule").transform;

        // ĳ���� ��Ʈ�ѷ� ������Ʈ �޾ƿ���
        cc = GetComponent<CharacterController>();
        vid = Endingvideo.GetComponent<VideoPlayer>();
        vid.loopPointReached += OnVideoEnd;


    }

    void Update()
    {
        // ���� ���¸� üũ�� �ش� ���º��� ������ ����� �����ϰ� �ϰ�ʹ�.
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
        // 'E' Ű�� ������ ���� �ε�
        if (GoStartButton.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene(0);
        }

    }

    // ������ ���� �Լ�
    public void HitEnemy(int hitPower)
    {
        // ���� �̹� �ǰ� �����̰ų� ��� ���� �Ǵ� ���� ���¶�� �ƹ��� ó���� ���� �ʰ� �Լ��� �����Ѵ�.
        if (m_state == BossState.Damaged || m_state == BossState.Die || m_state == BossState.Return)
        {
            return;
        }

        // �÷��̾��� ���ݷ� ��ŭ ���ʹ� ü���� ����
        hp -= hitPower;
        // ���ʹ��� ü���� 0���� ũ�� �ǰ� ���·� ��ȯ
        if (hp > 0)
        {
            m_state = BossState.Damaged;
            print("���� ��ȯ : any state -> damaged");
            Damaged();
        }
        // �׷��� �ʴٸ� ���� ���·� ��ȯ
        else
        {
            m_state = BossState.Die;
            print("���� ��ȯ : any state -> die");
            Die();
        }
    }

    private void Damaged()
    {
        // �ǰ� ���¸� ó���ϱ� ���� �ڷ�ƾ ����
        StartCoroutine(DamageProcess());
    }
    // ������ ó���� �ڷ�ƾ �Լ�
    IEnumerator DamageProcess()
    {
        // �ǰ� ��� �ð���ŭ ��ٸ���.
        yield return new WaitForSeconds(0.5f);

        // ���� ���¸� �̵� ���·� ��ȯ
        m_state = BossState.Move;
        print("���� ��ȯ : damaged -> move");
    }

    void Idle()
    {
        // ����, �÷��̾���� �Ÿ��� �׼� ���� ���� �̳���� Move ���·� ��ȯ�Ѵ�.
        if (Vector3.Distance(transform.position, player.position) < findDistance)
        {
            m_state = BossState.Move;
            print("���� ��ȯ : Idle -> Move");
        }
    }

    void Move()
    {
        print(player.position);
        // ���� �÷��̾���� �Ÿ��� ���� ���� ���̶�� �÷��̾ ���� �̵��Ѵ�.
        if (Vector3.Distance(transform.position, player.position) > attackDistance)
        {
            // �̵� ���� ����
            Vector3 dir = (player.position - transform.position).normalized;

            // ĳ���� ��Ʈ�ѷ��� �̿��� �̵��ϱ�
            cc.Move(dir * moveSpeed * Time.deltaTime);

            // �÷��̾���� �Ÿ��� ���ݹ��� ���̶�� ���� ���¸� �������� ��ȯ�Ѵ�.
            if (Vector3.Distance(transform.position, player.position) < attackDistance)
            {
                print("���� ��ȯ : Move -> Attack");
                m_state = BossState.Attack;
            }


        }
        //�׷��� �ʴٸ� ���� ���¸� �������� ��ȯ�Ѵ�.
        else
        {
            m_state = BossState.Attack;
            print("���� ��ȯ : Move -> Attack");

            // ���� �ð��� ���� ������ �ð� ��ŭ �̸� ������� ���´�.
            currentTime = attackDelay;
        }
    }
    void Attack()
    {
        //���� �÷��̾ ���� ���� �̳��� �ִٸ� �÷��̾ �����Ѵ�.
        if (Vector3.Distance(transform.position, player.position) < attackDistance)
        {
            // ���� �ð����� �÷��̾ �����Ѵ�.
            currentTime += Time.deltaTime;
            if (currentTime > attackDelay)
            {
                player.GetComponent<CharacterStats>().TakeDamage(attackPower);
                currentTime = 0;
            }
        }
        //�׷��� �ʴٸ� ���� ���¸� �̵����� ��ȯ�Ѵ�(�߰�)
        else
        {
            m_state = BossState.Move;
            print("���� ��ȯ : Attack -> Move");
            currentTime = 0;
        }
    }


    private void OnTriggerStay(Collider other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            Attack();  // �÷��̾�� �浹 �� Attack �޼��� ȣ��
        }
    }
    // ���� ���� �Լ�
    void Die()
    {
        // ���� ���� �ǰ� �ڷ�ƾ�� ����
        StopAllCoroutines();

        // ���� ���¸� ó���ϱ� ���� �ڷ�ƾ
        StartCoroutine(DieProcess());
    }

    IEnumerator DieProcess()
    {
        // ĳ���� ��Ʈ�ѷ� ������Ʈ�� ��Ȱ��ȭ��Ų��
        cc.enabled = false;

        // 5�� ���� ��ٸ� �Ŀ� �ڱ� �ڽ��� �����Ѵ�
        yield return new WaitForSeconds(5f);
        print("�Ҹ�");
        Destroy(gameObject);

        // ���� ���� ���
        Endingvideo.SetActive(true);
        vid.Play(); // ���� ���
        isVideoPlaying = true;
        Time.timeScale = 0; // �ð� ����
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        // ������ ������ �� ȣ��Ǵ� �Լ�
        isVideoPlaying = false;
        GoStartButton.SetActive(true);
        Time.timeScale = 1; // �ð� �ٽ� ����
    }


    //// ������ ������ ��ư ������ e ������ ù ������ ���Բ� ����...... ���� ���� �ٵ� ���� ������ �ϸ� �غ��� �ȵǱ淡 �׳� �� ��ũ��Ʈ �ļ� ĵ������ �Ҵ��ϴ� �ٷε�...
    //using System.Collections;
    //using System.Collections.Generic;
    //using UnityEditor;
    //using UnityEngine;
    //using UnityEngine.SceneManagement;
    //using UnityEngine.Video;

    //public class BOSS : MonoBehaviour
    //{

    //    // ���ʹ� ���� ���
    //    public enum BossState
    //    {
    //        Idle,
    //        Move,
    //        Attack,
    //        Return,
    //        Damaged,
    //        Die
    //    }
    //    // ���ʹ� ���� ����
    //    public BossState m_state;

    //    // �÷��̾� �߰� ����
    //    public float findDistance = 80f;

    //    // �÷��̾� Ʈ������
    //    public Transform player;

    //    // ���� ���� ����
    //    public float attackDistance = 200f;

    //    // �̵� �ӵ�
    //    public float moveSpeed = 2f;

    //    // ĳ���� ��Ʈ�ѷ� ������Ʈ
    //    CharacterController cc;

    //    // ���� �ð�
    //    float currentTime = 0;

    //    // ���� ������ �ð�
    //    public float attackDelay = 2f;

    //    // ���ʹ��� ���ݷ�
    //    public int attackPower = 30;

    //    // �̵� ���� ����
    //    public float moveDistance = 200f;

    //    // ���ʹ��� ü��
    //    public int hp = 50000;

    //    // ������ �������� ������Ʈ
    //    public GameObject Endingvideo;

    //    // ������ ��ư ������Ʈ
    //    public GameObject GoStartButton;

    //    // ���� �÷��̾�
    //    private VideoPlayer vid;
    //    private bool isVideoPlaying = false;


    //    void Start()
    //    {
    //        // ������ ���ʹ�  ���´� ���(Idle)�� �Ѵ�.
    //        m_state = BossState.Idle;

    //        // �÷��̾��� Ʈ������ ������Ʈ �޾ƿ���
    //        player = GameObject.Find("PlayerCapsule").transform;

    //        // ĳ���� ��Ʈ�ѷ� ������Ʈ �޾ƿ���
    //        cc = GetComponent<CharacterController>();
    //        vid = Endingvideo.GetComponent<VideoPlayer>();
    //        vid.loopPointReached += OnVideoEnd;


    //    }

    //    void Update()
    //    {
    //        // ���� ���¸� üũ�� �ش� ���º��� ������ ����� �����ϰ� �ϰ�ʹ�.
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
    //        // 'E' Ű�� ������ ���� �ε�
    //        if (GoStartButton.activeSelf && Input.GetKeyDown(KeyCode.E))
    //        {
    //            SceneManager.LoadScene(0);
    //        }

    //    }

    //    // ������ ���� �Լ�
    //    public void HitEnemy(int hitPower)
    //    {
    //        // ���� �̹� �ǰ� �����̰ų� ��� ���� �Ǵ� ���� ���¶�� �ƹ��� ó���� ���� �ʰ� �Լ��� �����Ѵ�.
    //        if (m_state == BossState.Damaged || m_state == BossState.Die || m_state == BossState.Return)
    //        {
    //            return;
    //        }

    //        // �÷��̾��� ���ݷ� ��ŭ ���ʹ� ü���� ����
    //        hp -= hitPower;
    //        // ���ʹ��� ü���� 0���� ũ�� �ǰ� ���·� ��ȯ
    //        if (hp > 0)
    //        {
    //            m_state = BossState.Damaged;
    //            print("���� ��ȯ : any state -> damaged");
    //            Damaged();
    //        }
    //        // �׷��� �ʴٸ� ���� ���·� ��ȯ
    //        else
    //        {
    //            m_state = BossState.Die;
    //            print("���� ��ȯ : any state -> die");
    //            Die();
    //        }
    //    }

    //    private void Damaged()
    //    {
    //        // �ǰ� ���¸� ó���ϱ� ���� �ڷ�ƾ ����
    //        StartCoroutine(DamageProcess());
    //    }
    //    // ������ ó���� �ڷ�ƾ �Լ�
    //    IEnumerator DamageProcess()
    //    {
    //        // �ǰ� ��� �ð���ŭ ��ٸ���.
    //        yield return new WaitForSeconds(0.5f);

    //        // ���� ���¸� �̵� ���·� ��ȯ
    //        m_state = BossState.Move;
    //        print("���� ��ȯ : damaged -> move");
    //    }

    //    void Idle()
    //    {
    //        // ����, �÷��̾���� �Ÿ��� �׼� ���� ���� �̳���� Move ���·� ��ȯ�Ѵ�.
    //        if (Vector3.Distance(transform.position, player.position) < findDistance)
    //        {
    //            m_state = BossState.Move;
    //            print("���� ��ȯ : Idle -> Move");
    //        }
    //    }

    //    void Move()
    //    {
    //        print(player.position);
    //        // ���� �÷��̾���� �Ÿ��� ���� ���� ���̶�� �÷��̾ ���� �̵��Ѵ�.
    //        if (Vector3.Distance(transform.position, player.position) > attackDistance)
    //        {
    //            // �̵� ���� ����
    //            Vector3 dir = (player.position - transform.position).normalized;

    //            // ĳ���� ��Ʈ�ѷ��� �̿��� �̵��ϱ�
    //            cc.Move(dir * moveSpeed * Time.deltaTime);

    //            // �÷��̾���� �Ÿ��� ���ݹ��� ���̶�� ���� ���¸� �������� ��ȯ�Ѵ�.
    //            if (Vector3.Distance(transform.position, player.position) < attackDistance)
    //            {
    //                print("���� ��ȯ : Move -> Attack");
    //                m_state = BossState.Attack;
    //            }


    //        }
    //        //�׷��� �ʴٸ� ���� ���¸� �������� ��ȯ�Ѵ�.
    //        else
    //        {
    //            m_state = BossState.Attack;
    //            print("���� ��ȯ : Move -> Attack");

    //            // ���� �ð��� ���� ������ �ð� ��ŭ �̸� ������� ���´�.
    //            currentTime = attackDelay;
    //        }
    //    }
    //    void Attack()
    //    {
    //        //���� �÷��̾ ���� ���� �̳��� �ִٸ� �÷��̾ �����Ѵ�.
    //        if (Vector3.Distance(transform.position, player.position) < attackDistance)
    //        {
    //            // ���� �ð����� �÷��̾ �����Ѵ�.
    //            currentTime += Time.deltaTime;
    //            if (currentTime > attackDelay)
    //            {
    //                player.GetComponent<CharacterStats>().TakeDamage(attackPower);
    //                currentTime = 0;
    //            }
    //        }
    //        //�׷��� �ʴٸ� ���� ���¸� �̵����� ��ȯ�Ѵ�(�߰�)
    //        else
    //        {
    //            m_state = BossState.Move;
    //            print("���� ��ȯ : Attack -> Move");
    //            currentTime = 0;
    //        }
    //    }


    //    private void OnTriggerStay(Collider other)
    //    {

    //        if (other.gameObject.CompareTag("Player"))
    //        {
    //            Attack();  // �÷��̾�� �浹 �� Attack �޼��� ȣ��
    //        }
    //    }
    //    // ���� ���� �Լ�
    //    void Die()
    //    {
    //        // ���� ���� �ǰ� �ڷ�ƾ�� ����
    //        StopAllCoroutines();

    //        // ���� ���¸� ó���ϱ� ���� �ڷ�ƾ
    //        StartCoroutine(DieProcess());
    //    }

    //    IEnumerator DieProcess()
    //    {
    //        // ĳ���� ��Ʈ�ѷ� ������Ʈ�� ��Ȱ��ȭ��Ų��
    //        cc.enabled = false;

    //        // 5�� ���� ��ٸ� �Ŀ� �ڱ� �ڽ��� �����Ѵ�
    //        yield return new WaitForSeconds(5f);
    //        print("�Ҹ�");
    //        Destroy(gameObject);

    //        // ���� ���� ���
    //        Endingvideo.SetActive(true);
    //        vid.Play(); // ���� ���
    //        isVideoPlaying = true;
    //        Time.timeScale = 0; // �ð� ����
    //    }

    //    void OnVideoEnd(VideoPlayer vp)
    //    {
    //        // ������ ������ �� ȣ��Ǵ� �Լ�
    //        isVideoPlaying = false;
    //        GoStartButton.SetActive(true);
    //        Time.timeScale = 1; // �ð� �ٽ� ����
    //    }

    //// �̰� ���� ���̰� ������ �׽�Ʈ  ... ���� �װ� ������ ������ �� ��.
    //using System.Collections;
    //using System.Collections.Generic;
    //using UnityEditor;
    //using UnityEngine;
    //using UnityEngine.SceneManagement;

    //public class BOSS : MonoBehaviour
    //{

    //    // ���ʹ� ���� ���
    //    public enum BossState
    //    {
    //        Idle,
    //        Move,
    //        Attack,
    //        Return,
    //        Damaged,
    //        Die
    //    }
    //    // ���ʹ� ���� ����
    //    public BossState m_state;

    //    // �÷��̾� �߰� ����
    //    public float findDistance = 80f;

    //    // �÷��̾� Ʈ������
    //    public Transform player;

    //    // ���� ���� ����
    //    public float attackDistance = 200f;

    //    // �̵� �ӵ�
    //    public float moveSpeed = 2f;

    //    // ĳ���� ��Ʈ�ѷ� ������Ʈ
    //    CharacterController cc;

    //    // ���� �ð�
    //    float currentTime = 0;

    //    // ���� ������ �ð�
    //    public float attackDelay = 2f;

    //    // ���ʹ��� ���ݷ�
    //    public int attackPower = 30;

    //    // �̵� ���� ����
    //    public float moveDistance = 200f;

    //    // ���ʹ��� ü��
    //    public int hp = 50000;

    //    // ������ �������� ������Ʈ
    //    public GameObject Endingvideo;

    //    // ������ ��ư ������Ʈ
    //    public GameObject GoStartButton;





    //    void Start()
    //    {
    //        // ������ ���ʹ�  ���´� ���(Idle)�� �Ѵ�.
    //        m_state = BossState.Idle;

    //        // �÷��̾��� Ʈ������ ������Ʈ �޾ƿ���
    //        player = GameObject.Find("PlayerCapsule").transform;

    //        // ĳ���� ��Ʈ�ѷ� ������Ʈ �޾ƿ���
    //        cc = GetComponent<CharacterController>();


    //    }

    //    void Update()
    //    {
    //        // ���� ���¸� üũ�� �ش� ���º��� ������ ����� �����ϰ� �ϰ�ʹ�.
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

    //    // ������ ���� �Լ�
    //    public void HitEnemy(int hitPower)
    //    {
    //        // ���� �̹� �ǰ� �����̰ų� ��� ���� �Ǵ� ���� ���¶�� �ƹ��� ó���� ���� �ʰ� �Լ��� �����Ѵ�.
    //        if (m_state == BossState.Damaged || m_state == BossState.Die || m_state == BossState.Return)
    //        {
    //            return;
    //        }

    //        // �÷��̾��� ���ݷ� ��ŭ ���ʹ� ü���� ����
    //        hp -= hitPower;
    //        // ���ʹ��� ü���� 0���� ũ�� �ǰ� ���·� ��ȯ
    //        if (hp > 0)
    //        {
    //            m_state = BossState.Damaged;
    //            print("���� ��ȯ : any state -> damaged");
    //            Damaged();
    //        }
    //        // �׷��� �ʴٸ� ���� ���·� ��ȯ
    //        else
    //        {
    //            m_state = BossState.Die;
    //            print("���� ��ȯ : any state -> die");
    //            Die();
    //        }
    //    }

    //    private void Damaged()
    //    {
    //        // �ǰ� ���¸� ó���ϱ� ���� �ڷ�ƾ ����
    //        StartCoroutine(DamageProcess());
    //    }
    //    // ������ ó���� �ڷ�ƾ �Լ�
    //    IEnumerator DamageProcess()
    //    {
    //        // �ǰ� ��� �ð���ŭ ��ٸ���.
    //        yield return new WaitForSeconds(0.5f);

    //        // ���� ���¸� �̵� ���·� ��ȯ
    //        m_state = BossState.Move;
    //        print("���� ��ȯ : damaged -> move");
    //    }

    //    void Idle()
    //    {
    //        // ����, �÷��̾���� �Ÿ��� �׼� ���� ���� �̳���� Move ���·� ��ȯ�Ѵ�.
    //        if (Vector3.Distance(transform.position, player.position) < findDistance)
    //        {
    //            m_state = BossState.Move;
    //            print("���� ��ȯ : Idle -> Move");
    //        }
    //    }

    //    void Move()
    //    {
    //        print(player.position);
    //        // ���� �÷��̾���� �Ÿ��� ���� ���� ���̶�� �÷��̾ ���� �̵��Ѵ�.
    //        if (Vector3.Distance(transform.position, player.position) > attackDistance)
    //        {
    //            // �̵� ���� ����
    //            Vector3 dir = (player.position - transform.position).normalized;

    //            // ĳ���� ��Ʈ�ѷ��� �̿��� �̵��ϱ�
    //            cc.Move(dir * moveSpeed * Time.deltaTime);

    //            // �÷��̾���� �Ÿ��� ���ݹ��� ���̶�� ���� ���¸� �������� ��ȯ�Ѵ�.
    //            if (Vector3.Distance(transform.position, player.position) < attackDistance)
    //            {
    //                print("���� ��ȯ : Move -> Attack");
    //                m_state = BossState.Attack;
    //            }


    //        }
    //        //�׷��� �ʴٸ� ���� ���¸� �������� ��ȯ�Ѵ�.
    //        else
    //        {
    //            m_state = BossState.Attack;
    //            print("���� ��ȯ : Move -> Attack");

    //            // ���� �ð��� ���� ������ �ð� ��ŭ �̸� ������� ���´�.
    //            currentTime = attackDelay;
    //        }
    //    }
    //    void Attack()
    //    {
    //        //���� �÷��̾ ���� ���� �̳��� �ִٸ� �÷��̾ �����Ѵ�.
    //        if (Vector3.Distance(transform.position, player.position) < attackDistance)
    //        {
    //            // ���� �ð����� �÷��̾ �����Ѵ�.
    //            currentTime += Time.deltaTime;
    //            if (currentTime > attackDelay)
    //            {
    //                player.GetComponent<CharacterStats>().TakeDamage(attackPower);
    //                currentTime = 0;
    //            }
    //        }
    //        //�׷��� �ʴٸ� ���� ���¸� �̵����� ��ȯ�Ѵ�(�߰�)
    //        else
    //        {
    //            m_state = BossState.Move;
    //            print("���� ��ȯ : Attack -> Move");
    //            currentTime = 0;
    //        }
    //    }


    //    private void OnTriggerStay(Collider other)
    //    {

    //        if (other.gameObject.CompareTag("Player"))
    //        {
    //            Attack();  // �÷��̾�� �浹 �� Attack �޼��� ȣ��
    //        }
    //    }
    //    // ���� ���� �Լ�
    //    void Die()
    //    {
    //        // ���� ���� �ǰ� �ڷ�ƾ�� ����
    //        StopAllCoroutines();

    //        // ���� ���¸� ó���ϱ� ���� �ڷ�ƾ
    //        StartCoroutine(DieProcess());
    //    }

    //    IEnumerator DieProcess()
    //    {
    //        // ĳ���� ��Ʈ�ѷ� ������Ʈ�� ��Ȱ��ȭ��Ų��
    //        cc.enabled = false;

    //        // 5�� ���� ��ٸ� �Ŀ� �ڱ� �ڽ��� �����Ѵ�
    //        yield return new WaitForSeconds(5f);
    //        print("�Ҹ�");
    //        Destroy(gameObject);

    //        // ���� ���� ���
    //        Endingvideo.SetActive(true);

    //        // �ð� ����
    //        Time.timeScale = 1;

    //        // �ð� ���� �Ŀ� ���� ���� UI����
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

}



//// �̰� ���� ���� �۵���
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class BOSS : MonoBehaviour
//{

//    // ���ʹ� ���� ���
//    public enum BossState
//    {
//        Idle,
//        Move,
//        Attack,
//        Return,
//        Damaged,
//        Die
//    }
//    // ���ʹ� ���� ����
//    public BossState m_state;

//    // �÷��̾� �߰� ����
//    public float findDistance = 80f;

//    // �÷��̾� Ʈ������
//    public Transform player;

//    // ���� ���� ����
//    public float attackDistance = 200f;

//    // �̵� �ӵ�
//    public float moveSpeed = 2f;

//    // ĳ���� ��Ʈ�ѷ� ������Ʈ
//    CharacterController cc;

//    // ���� �ð�
//    float currentTime = 0;

//    // ���� ������ �ð�
//    public float attackDelay = 2f;

//    // ���ʹ��� ���ݷ�
//    public int attackPower = 30;

//    // �̵� ���� ����
//    public float moveDistance = 200f;

//    // ���ʹ��� ü��
//    public int hp = 50000;

//    public CharacterStats playerStats;

//    void Start()
//    {
//        // ������ ���ʹ�  ���´� ���(Idle)�� �Ѵ�.
//        m_state = BossState.Idle;

//        // �÷��̾��� Ʈ������ ������Ʈ �޾ƿ���
//        player = GameObject.Find("PlayerCapsule").transform;

//        // ĳ���� ��Ʈ�ѷ� ������Ʈ �޾ƿ���
//        cc = GetComponent<CharacterController>();

//        playerStats = CharacterStats.cs;
//    }

//    void Update()
//    {
//        // ���� ���¸� üũ�� �ش� ���º��� ������ ����� �����ϰ� �ϰ�ʹ�.
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

//    // ������ ���� �Լ�
//    public void HitEnemy(int hitPower)
//    {
//        // ���� �̹� �ǰ� �����̰ų� ��� ���� �Ǵ� ���� ���¶�� �ƹ��� ó���� ���� �ʰ� �Լ��� �����Ѵ�.
//        if (m_state == BossState.Damaged || m_state == BossState.Die || m_state == BossState.Return)
//        {
//            return;
//        }

//        // �÷��̾��� ���ݷ� ��ŭ ���ʹ� ü���� ����
//        hp -= hitPower;
//        // ���ʹ��� ü���� 0���� ũ�� �ǰ� ���·� ��ȯ
//        if (hp > 0)
//        {
//            m_state = BossState.Damaged;
//            print("���� ��ȯ : any state -> damaged");
//            Damaged();
//        }
//        // �׷��� �ʴٸ� ���� ���·� ��ȯ
//        else
//        {
//            m_state = BossState.Die;
//            print("���� ��ȯ : any state -> die");
//            Die();
//        }
//    }

//    private void Damaged()
//    {
//        // �ǰ� ���¸� ó���ϱ� ���� �ڷ�ƾ ����
//        StartCoroutine(DamageProcess());
//    }
//    // ������ ó���� �ڷ�ƾ �Լ�
//    IEnumerator DamageProcess()
//    {
//        // �ǰ� ��� �ð���ŭ ��ٸ���.
//        yield return new WaitForSeconds(0.5f);

//        // ���� ���¸� �̵� ���·� ��ȯ
//        m_state = BossState.Move;
//        print("���� ��ȯ : damaged -> move");
//    }

//    void Idle()
//    {
//        // ����, �÷��̾���� �Ÿ��� �׼� ���� ���� �̳���� Move ���·� ��ȯ�Ѵ�.
//        if (Vector3.Distance(transform.position, player.position) < findDistance)
//        {
//            m_state = BossState.Move;
//            print("���� ��ȯ : Idle -> Move");
//        }
//    }

//    void Move()
//    {
//        print(player.position);
//        // ���� �÷��̾���� �Ÿ��� ���� ���� ���̶�� �÷��̾ ���� �̵��Ѵ�.
//        if (Vector3.Distance(transform.position, player.position) > attackDistance)
//        {
//            // �̵� ���� ����
//            Vector3 dir = (player.position - transform.position).normalized;

//            // ĳ���� ��Ʈ�ѷ��� �̿��� �̵��ϱ�
//            cc.Move(dir * moveSpeed * Time.deltaTime);

//            // �÷��̾���� �Ÿ��� ���ݹ��� ���̶�� ���� ���¸� �������� ��ȯ�Ѵ�.
//            if(Vector3.Distance(transform.position, player.position) < attackDistance)
//            {
//                print("���� ��ȯ : Move -> Attack");
//                m_state = BossState.Attack;
//            }


//        }
//        //�׷��� �ʴٸ� ���� ���¸� �������� ��ȯ�Ѵ�.
//        else
//        {
//            m_state = BossState.Attack;
//            print("���� ��ȯ : Move -> Attack");

//            // ���� �ð��� ���� ������ �ð� ��ŭ �̸� ������� ���´�.
//            currentTime = attackDelay;
//        }
//    }
//    void Attack()
//    {
//        //���� �÷��̾ ���� ���� �̳��� �ִٸ� �÷��̾ �����Ѵ�.
//        if (Vector3.Distance(transform.position, player.position) < attackDistance)
//        {
//            // ���� �ð����� �÷��̾ �����Ѵ�.
//            currentTime += Time.deltaTime;
//            if (currentTime > attackDelay)
//            {
//                player.GetComponent<CharacterStats>().TakeDamage(attackPower);
//                currentTime = 0;
//            }
//        }
//        //�׷��� �ʴٸ� ���� ���¸� �̵����� ��ȯ�Ѵ�(�߰�)
//        else
//        {
//            m_state = BossState.Move;
//            print("���� ��ȯ : Attack -> Move");
//            currentTime = 0;
//        }
//    }


//    private void OnTriggerStay(Collider other)
//    {
        
//        if (other.gameObject.CompareTag("Player"))
//        {
//            Attack();  // �÷��̾�� �浹 �� Attack �޼��� ȣ��
//        }
//    }
//    // ���� ���� �Լ�
//    void Die()
//    {
//        // ���� ���� �ǰ� �ڷ�ƾ�� ����
//        StopAllCoroutines();

//        // ���� ���¸� ó���ϱ� ���� �ڷ�ƾ
//        StartCoroutine(DieProcess());
//    }

//    IEnumerator DieProcess()
//    {
//        // ĳ���� ��Ʈ�ѷ� ������Ʈ�� ��Ȱ��ȭ��Ų��
//        cc.enabled = false;

//        // 2�� ���� ��ٸ� �Ŀ� �ڱ� �ڽ��� �����Ѵ�
//        yield return new WaitForSeconds(2f);
//        print("�Ҹ�");
//        Destroy(gameObject);

//    }
//    //Vector2 newPos = Random.insideUnitCircle * initPreferences.patrolRadius;
//    // patrolNext = patrolCenter + new Vector3(newPos.x, 0, newPos.y);
//    // myState = EnemyState.Idle;
//    // idleTime = Random.Range(2.0f, 3.0f);

//}

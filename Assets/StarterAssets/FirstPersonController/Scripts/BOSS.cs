using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOSS : MonoBehaviour
{

    // ���ʹ� ���� ���
    enum EnemyState
    {
        Idle,
        Move,
        Attack,
        Return,
        Damaged,
        Die
    }
    // ���ʹ� ���� ����
    EnemyState m_state;

    // �÷��̾� �߰� ����
    public float findDistance = 80f;

    // �÷��̾� Ʈ������
    Transform player;

    // ���� ���� ����
    public float attackDistance = 2f;

    // �̵� �ӵ�
    public float moveSpeed = 2f;

    // ĳ���� ��Ʈ�ѷ� ������Ʈ
    CharacterController cc;

    // ���� �ð�
    float currentTime = 0;

    // ���� ������ �ð�
    float attackDelay = 2f;

    // ���ʹ��� ���ݷ�
    public int attackPower = 10;

    // �ʱ� ��ġ ����� ����
    Vector3 orginPos;

    // �̵� ���� ����
    public float moveDistance = 200f;

    // ���ʹ��� ü��
    public int hp = 50000;
    //private int 

    void Start()
    {
        // ������ ���ʹ�  ���´� ���(Idle)�� �Ѵ�.
        m_state = EnemyState.Idle;

        // �÷��̾��� Ʈ������ ������Ʈ �޾ƿ���
        player = GameObject.Find("PlayerCapsule").transform;

        // ĳ���� ��Ʈ�ѷ� ������Ʈ �޾ƿ���
        cc = GetComponent<CharacterController>();

        // �ڽ��� �ʱ� ��ġ �����ϱ�
        orginPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // ���� ���¸� üũ�� �ش� ���º��� ������ ����� �����ϰ� �ϰ�ʹ�.
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

    // ������ ���� �Լ�
    public void HitEnemy(int hitPower)
    {
        // ���� �̹� �ǰ� �����̰ų� ��� ���� �Ǵ� ���� ���¶�� �ƹ��� ó���� ���� �ʰ� �Լ��� �����Ѵ�.
        if (m_state == EnemyState.Damaged || m_state == EnemyState.Die || m_state == EnemyState.Return)
        {
            return;
        }

        // �÷��̾��� ���ݷ� ��ŭ ���ʹ� ü���� ����
        hp -= hitPower;
        // ���ʹ��� ü���� 0���� ũ�� �ǰ� ���·� ��ȯ
        if (hp > 0)
        {
            m_state = EnemyState.Damaged;
            print("���� ��ȯ : any state -> damaged");
            Damaged();
        }
        // �׷��� �ʴٸ� ���� ���·� ��ȯ
        else
        {
            m_state = EnemyState.Die;
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
        m_state = EnemyState.Move;
        print("���� ��ȯ : damaged -> move");
    }

    private void Return()
    {
        // ���� �ʱ� ��ġ������ �Ÿ��� 0.1f �̻��̸� �ʱ� ��ġ ������ �̵�
        if (Vector3.Distance(transform.position, orginPos) > 0.1f)
        {
            Vector3 dir = (orginPos - transform.position).normalized;
            cc.Move(dir * moveSpeed * Time.deltaTime);
        }
        // �׷��� �ʴٸ� �ڽ��� ��ġ�� �ʱ� ��ġ�� �����ϰ� ���� ���¸� ���� ��ȯ
        else
        {
            transform.position = orginPos;
            // hp�� �ٽ� ȸ��
            // hp = maxHp;
            m_state = EnemyState.Idle;
            print("���� ��ȯ : return -> Idle");
        }

    }

    void Idle()
    {
        // ����, �÷��̾���� �Ÿ��� �׼� ���� ���� �̳���� Move ���·� ��ȯ�Ѵ�.
        if (Vector3.Distance(transform.position, player.position) < findDistance)
        {
            m_state = EnemyState.Move;
            print("���� ��ȯ : Idle -> Move");
        }
    }

    void Move()
    {
        // ���� ���� ��ġ�� �ʱ� ��ġ���� �̵� ���� ������ �Ѿ�ٸ�
        if (Vector3.Distance(transform.position, orginPos) > moveDistance)
        {
            // ���� ���¸� ����(return)�� ��ȯ
            m_state = EnemyState.Return;
            print("���� ��ȯ : move -> return");
        }

        // ���� �÷��̾���� �Ÿ��� ���� ���� ���̶�� �÷��̾ ���� �̵��Ѵ�.
        else if (Vector3.Distance(transform.position, player.position) > attackDistance)
        {
            // �̵� ���� ����
            Vector3 dir = (player.position - transform.position).normalized;

            // ĳ���� ��Ʈ�ѷ��� �̿��� �̵��ϱ�
            cc.Move(dir * moveSpeed * Time.deltaTime);
        }
        //�׷��� �ʴٸ� ���� ���¸� �������� ��ȯ�Ѵ�.
        else
        {
            m_state = EnemyState.Attack;
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
                player.GetComponent<CharacterStats>().DamageAction(attackPower);
                print("����");
                currentTime = 0;
            }

        }
        //�׷��� �ʴٸ� ���� ���¸� �̵����� ��ȯ�Ѵ�(�߰�)
        else
        {
            m_state = EnemyState.Move;
            print("���� ��ȯ : Attack -> Move");
            currentTime = 0;
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

        // 2�� ���� ��ٸ� �Ŀ� �ڱ� �ڽ��� �����Ѵ�
        yield return new WaitForSeconds(2f);
        print("�Ҹ�");
        Destroy(gameObject);
    }



}

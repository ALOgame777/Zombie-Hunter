// ���� �߷� �߰�? + ++ �̵� �������� ȸ�� �߰�
using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemyFSM : MonoBehaviour
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
    public float findDistance = 8f;

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
    public int attackPower = 5;

    // �ʱ� ��ġ ����� ����
    Vector3 orginPos;

    // �̵� ���� ����
    public float moveDistance = 50f;

    // ���ʹ��� ü��
    public int hp = 200;
    private int maxHp = 10000;

    // ĳ���� �������ͽ� ��ũ��Ʈ  �ҷ�����
    private CharacterStats playerStats;

    // �߷� ������ ���� ����
    private Vector3 velocity;
    public float gravity = -9.81f;

    public float patrolSpeed = 4;

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

        playerStats = CharacterStats.cs;
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
    //// ������ ������ �������� �ο��ϴ� �Լ�
    //public void TakeDamage(float atkPower, Vector3 hitDir, Transform attacker)
    //{
       

    //    // 1. ���� ü�¿� ����� ���ݷ¸�ŭ�� ���ҽ�Ų��. (min 0 ~ max 100)
    //    hp = Mathf.Clamp(hp - hitPower, 0, maxHP);

    //    // 2. ���� �� ��� ���� ü���� 0 ���϶��
    //    if (currentHP <= 0)
    //    {
    //        // 2-1. ���� ���¸� ���� ���·� ��ȯ
    //        myState = EnemyState.Dead;
    //        print("My state : any -> dead");
    //        currentTime = 0;

    //        // 2-2. �ݶ��̴� ������Ʈ ��Ȱ��ȭ
    //        GetComponent<CapsuleCollider>().enabled = false;
    //        GetComponent<CharacterController>().enabled = false;

    //    }
    //    // 3. �׷��� �ʴٸ�
    //    else
    //    {
    //        // 3-1. ���� ���¸� ������ ���·� ��ȯ
    //        myState = EnemyState.Damaged;
    //        print("My state : any -> damaged");
    //        // 3-2. Ÿ�� �������� ���� �Ÿ���ŭ�� �˹� ��ġ�� �����Ѵ�.
    //        hitDirection = transform.position + hitDir * 1.5f;
    //        // 3-3. �����ڸ� Ÿ������ ����
    //        target = attacker;
    //        //hitDirection = transform.position + hitDir * 
    //    }
    //}

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
            hp = maxHp;
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

            // y �� �̵� ����
            dir.y = 0;

            // ĳ���� ��Ʈ�ѷ��� �̿��� �̵��ϱ�
            //cc.Move(dir * moveSpeed * Time.deltaTime);

            if (dir.magnitude > 0.1f)
            {
                // ĳ���� ��Ʈ�ѷ��� �̿��� �̵��ϱ�
                cc.Move(dir * moveSpeed * Time.deltaTime);

                // �̵��Ϸ��� �������� ȸ���Ѵ�.
                transform.rotation = Quaternion.LookRotation(dir.normalized);
            }

            // �߷� ����
            if (!cc.isGrounded)
            {
                velocity.y += gravity * Time.deltaTime;
                cc.Move(velocity * Time.deltaTime);
            }
            else
            {
                velocity.y = 0;
            }

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
            PlayerInvincibility playerInvincibility = player.GetComponent<PlayerInvincibility>();
            if (playerInvincibility != null && playerInvincibility.IsInvincible())
            {
                // Player is invincible, do not attack
                print("�÷��̾ ���� �����Դϴ�. ������ �����մϴ�.");
                return;
            }
            // ���� �ð����� �÷��̾ �����Ѵ�.
            currentTime += Time.deltaTime;
            if (currentTime > attackDelay)
            {
                player.GetComponent<CharacterStats>().TakeDamage(attackPower);
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
    private void OnCollisionStay(Collision collision)
{
    if (collision.gameObject.CompareTag("Player"))
    {
        Attack();  // �÷��̾�� �浹 �� Attack �޼��� ȣ��
    }
}
    // ���� ���� �Լ�
    void Die()
    {
        // ���� ���� �ǰ� �ڷ�ƾ�� ����
        StopAllCoroutines();
        Debug.Log("�� ��� �� ����: " + ScoreManager.Instance.GetScore());
        ScoreManager.Instance.AddScore(10000);
        Debug.Log("�� ��� �� ����: " + ScoreManager.Instance.GetScore());
        // ���� ���¸� ó���ϱ� ���� �ڷ�ƾ
        StartCoroutine(DieProcess());
    }

    IEnumerator DieProcess()
    {
        // ĳ���� ��Ʈ�ѷ� ������Ʈ�� ��Ȱ��ȭ��Ų��
        cc.enabled = false;

        // 2�� ���� ��ٸ� �Ŀ� �ڱ� �ڽ��� �����Ѵ�
        yield return new WaitForSeconds(1f);
        print("�Ҹ�");
        Destroy(gameObject);
    }



}

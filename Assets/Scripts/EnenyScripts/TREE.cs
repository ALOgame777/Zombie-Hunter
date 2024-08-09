
// �� �� �˸°� �� �Ȱ� ���� ���� ���� ü�� + �÷��̾� �ӹ� + 
using System.Collections;
using UnityEngine;

public class TREE : MonoBehaviour
{
    public int maxHealth = 3; // ������ �ִ� ü��
    private int currentHealth; // ������ ���� ü��

    private void Start()
    {
        currentHealth = maxHealth; // ���� ü���� �ִ� ü������ ����
    }
    private void OnTriggerEnter(Collider other)
    {
        // �÷��̾�� �浹�ߴ��� Ȯ��
        if (other.CompareTag("Player"))
        {
            // �÷��̾��� CharacterController�� ã�� �ӹ� �ڷ�ƾ ����
            CharacterController playerController = other.GetComponent<CharacterController>();
            if (playerController != null)
            {
                StartCoroutine(BindPlayer(playerController));
            }
        }
    }

    private IEnumerator BindPlayer(CharacterController playerController)
    {
        // �÷��̾��� �̵� �ӵ��� 0���� ����
        playerController.enabled = false;

        // 1�� ���� ��ٸ�
        yield return new WaitForSeconds(1f);

        // �÷��̾��� �̵��� �ٽ� Ȱ��ȭ
        playerController.enabled = true;
    }
    // ������ �������� ���� �� ȣ��Ǵ� �Լ�
    public void TakeDamage(int damage)
    {
        currentHealth -= damage; // ���� ü���� ���ҽ�Ŵ
        DamagePopUpGenerator.current.CreatePopUp(transform.position, damage.ToString(), Color.red);
        Debug.Log("Tree took damage, current health: " + currentHealth);
        if (currentHealth <= 0)
        {
            Die(); // ü���� 0 ���ϰ� �Ǹ� ���� ó��
        }
    }

    // ������ �׾��� �� ȣ��Ǵ� �Լ�
    private void Die()
    {
        Debug.Log("Tree died");
        Destroy(gameObject); // ���� ������Ʈ�� �ı�
        ScoreManager.Instance.AddScore(10000);
    }

}


//// ��Ȱ��ȭ �Ǵϱ� ī�޶� ����
//using System.Collections;
//using UnityEngine;

//public class TREE : MonoBehaviour
//{
//    private void OnTriggerEnter(Collider other)
//    {
//        // �÷��̾�� �浹�ߴ��� Ȯ��
//        if (other.CompareTag("Player"))
//        {
//            // �÷��̾� ������Ʈ�� ã�� �ӹ� �ڷ�ƾ ����
//            StartCoroutine(BindPlayer(other.gameObject));
//        }
//    }

//    private IEnumerator BindPlayer(GameObject player)
//    {
//        // �÷��̾��� �������� ���߱� ���� �÷��̾ ��Ȱ��ȭ
//        player.SetActive(false);

//        // 1�� ���� ��ٸ�
//        yield return new WaitForSeconds(1f);

//        // �÷��̾��� �������� �ٽ� Ȱ��ȭ
//        player.SetActive(true);
//    }
//}

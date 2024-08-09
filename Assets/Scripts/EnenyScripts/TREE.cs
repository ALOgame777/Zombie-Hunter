
// 와 딱 알맞게 잘 된거 같음 ㅋㅋ 나무 체력 + 플레이어 속박 + 
using System.Collections;
using UnityEngine;

public class TREE : MonoBehaviour
{
    public int maxHealth = 3; // 나무의 최대 체력
    private int currentHealth; // 나무의 현재 체력

    private void Start()
    {
        currentHealth = maxHealth; // 현재 체력을 최대 체력으로 설정
    }
    private void OnTriggerEnter(Collider other)
    {
        // 플레이어와 충돌했는지 확인
        if (other.CompareTag("Player"))
        {
            // 플레이어의 CharacterController를 찾고 속박 코루틴 시작
            CharacterController playerController = other.GetComponent<CharacterController>();
            if (playerController != null)
            {
                StartCoroutine(BindPlayer(playerController));
            }
        }
    }

    private IEnumerator BindPlayer(CharacterController playerController)
    {
        // 플레이어의 이동 속도를 0으로 설정
        playerController.enabled = false;

        // 1초 동안 기다림
        yield return new WaitForSeconds(1f);

        // 플레이어의 이동을 다시 활성화
        playerController.enabled = true;
    }
    // 나무가 데미지를 받을 때 호출되는 함수
    public void TakeDamage(int damage)
    {
        currentHealth -= damage; // 현재 체력을 감소시킴
        DamagePopUpGenerator.current.CreatePopUp(transform.position, damage.ToString(), Color.red);
        Debug.Log("Tree took damage, current health: " + currentHealth);
        if (currentHealth <= 0)
        {
            Die(); // 체력이 0 이하가 되면 죽음 처리
        }
    }

    // 나무가 죽었을 때 호출되는 함수
    private void Die()
    {
        Debug.Log("Tree died");
        Destroy(gameObject); // 나무 오브젝트를 파괴
        ScoreManager.Instance.AddScore(10000);
    }

}


//// 비활성화 되니까 카메라가 꺼짐
//using System.Collections;
//using UnityEngine;

//public class TREE : MonoBehaviour
//{
//    private void OnTriggerEnter(Collider other)
//    {
//        // 플레이어와 충돌했는지 확인
//        if (other.CompareTag("Player"))
//        {
//            // 플레이어 오브젝트를 찾고 속박 코루틴 시작
//            StartCoroutine(BindPlayer(other.gameObject));
//        }
//    }

//    private IEnumerator BindPlayer(GameObject player)
//    {
//        // 플레이어의 움직임을 멈추기 위해 플레이어를 비활성화
//        player.SetActive(false);

//        // 1초 동안 기다림
//        yield return new WaitForSeconds(1f);

//        // 플레이어의 움직임을 다시 활성화
//        player.SetActive(true);
//    }
//}

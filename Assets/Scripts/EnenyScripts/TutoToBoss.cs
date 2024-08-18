// 소리 나오고 3초 뒤에 이동
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutoToBoss : MonoBehaviour
{
    // 오디오 소스
    public AudioSource audioSource;

    // 소리 클립
    public AudioClip potalsound;

    void Start()
    {
        // AudioSource 컴포넌트 가져오기
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // 빈 Update 함수는 지워도 돼
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 소리 재생
            audioSource.clip = potalsound;
            audioSource.volume = 300;
            audioSource.Play();

            // 3초 후에 Scene을 로드하는 코루틴 실행
            StartCoroutine(LoadSceneAfterDelay(3f));
        }
    }

    IEnumerator LoadSceneAfterDelay(float delay)
    {
        // delay 시간만큼 대기
        yield return new WaitForSeconds(delay);

        // 지정된 씬 로드
        SceneManager.LoadScene(2);
    }
}

//// 기존 이동
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class TutoToBoss : MonoBehaviour
//{
//    // Start is called before the first frame update
//    void Start()
//    {

//    }

//    // Update is called once per frame
//    void Update()
//    {

//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            SceneManager.LoadScene(2);

//        }
//    }
//}
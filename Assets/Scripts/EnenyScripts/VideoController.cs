using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    public GameObject uiCanvas; // UI를 포함하는 캔버스를 연결해줘

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();

       
    }

    public void PlayVideo()
    {
        Debug.Log("보스가 파괴되어 동영상을 재생합니다.");  // 로그를 남겨서 함수 호출 여부를 확인

        // UI 비활성화
        if (uiCanvas != null)
        {
            uiCanvas.SetActive(false);
        }

        // 동영상 재생
        videoPlayer.Play();
    }

   
}


//using UnityEngine;
//using UnityEngine.Video;

//public class VideoController : MonoBehaviour
//{
//    private VideoPlayer videoPlayer;

//    void Start()
//    {
//        videoPlayer = GetComponent<VideoPlayer>();
//    }

//    public void PlayVideo()
//    {
//        Debug.Log("보스가 파괴되어 동영상을 재생합니다.");  // 로그를 남겨서 함수 호출 여부를 확인
//        videoPlayer.Play();
//    }
//}

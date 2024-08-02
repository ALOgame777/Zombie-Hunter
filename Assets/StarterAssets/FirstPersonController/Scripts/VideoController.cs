using UnityEngine;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
    }

    public void PlayVideo()
    {
        Debug.Log("보스가 파괴되어 동영상을 재생합니다.");  // 로그를 남겨서 함수 호출 여부를 확인
        videoPlayer.Play();
    }
}

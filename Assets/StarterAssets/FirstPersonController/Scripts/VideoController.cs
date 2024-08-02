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
        Debug.Log("������ �ı��Ǿ� �������� ����մϴ�.");  // �α׸� ���ܼ� �Լ� ȣ�� ���θ� Ȯ��
        videoPlayer.Play();
    }
}

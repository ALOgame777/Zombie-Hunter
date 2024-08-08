using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    // 실행할 버튼 오브젝트
    public GameObject GoStartButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 보스 죽이고 동영상 나오고 버튼 나온 뒤에 'E' 키를 누르면 씬을 로드
        if (GoStartButton.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene(0);
        }
    }
}

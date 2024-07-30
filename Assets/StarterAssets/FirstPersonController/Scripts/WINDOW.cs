using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WINDOW : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerStay(Collider other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            REPAIR();  // 플레이어와 충돌 시 수리 메서드 호출
        }
    }
    void REPAIR()
    {

    }
}
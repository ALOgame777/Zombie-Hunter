using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public PlayerInvincibility playerInvincibility;

    public void RespawnPlayer()
    {
        // 플레이어 리스폰 로직...

        // 리스폰 후 무적 상태 적용
        playerInvincibility.Respawn();
    }
}

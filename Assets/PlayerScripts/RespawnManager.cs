using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public PlayerInvincibility playerInvincibility;

    public void RespawnPlayer()
    {
        // �÷��̾� ������ ����...

        // ������ �� ���� ���� ����
        playerInvincibility.Respawn();
    }
}

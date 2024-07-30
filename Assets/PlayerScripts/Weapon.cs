using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] Camera FPCamera;
    [SerializeField] float range = 100.0f;
    [SerializeField] int damage = 50;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        RaycastHit hit;
        if( Physics.Raycast(FPCamera.transform.position, FPCamera.transform.forward, out hit, range))
        {
            
            EnemyFSM target = hit.transform.GetComponent<EnemyFSM>();
            if (target == null) return;
            target.HitEnemy(damage);
        }
        else
        {
            return;
        }

        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallWeapon : MonoBehaviour
{
    public GameObject weaponPrefab;
    public float interactionDistance = 2f;
    public KeyCode interactionKey = KeyCode.E;

    private void Update()
    {
        if (Input.GetKeyDown(interactionKey))
        {
            TrySwapWeapon();
        }
    }

    private void TrySwapWeapon()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactionDistance);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                WeaponManager playerWeaponManager = collider.GetComponent<WeaponManager>();
                if (playerWeaponManager != null)
                {
                    playerWeaponManager.SwapWeapon(weaponPrefab);
                    Destroy(gameObject);
                    break;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}

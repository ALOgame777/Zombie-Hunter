using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    private PlayerHUD hud;
    private void Start()
    {
        GetReferences();
        InitVariables();
        CheckHealth();
    }

    public void GetReferences()
    {
        hud = GetComponent<PlayerHUD>();
    }

    public override void CheckHealth()
    {
        base.CheckHealth();
        if (hud != null)
        {
            hud.UpdateHealth(health, maxHealth);
        }
        else
        {
            Debug.LogError("PlayerHUD reference is missing!");
        }
    }

}

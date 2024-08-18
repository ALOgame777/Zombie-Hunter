using BigRookGames.Build;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowWallDamageController : MonoBehaviour
{
    public BasicWoodWallController targetWall;
    private int currentDamageStage = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ApplyDamage();
        }
    }

    void ApplyDamage()
    {
        if (targetWall != null)
        {
            targetWall.PlayDamageAnimation(currentDamageStage);
            Debug.Log($"Damage applied to WindowWall. Stage: {currentDamageStage}");

            // Increase damage stage for next press, reset if it reaches 5
            currentDamageStage = (currentDamageStage + 1) % 5;
        }
        else
        {
            Debug.LogWarning("WindowWall reference is missing!");
        }
    }
}
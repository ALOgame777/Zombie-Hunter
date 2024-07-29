
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public int health;
    public int maxHealth;

    public bool isDead;

    private void Start()
    {
        InitVariables();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(10);
        }
    }
    public virtual void CheckHealth()
    {
        if(health <= 0)
        {
            health = 0;
            Die();
        }
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }
    private void setHealthTo(int healthTosetto)
    {
        health = healthTosetto;
        CheckHealth();
    }

    public void TakeDamage(int damage)
    {
        int healthAfterDamage = health - damage;
        setHealthTo(healthAfterDamage);
    }

    public void Heal(int heal)
    {
        int healthAfterHeal = health + heal;
        setHealthTo(healthAfterHeal);
    }

    public void InitVariables()
    {
        maxHealth = 300;
        setHealthTo(maxHealth);
        isDead = false;
    }

    public void Die()
    {
        isDead = true;
    }

    // K 작성 코드 입니당
    // 플레이어 피격  함수
    public void DamageAction(int damage)
    {
        // 에너미의 공격력 만큼 플레이어의 체력을 깎는닷!
        health -= damage;
    }

}

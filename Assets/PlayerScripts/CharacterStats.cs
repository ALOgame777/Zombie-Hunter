
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

    // K �ۼ� �ڵ� �Դϴ�
    // �÷��̾� �ǰ�  �Լ�
    public void DamageAction(int damage)
    {
        // ���ʹ��� ���ݷ� ��ŭ �÷��̾��� ü���� ��´�!
        health -= damage;
    }

}

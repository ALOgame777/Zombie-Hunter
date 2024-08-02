
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStats : MonoBehaviour    
{
    public static CharacterStats cs;
    public int health;
    public int maxHealth;
    public Image img_hitUI;
    public bool isDead;

    private void Awake()
    {
        if (cs == null)
        {
            cs = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
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
        StartCoroutine(TakeHit(0.5f));
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

    IEnumerator TakeHit(float delayTime)
    {
        //float addValue = 0.05f;
        for (int i = 0; i < 100; i++)
        {
            Color colorVector = img_hitUI.color;
            float addValue = 0.05f;
            if (i > 49)
            {
                addValue *= -1;
            }
            colorVector.a += addValue;
            img_hitUI.color = colorVector;
            //yield return new WaitForSeconds(delayTime);
            yield return null;
        }
    }

}

// 체력바 UI 해보자... 성공?
using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Slider를 사용하기 위해

public class CharacterStats : MonoBehaviour
{
    public static CharacterStats cs;
    public int health;
    public int maxHealth;
    public Image img_hitUI;
    public bool isDead = false;

    public Slider HPsliderPlayer; // 플레이어 체력바

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
        UpdateHealthUI(); // 체력 UI 초기화
    }

    private void Update()
    {

    }

    public virtual void CheckHealth()
    {
        if (health <= 0)
        {
            health = 0;
            Die();
        }
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    public void setHealthTo(int healthTosetto)
    {
        health = healthTosetto;
        CheckHealth();
        UpdateHealthUI(); // 체력 UI 업데이트
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
        PlayerInvincibility.pi.Respawn();
    }

    private void UpdateHealthUI()
    {
        // 체력 바의 값 업데이트
        HPsliderPlayer.value = (float)health / maxHealth;
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
            colorVector.a = Mathf.Clamp01(colorVector.a + addValue); // 알파 값 클램프
            img_hitUI.color = colorVector;
            yield return null;
        }
    }
}

//// 그냥 체력바 슬라이더 하려고 했는데... 아니 플레이어가 무적이 되어버렸잖아?
//using System.Collections;
//using UnityEditor.UI;
//using UnityEngine;
//using UnityEngine.UI;
//public class CharacterStats : MonoBehaviour
//{
//    public static CharacterStats cs;
//    public int health;
//    public int maxHealth;
//    public Image img_hitUI;
//    public bool isDead = false;

//    public Slider HPsliderPlayer; // 플레이어 체력바 

//    private void Awake()
//    {
//        if (cs == null)
//        {
//            cs = this;
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    private void Start()
//    {
//        InitVariables();
//    }

//    private void Update()
//    {

//    }

//    public virtual void CheckHealth()
//    {
//        if (health <= 0)
//        {
//            health = 0;
//            Die();
//        }
//        if (health > maxHealth)
//        {
//            health = maxHealth;
//        }
//    }

//    public void setHealthTo(int healthTosetto)
//    {
//        health = healthTosetto;
//        CheckHealth();
//        //체력 UI 업데이트
//        HPsliderPlayer.value = (float)health / maxHealth; // float으로 나눠서 소수점 반영
//    }

//    public void TakeDamage(int damage)
//    {
//        int healthAfterDamage = health - damage;
//        StartCoroutine(TakeHit(0.5f));
//        setHealthTo(healthAfterDamage);
//    }

//    public void Heal(int heal)
//    {
//        int healthAfterHeal = health + heal;
//        setHealthTo(healthAfterHeal);
//    }

//    public void InitVariables()
//    {
//        maxHealth = 300;
//        setHealthTo(maxHealth); // 체력과 체력바 초기화
//        isDead = false;
//    }

//    public void Die()
//    {
//        isDead = true;
//        PlayerInvincibility.pi.Respawn();
//    }

//    IEnumerator TakeHit(float delayTime)
//    {
//        Color colorVector = img_hitUI.color;
//        for (int i = 0; i < 100; i++)
//        {
//            float addValue = 0.05f;
//            if (i > 49)
//            {
//                addValue *= -1;
//            }
//            colorVector.a = Mathf.Clamp01(colorVector.a + addValue); // 알파 값을 0과 1 사이로 유지
//            img_hitUI.color = colorVector;
//            yield return null;
//            //yield return new WaitForSeconds(delayTime);
//        }
//    }
//}
//기존 식
//using System.Collections;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEditor.UI;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.UIElements;

//public class CharacterStats : MonoBehaviour
//{
//    public static CharacterStats cs;
//    public int health;
//    public int maxHealth;
//    public Image img_hitUI;
//    public bool isDead = false;

//    public SliderEditor HPsliderPlayer; // 플레이어 체력바 

//    private void Awake()
//    {
//        if (cs == null)
//        {
//            cs = this;
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }
//    private void Start()
//    {
//        InitVariables();
//    }

//    private void Update()
//    {

//    }
//    public virtual void CheckHealth()
//    {
//        if (health <= 0)
//        {
//            health = 0;
//            Die();
//        }
//        if (health > maxHealth)
//        {
//            health = maxHealth;
//        }
//    }
//    public void setHealthTo(int healthTosetto)
//    {
//        health = healthTosetto;
//        CheckHealth();
//    }

//    public void TakeDamage(int damage)
//    {
//        int healthAfterDamage = health - damage;
//        StartCoroutine(TakeHit(0.5f));
//        setHealthTo(healthAfterDamage);
//    }

//    public void Heal(int heal)
//    {
//        int healthAfterHeal = health + heal;
//        setHealthTo(healthAfterHeal);
//    }

//    public void InitVariables()
//    {
//        maxHealth = 300;
//        setHealthTo(maxHealth);
//        isDead = false;

//    }

//    public void Die()
//    {
//        isDead = true;
//        PlayerInvincibility.pi.Respawn();
//    }

//    IEnumerator TakeHit(float delayTime)
//    {
//        //float addValue = 0.05f;
//        for (int i = 0; i < 100; i++)
//        {
//            Color colorVector = img_hitUI.color;
//            float addValue = 0.05f;
//            if (i > 49)
//            {
//                addValue *= -1;
//            }
//            colorVector.a += addValue;
//            img_hitUI.color = colorVector;
//            //yield return new WaitForSeconds(delayTime);
//            yield return null;
//        }
//    }
//}

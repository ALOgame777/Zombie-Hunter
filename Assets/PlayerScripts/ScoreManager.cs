using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private int score = 0;
    private int money = 0;
    public int scoreToMoneyRatio = 10; // 10���� 1��

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        UIManager.Instance.UpdateScoreText(0);
        UIManager.Instance.UpdateMoneyText(0);
    }
    public void AddScore(int amount)
    {
        score += amount;
        ConvertScoreToMoney();
        UIManager.Instance.UpdateScoreText(score);
        UIManager.Instance.UpdateMoneyText(money);
    }

    private void ConvertScoreToMoney()
    {
        int newMoney = score / scoreToMoneyRatio;
        int moneyToAdd = newMoney - money;
        money = newMoney;

        if (moneyToAdd > 0)
        {
            Debug.Log($"�÷��̾ {moneyToAdd}���� ������ϴ�!");
        }
    }

    public int GetScore()
    {
        return score;
    }

    public int GetMoney()
    {
        return money;
    }

}

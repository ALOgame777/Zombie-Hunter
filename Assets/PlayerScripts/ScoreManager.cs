using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private int score = 0;
    private int money = 0;
    public int scoreToMoneyRatio = 10; // 10점당 1원

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
            Debug.Log($"플레이어가 {moneyToAdd}원을 얻었습니다!");
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
    public bool TryOpenDoor(int requiredMoney)
    {
        Debug.Log($"문 열기 시도: 필요 금액 = {requiredMoney}, 현재 돈 = {money}");
        if (money >= requiredMoney)
        {
            money -= requiredMoney;
            UIManager.Instance.UpdateMoneyText(money);
            
            Debug.Log($"문이 열렸습니다! 남은 돈: {money}");
            return true; // 문이 열리는 상태로 변경
        }
        else
        {
            Debug.Log("돈이 부족합니다. 문을 열 수 없습니다.");
            return false;
        }
    }
}

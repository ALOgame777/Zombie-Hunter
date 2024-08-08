// 이제 정상적으로 돈 쌓이고, 점수도 안사라짐.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private int score = 0;
    private int money = 0;
    private int totalMoneyEarned = 0; // 총 획득한 돈
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
        AddMoney(amount / scoreToMoneyRatio);
        UIManager.Instance.UpdateScoreText(score);
        UIManager.Instance.UpdateMoneyText(money);
    }

    private void AddMoney(int amount)
    {
        totalMoneyEarned += amount;
        money += amount;
        UIManager.Instance.UpdateMoneyText(money);
    }

    public int GetScore()
    {
        return score;
    }

    public int GetMoney()
    {
        return money;
    }

    public bool TryOpenDoor(int requiredMoney1 = 700)
    {
        Debug.Log($"문 열기 시도: 필요 금액 = {requiredMoney1}, 현재 돈 = {money}");
        if (money >= requiredMoney1)
        {
            money -= requiredMoney1;
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
    public bool BuyDoubleShoot(int requiredMoney2 = 2000)
    {
        Debug.Log($"더블샷 구매 시도: 필요 금액 = {requiredMoney2}, 현재 돈 = {money}");
        if (money >= requiredMoney2)
        {
            money -= requiredMoney2;
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
    public bool BuyAK(int requiredMoney3 = 3000)
    {
        Debug.Log($"더블샷 구매 시도: 필요 금액 = {requiredMoney3}, 현재 돈 = {money}");
        if (money >= requiredMoney3)
        {
            money -= requiredMoney3;
            UIManager.Instance.UpdateMoneyText(money);

            
            return true;
        }
        else
        {
           
            return false;
        }
    }

    public bool UpgradeWeapon(int requiredMoney5 = 2000)
    {
        Debug.Log($" 업그레이드 구매 시도: 필요 금액 = {requiredMoney5}, 현재 돈 = {money}");
        if (money >= requiredMoney5)
        {
            money -= requiredMoney5;
            UIManager.Instance.UpdateMoneyText(money);
            return true;
        }
        else
        {
            
            return false;
        }
    }
}


//// 이제 정상적으로 돈 쌓이고, 점수도 안사라짐.
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class ScoreManager : MonoBehaviour
//{
//    public static ScoreManager Instance;

//    private int score = 0;
//    private int money = 0;
//    private int totalMoneyEarned = 0; // 총 획득한 돈
//    public int scoreToMoneyRatio = 10; // 10점당 1원

//    private void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    void Start()
//    {
//        UIManager.Instance.UpdateScoreText(0);
//        UIManager.Instance.UpdateMoneyText(0);
//    }

//    public void AddScore(int amount)
//    {
//        score += amount;
//        AddMoney(amount / scoreToMoneyRatio);
//        UIManager.Instance.UpdateScoreText(score);
//        UIManager.Instance.UpdateMoneyText(money);
//    }

//    private void AddMoney(int amount)
//    {
//        totalMoneyEarned += amount;
//        money += amount;
//        UIManager.Instance.UpdateMoneyText(money);
//    }

//    public int GetScore()
//    {
//        return score;
//    }

//    public int GetMoney()
//    {
//        return money;
//    }

//    public bool TryOpenDoor(int requiredMoney)
//    {
//        Debug.Log($"문 열기 시도: 필요 금액 = {requiredMoney}, 현재 돈 = {money}");
//        if (money >= requiredMoney)
//        {
//            money -= requiredMoney;
//            UIManager.Instance.UpdateMoneyText(money);

//            Debug.Log($"문이 열렸습니다! 남은 돈: {money}");
//            return true; // 문이 열리는 상태로 변경
//        }
//        else
//        {
//            Debug.Log("돈이 부족합니다. 문을 열 수 없습니다.");
//            return false;
//        }
//    }
//}



//// 돈 정상적으로 차감 됨. 돈 써도 정상적임. 근데 점수도 같이 차감됨ㅋㅋ
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class ScoreManager : MonoBehaviour
//{
//    public static ScoreManager Instance;

//    private int score = 0;
//    private int money = 0;
//    public int scoreToMoneyRatio = 10; // 10점당 1원

//    private void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }
//    void Start()
//    {
//        UIManager.Instance.UpdateScoreText(0);
//        UIManager.Instance.UpdateMoneyText(0);
//    }
//    public void AddScore(int amount)
//    {
//        score += amount;
//        ConvertScoreToMoney();
//        UIManager.Instance.UpdateScoreText(score);
//        UIManager.Instance.UpdateMoneyText(money);
//    }

//    private void ConvertScoreToMoney()
//    {
//        money = score / scoreToMoneyRatio;
//        UIManager.Instance.UpdateMoneyText(money);
//    }

//    public int GetScore()
//    {
//        return score;
//    }

//    public int GetMoney()
//    {
//        return money;
//    }

//    public bool TryOpenDoor(int requiredMoney)
//    {
//        Debug.Log($"문 열기 시도: 필요 금액 = {requiredMoney}, 현재 돈 = {money}");
//        if (money >= requiredMoney)
//        {
//            // 필요한 돈에 해당하는 점수를 차감
//            int scoreToDeduct = requiredMoney * scoreToMoneyRatio;
//            score -= scoreToDeduct;

//            // 점수를 차감한 후 돈을 다시 계산
//            ConvertScoreToMoney();
//            UIManager.Instance.UpdateScoreText(score);
//            UIManager.Instance.UpdateMoneyText(money);

//            Debug.Log($"문이 열렸습니다! 남은 돈: {money}");
//            return true; // 문이 열리는 상태로 변경
//        }
//        else
//        {
//            Debug.Log("돈이 부족합니다. 문을 열 수 없습니다.");
//            return false;
//        }
//    }
//}



// 원래 프로토. 돈 쓴 곳에서 돈 벌면 그냥 점수에 따라 돈 생김.
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class ScoreManager : MonoBehaviour
//{
//    public static ScoreManager Instance;

//    private int score = 0;
//    private int money = 0;
//    public int scoreToMoneyRatio = 10; // 10점당 1원

//    private void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }
//    void Start()
//    {
//        UIManager.Instance.UpdateScoreText(0);
//        UIManager.Instance.UpdateMoneyText(0);
//    }
//    public void AddScore(int amount)
//    {
//        score += amount;
//        ConvertScoreToMoney();
//        UIManager.Instance.UpdateScoreText(score);
//        UIManager.Instance.UpdateMoneyText(money);
//    }

//    private void ConvertScoreToMoney()
//    {
//        int newMoney = score / scoreToMoneyRatio;
//        int moneyToAdd = newMoney - money;
//        money = newMoney;

//        if (moneyToAdd > 0)
//        {
//            Debug.Log($"플레이어가 {moneyToAdd}원을 얻었습니다!");
//        }
//    }

//    public int GetScore()
//    {
//        return score;
//    }

//    public int GetMoney()
//    {
//        return money;
//    }
//    public bool TryOpenDoor(int requiredMoney)
//    {
//        Debug.Log($"문 열기 시도: 필요 금액 = {requiredMoney}, 현재 돈 = {money}");
//        if (money >= requiredMoney)
//        {
//            money -= requiredMoney;
//            UIManager.Instance.UpdateMoneyText(money);

//            Debug.Log($"문이 열렸습니다! 남은 돈: {money}");
//            return true; // 문이 열리는 상태로 변경
//        }
//        else
//        {
//            Debug.Log("돈이 부족합니다. 문을 열 수 없습니다.");
//            return false;
//        }
//    }
//}

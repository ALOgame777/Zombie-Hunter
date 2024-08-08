// ���� ���������� �� ���̰�, ������ �Ȼ����.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private int score = 0;
    private int money = 0;
    private int totalMoneyEarned = 0; // �� ȹ���� ��
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
        Debug.Log($"�� ���� �õ�: �ʿ� �ݾ� = {requiredMoney1}, ���� �� = {money}");
        if (money >= requiredMoney1)
        {
            money -= requiredMoney1;
            UIManager.Instance.UpdateMoneyText(money);

            Debug.Log($"���� ���Ƚ��ϴ�! ���� ��: {money}");
            return true; // ���� ������ ���·� ����
        }
        else
        {
            Debug.Log("���� �����մϴ�. ���� �� �� �����ϴ�.");
            return false;
        }
    }
    public bool BuyDoubleShoot(int requiredMoney2 = 2000)
    {
        Debug.Log($"���� ���� �õ�: �ʿ� �ݾ� = {requiredMoney2}, ���� �� = {money}");
        if (money >= requiredMoney2)
        {
            money -= requiredMoney2;
            UIManager.Instance.UpdateMoneyText(money);

            Debug.Log($"���� ���Ƚ��ϴ�! ���� ��: {money}");
            return true; // ���� ������ ���·� ����
        }
        else
        {
            Debug.Log("���� �����մϴ�. ���� �� �� �����ϴ�.");
            return false;
        }
    }
    public bool BuyAK(int requiredMoney3 = 3000)
    {
        Debug.Log($"���� ���� �õ�: �ʿ� �ݾ� = {requiredMoney3}, ���� �� = {money}");
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
        Debug.Log($" ���׷��̵� ���� �õ�: �ʿ� �ݾ� = {requiredMoney5}, ���� �� = {money}");
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


//// ���� ���������� �� ���̰�, ������ �Ȼ����.
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class ScoreManager : MonoBehaviour
//{
//    public static ScoreManager Instance;

//    private int score = 0;
//    private int money = 0;
//    private int totalMoneyEarned = 0; // �� ȹ���� ��
//    public int scoreToMoneyRatio = 10; // 10���� 1��

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
//        Debug.Log($"�� ���� �õ�: �ʿ� �ݾ� = {requiredMoney}, ���� �� = {money}");
//        if (money >= requiredMoney)
//        {
//            money -= requiredMoney;
//            UIManager.Instance.UpdateMoneyText(money);

//            Debug.Log($"���� ���Ƚ��ϴ�! ���� ��: {money}");
//            return true; // ���� ������ ���·� ����
//        }
//        else
//        {
//            Debug.Log("���� �����մϴ�. ���� �� �� �����ϴ�.");
//            return false;
//        }
//    }
//}



//// �� ���������� ���� ��. �� �ᵵ ��������. �ٵ� ������ ���� �����ʤ���
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class ScoreManager : MonoBehaviour
//{
//    public static ScoreManager Instance;

//    private int score = 0;
//    private int money = 0;
//    public int scoreToMoneyRatio = 10; // 10���� 1��

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
//        Debug.Log($"�� ���� �õ�: �ʿ� �ݾ� = {requiredMoney}, ���� �� = {money}");
//        if (money >= requiredMoney)
//        {
//            // �ʿ��� ���� �ش��ϴ� ������ ����
//            int scoreToDeduct = requiredMoney * scoreToMoneyRatio;
//            score -= scoreToDeduct;

//            // ������ ������ �� ���� �ٽ� ���
//            ConvertScoreToMoney();
//            UIManager.Instance.UpdateScoreText(score);
//            UIManager.Instance.UpdateMoneyText(money);

//            Debug.Log($"���� ���Ƚ��ϴ�! ���� ��: {money}");
//            return true; // ���� ������ ���·� ����
//        }
//        else
//        {
//            Debug.Log("���� �����մϴ�. ���� �� �� �����ϴ�.");
//            return false;
//        }
//    }
//}



// ���� ������. �� �� ������ �� ���� �׳� ������ ���� �� ����.
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class ScoreManager : MonoBehaviour
//{
//    public static ScoreManager Instance;

//    private int score = 0;
//    private int money = 0;
//    public int scoreToMoneyRatio = 10; // 10���� 1��

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
//            Debug.Log($"�÷��̾ {moneyToAdd}���� ������ϴ�!");
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
//        Debug.Log($"�� ���� �õ�: �ʿ� �ݾ� = {requiredMoney}, ���� �� = {money}");
//        if (money >= requiredMoney)
//        {
//            money -= requiredMoney;
//            UIManager.Instance.UpdateMoneyText(money);

//            Debug.Log($"���� ���Ƚ��ϴ�! ���� ��: {money}");
//            return true; // ���� ������ ���·� ����
//        }
//        else
//        {
//            Debug.Log("���� �����մϴ�. ���� �� �� �����ϴ�.");
//            return false;
//        }
//    }
//}

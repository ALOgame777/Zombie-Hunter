using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public Text scoreText;
    public Text moneyText;
    public Text storeText;

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
    public void UpdateScoreText(int score)
    {
        scoreText.text = $"����: {score}";
    }

    public void UpdateMoneyText(int money)
    {
        moneyText.text = $"��: {money}��";
        storeText.text = $" {money}��";
    }
}

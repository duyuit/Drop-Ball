using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour
{
    public Text playerScoreText;
    public Text botScoreText;

    public static ScoreController Instance = null;

    private int _playerScore = 0;
    private int _botScore = 0;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance.gameObject);
            Instance = this;
        }
    }

    public void AddScore(Winner winner)
    {
        if (winner == Winner.BOT)
        {
            _botScore++;
            botScoreText.text = _botScore.ToString();
        }
        else
        {
            _playerScore++;
            playerScoreText.text = _playerScore.ToString();
        }
    }

}

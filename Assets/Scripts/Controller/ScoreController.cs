using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScoreController : MonoBehaviour
{
    public Text playerScoreText;
    public Text botScoreText;
    public Image targetCircle;

    public List<Image> listScoreImage;
    public Color winColor;
    public Color loseColor;

    public static ScoreController Instance = null;

    private int _playerScore = 0;
    private int _botScore = 0;

    private int _playerWinRound = 0;
    private int _botWinRound = 0;
    private int _currentRound = 1;
    public int CurrentRound { get { return _currentRound; } }

    private DelayFunctionHelper _delayHelper;


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

        _delayHelper = gameObject.AddComponent<DelayFunctionHelper>();
    }

    public void AddScore(PlayerTag winner)
    {
        if (winner == PlayerTag.BOT)
        {
            _botScore++;
            botScoreText.text = _botScore.ToString();
        }
        else
        {
            _playerScore++;
            playerScoreText.text = _playerScore.ToString();
        }


        if (_botScore == 5 || _playerScore == 5)
        {
            Image currentRoundImage = listScoreImage[_currentRound - 1];
            Color color;

            if (winner == PlayerTag.BOT)
            {
                _botWinRound++;
                color = loseColor;
            }
            else
            {
                _playerWinRound++;
                color = winColor;
            }

            _currentRound++;
            if (_playerWinRound >= 3 || _botWinRound >= 3)
            {
                GameController.Instance.EndGame(winner);
                targetCircle.color = color;
                targetCircle.transform.position = currentRoundImage.transform.position;
                targetCircle.transform.localScale = new Vector3(10, 10, 1);
                targetCircle.transform.DOScale(0, 1f).OnComplete(() =>
                {
                    currentRoundImage.color = color;
                    currentRoundImage.transform.localScale = Vector3.zero;
                    currentRoundImage.transform.DOScale(Vector3.one, 0.5f);
                });
                return;
            }

            GameController.Instance.EndRound(winner, () =>
            {
                targetCircle.color = color;
                targetCircle.transform.position = currentRoundImage.transform.position;
                targetCircle.transform.localScale = new Vector3(10, 10, 1);
                targetCircle.transform.DOScale(0, 1f).OnComplete(() =>
                {
                    currentRoundImage.color = color;
                    currentRoundImage.transform.localScale = Vector3.zero;
                    currentRoundImage.transform.DOScale(Vector3.one, 0.5f).OnComplete(() =>
                    {
                        GameController.Instance.ShowText("Round " + _currentRound);
                    });
                });


                _botScore = _playerScore = 0;
                botScoreText.text = playerScoreText.text = "0";
            });
        }
    }


}

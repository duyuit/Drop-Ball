using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public enum Winner
{
    PLAYER,
    BOT,
    NONE
}

public class GameController : MonoBehaviour
{
    public static GameController Instance = null;
    public Text botRemainText;
    public Text cleanTimeText;
    public Player player;
    public Ball ball;

    [HideInInspector]
    public bool isStarted = false;

    public GameObject losePanel;
    public GameObject winPanel;
    public GameObject winPS;

    private List<Bot> _listBot;
    private int _totalBot = 0;

    private List<MyBlock> _damageBlocks;
    private List<MyBlock> _recoverBlocks;

    private DelayFunctionHelper _delayHelper;
    public Winner currentWinner;


    private List<string> _winText;
    private List<string> _loseText;


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

    private void ShowWinnerText(Winner winner)
    {
        if (winner == Winner.PLAYER)
        {
            ShowText(_winText[UnityEngine.Random.Range(0,_winText.Count)]);
        }
        else
        {
            ShowText(_loseText[UnityEngine.Random.Range(0, _loseText.Count)]);
        }
    }

    public void SetWinner(Winner winner)
    {
        currentWinner = winner;
        ScoreController.Instance.AddScore(currentWinner);
        Reset();
    }
    public void Reset()
    {
        ShowWinnerText(currentWinner);
        _delayHelper.delayFunction(() =>
        {
            foreach (var bot in _listBot)
            {
                bot.Reset();
            }
            player.Reset();

            ball.Reset(currentWinner);

            isStarted = false;
        }, 1f);

    }

    private void Start()
    {
        _listBot = new List<Bot>();
        foreach (var bot in FindObjectsOfType<Bot>())
        {
            _listBot.Add(bot);
        }

        _winText = new List<string>();
        _loseText = new List<string>();

        _winText.Add("YEAH!!");
        _winText.Add("BOOYAH!!");
        _winText.Add("AHAHA!!");
        _winText.Add("HOORAYY!!");

        _loseText.Add("OOPS!");
        _loseText.Add("AGAIN!");
        _loseText.Add("****!");
        _loseText.Add("HM . . .");
    }

    public void ShowText(string text)
    {
        cleanTimeText.text = text;
        cleanTimeText.transform.DOScale(1, .5f).SetEase(Ease.OutBounce);
        _delayHelper.delayFunction(() =>
        {
            cleanTimeText.transform.DOScale(0, .5f);
        }, 1f);
    }

    public void SetLastBlock(MyBlock blockComp)
    {
        Vector2Int index = blockComp.index;
        if (index.y < 15)
            currentWinner = Winner.BOT;
        else
            currentWinner = Winner.PLAYER;
    }

    public void DropBall()
    {
        Winner winner = ball.GetWinner();
        // Ball out before touch
        if (winner != Winner.NONE)
        {
            currentWinner = winner;
        }

        ScoreController.Instance.AddScore(currentWinner);
    }

    public void Lose()
    {
        losePanel.SetActive(true);
    }

    public void Win()
    {
        //player.Win();

        _delayHelper.delayFunction(() =>
        {
            GrassController.Instance.GeneratePlan();
        }, 1f);

        _delayHelper.delayFunction(() =>
        {
            winPanel.SetActive(true);
            winPS.SetActive(true);

        }, 2f);
    }
}

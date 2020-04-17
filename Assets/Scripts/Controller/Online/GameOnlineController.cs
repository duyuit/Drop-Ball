using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;


public class GameOnlineController : MonoBehaviour
{
    public static GameOnlineController Instance = null;

    [HideInInspector]
    public Player.Player player;
    public Ball ball;
    public Transform cameraParent;

    [HideInInspector] public bool isStarted = false;
    [HideInInspector] public bool isWaiting = false;
    [HideInInspector] public bool isWaitingNewRound = false;
    [HideInInspector] public bool isEndGame = false;

    public Text botRemainText;
    public Text cleanTimeText;

    public GameObject losePanel;
    public GameObject winPanel;
    public GameObject winPS;

    private List<Bot> _listBot;
    private int _totalBot = 0;

    private List<MyBlock> _damageBlocks;
    private List<MyBlock> _recoverBlocks;

    private DelayFunctionHelper _delayHelper;
    public PlayerTag currentWinner;


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

    private void ShowWinnerText(PlayerTag winner)
    {
        if (winner == PlayerTag.PLAYER)
        {
            ShowText(_winText[UnityEngine.Random.Range(0, _winText.Count)]);
        }
        else
        {
            ShowText(_loseText[UnityEngine.Random.Range(0, _loseText.Count)]);
        }
    }

    public void SetWinner(PlayerTag winner)
    {
        currentWinner = winner;
        isStarted = false;
        isWaiting = true;
        ScoreController.Instance.AddScore(currentWinner);

        bool isBotWin = currentWinner == PlayerTag.BOT;
        foreach (var bot in _listBot)
        {
            if (isBotWin)
                bot.Win();
            else
                bot.Lose();
        }

        if (isBotWin)
        {
            player.Lose();
            SoundController.Instance.PlayLoseSound();
        }
        else
        {
            player.Win();
            SoundController.Instance.PlayWinSound();
        }

        Reset();
    }

    public void Reset(bool showWinnerText = true)
    {
        if (isWaitingNewRound || isEndGame)
        {
            return;
        }

        if (showWinnerText)
            ShowWinnerText(currentWinner);

        _delayHelper.delayFunction(() =>
        {
            foreach (var bot in _listBot)
            {
                bot.Reset();
            }

            player.Reset();

            ball.Reset(currentWinner);
            isWaiting = false;
        }, 2f);
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

        isWaiting = true;
        ShowText("Round " + 1, 2f);
        _delayHelper.delayFunction(() =>
        {
            isWaiting = false;
        }, 3f);

        InitPlayerAndOpponent();
    }

    public void InitPlayerAndOpponent()
    {
        player = Instantiate(PrefabController.Instance.playerPrefab).GetComponent<Player.Player>();
        var opponent = Instantiate(PrefabController.Instance.opponentPrefab);

        Vector3 myPosition = new Vector3(0, 6.11f, -8.8f);
        Vector3 opponentPosition = new Vector3(0, 6.11f, 8.8f);

        if (GlobalVariable.myIndex == 1)
        {
            player.transform.position = myPosition;
            opponent.transform.position = opponentPosition;
        }
        else
        {
            player.transform.position = opponentPosition;
            opponent.transform.position = myPosition;

            opponent.transform.localEulerAngles = Vector3.zero;
            player.transform.localEulerAngles = new Vector3(0, 180, 0);
            cameraParent.localEulerAngles = new Vector3(0, 180, 0);
        }
    }

    public void ShowText(string text, float time = 1f)
    {
        cleanTimeText.text = text;
        cleanTimeText.transform.DOScale(1, .5f).SetEase(Ease.OutBounce);
        _delayHelper.delayFunction(() => { cleanTimeText.transform.DOScale(0, .5f); }, time);
    }

    public void SetLastRange(PlayerTag tag)
    {
        if (tag == PlayerTag.BOT)
            currentWinner = PlayerTag.PLAYER;
        else
            currentWinner = PlayerTag.BOT;
    }

    public void DropBall()
    {
        PlayerTag winner = ball.GetWinner();
        // Ball out before touch
        if (winner != PlayerTag.NONE)
        {
            SetWinner(winner);
        }
        else
        {
            SetWinner(currentWinner);
        }
    }

    public void Lose()
    {
        losePanel.SetActive(true);
    }

    public void Win()
    {
        _delayHelper.delayFunction(() =>
        {
            winPanel.SetActive(true);
            winPS.SetActive(true);
        }, 2f);
    }

    public void WinTurn()
    {
        SetWinner(PlayerTag.PLAYER);
    }

    public void LoseTurn()
    {
        SetWinner(PlayerTag.BOT);
    }

    public void EndRound(PlayerTag playerTag, Action callBack)
    {
        isWaitingNewRound = true;
        isWaiting = true;

        if (playerTag == PlayerTag.PLAYER)
        {
            ShowText("You win", 2f);
        }
        else
        {
            ShowText("You lose", 2f);
        }

        _delayHelper.delayFunction(() =>
        {
            callBack?.Invoke();

            if (isEndGame)
                return;

            isWaiting = false;
            isWaitingNewRound = false;

            _delayHelper.delayFunction(() =>
            {
                Reset(false);
            }, 1f);
        }, 3);
    }

    public void EndGame(PlayerTag winner)
    {
        Debug.Log("End game");

        isWaiting = true;
        isEndGame = true;
        _delayHelper.delayFunction(() =>
        {
            if (winner == PlayerTag.PLAYER)
            {
                winPS.SetActive(true);
                winPanel.SetActive(true);
            }
            else
            {
                losePanel.SetActive(true);
            }
        }, 3f);

    }
}
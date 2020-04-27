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
    [HideInInspector]
    public Opponent opponent;

    public Ball ball;
    public Transform cameraParent;

    [HideInInspector] public bool isStarted = false;
    [HideInInspector] public bool isWaiting = false;
    [HideInInspector] public bool isWaitingNewRound = false;
    [HideInInspector] public bool isEndGame = false;

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
        if (IsMe(winner))
            ShowText(_winText[UnityEngine.Random.Range(0, _winText.Count)]);
        else
            ShowText(_loseText[UnityEngine.Random.Range(0, _loseText.Count)]);
    }

    public void SetWinner(PlayerTag winner)
    {
        currentWinner = winner;
        isStarted = false;
        isWaiting = true;
        Debug.Log("Current winner" + currentWinner);
        //ScoreController.Instance.AddScore(currentWinner);

        if (IsMe(winner))
        {
            player.Win();
            opponent.Lose();

            NetworkController.Instance.HasWin();
        }
        else
        {
            player.Lose();
            opponent.Win();
        }

        //Reset();
    }

    public bool IsMe(PlayerTag playerTag)
    {
        return playerTag == PlayerTag.PLAYER1 && GlobalVariable.myIndex == 1 ||
               playerTag == PlayerTag.PLAYER2 && GlobalVariable.myIndex == 2;
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
            player.Reset();
            opponent.Reset();
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

        NetworkController.Instance.onReset += OnReset;
        InitPlayerAndOpponent();
    }

    private void OnDestroy()
    {
        NetworkController.Instance.onReset -= OnReset;
    }

    private void OnReset(PlayerTag winner, bool isShowText)
    {
        currentWinner = winner;
        ScoreController.Instance.AddScore(winner);

        Reset();
    }
    public void InitPlayerAndOpponent()
    {
        player = Instantiate(PrefabController.Instance.playerPrefab).GetComponent<Player.Player>();
        opponent = Instantiate(PrefabController.Instance.opponentPrefab).GetComponent<Opponent>();

        Vector3 myPosition = new Vector3(0, 6.11f, -8.8f);
        Vector3 opponentPosition = new Vector3(0, 6.11f, 8.8f);

        if (GlobalVariable.myIndex == 1)
        {
            player.transform.position = myPosition;
            opponent.transform.position = opponentPosition;
            player.GetComponentInChildren<BallCatcher>().playerTag = PlayerTag.PLAYER1;
            opponent.GetComponentInChildren<BallCatcher>().playerTag = PlayerTag.PLAYER2;
        }
        else
        {
            player.transform.position = opponentPosition;
            opponent.transform.position = myPosition;

            opponent.transform.localEulerAngles = Vector3.zero;
            player.transform.localEulerAngles = new Vector3(0, 180, 0);
            cameraParent.localEulerAngles = new Vector3(0, 180, 0);

            player.GetComponentInChildren<BallCatcher>().playerTag = PlayerTag.PLAYER2;
            opponent.GetComponentInChildren<BallCatcher>().playerTag = PlayerTag.PLAYER1;
        }

        player.UpdateOriginPos();
        opponent.UpdateOriginPos();

    }

    public void ShowText(string text, float time = 1f)
    {
        cleanTimeText.text = text;
        cleanTimeText.transform.DOScale(1, .5f).SetEase(Ease.OutBounce);
        _delayHelper.delayFunction(() => { cleanTimeText.transform.DOScale(0, .5f); }, time);
    }

    public void SetLastRange(PlayerTag tag)
    {
        if (tag == PlayerTag.PLAYER1)
            currentWinner = PlayerTag.PLAYER2;
        else
            currentWinner = PlayerTag.PLAYER1;

    }

    public void DropBall()
    {
        PlayerTag winner = ball.GetWinner();
        // Ball out before touch
        if (winner != PlayerTag.NONE)
        {
            SetWinner(winner);
            Debug.Log("Winner not cut" + winner);
        }
        else
        {
            SetWinner(currentWinner);
        }

        if (IsMe(currentWinner))
        {
            Debug.Log("Notify i am winner");
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

        if (IsMe(playerTag))
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
            if (IsMe(winner))
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
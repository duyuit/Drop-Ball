using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public BallBouncer ballBouncer;
    public BallCutter ballCutter;

    private BallCatcher _playerCatcher;
    private BallCatcher _opponentCatcher;
    private List<BallCatcher> _botCatcher;

    private BallCatcher _holder;
    private BallCatcher _lastHolder;

    private float _lastSendUpdatePositionTime = 0;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);

        _botCatcher = new List<BallCatcher>();
        foreach (var catcher in FindObjectsOfType<Bot>())
        {
            var catcherComp = catcher.GetComponentInChildren<BallCatcher>();
            catcherComp.onFire += ballCutter.ResetHasCut;

            _botCatcher.Add(catcherComp);
        }

        _playerCatcher = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<BallCatcher>();
        _playerCatcher.onFire += ballCutter.ResetHasCut;

        if (GlobalVariable.isOnline)
        {
            _opponentCatcher = GameObject.FindGameObjectWithTag("Opponent").GetComponentInChildren<BallCatcher>();

            NetworkController.Instance.onBallMove += UpdateBallPosition;
            if (GlobalVariable.myIndex == 1)
            {
                SetCatcher(_playerCatcher);
            }
            else
            {
                SetCatcher(_opponentCatcher);
            }
        }
        else
        {
            SetCatcher(_playerCatcher);
        }
    }

    private void OnDestroy()
    {
        NetworkController.Instance.onBallMove -= UpdateBallPosition;
    }

    private void Update()
    {
        if (_holder == null && GlobalVariable.isOnline && GlobalVariable.myIndex == 1)
        {
            if (Time.time - _lastSendUpdatePositionTime > 0.05f)
            {
                NetworkController.Instance.SendBallPosition(transform.position);
                _lastSendUpdatePositionTime = Time.time;
            }
        }
    }
    private void UpdateBallPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    public void SetHolder(BallCatcher newPlayer)
    {
        _holder = newPlayer;
    }

    public bool HasHolder()
    {
        return _holder != null;
    }

    public void Release()
    {
        _lastHolder = _holder;
        _holder = null;
    }

    public void Reset(PlayerTag winner)
    {
        transform.position = new Vector3(0, 10, 0);
        //ballBouncer.SetVelocity(new Vector3(UnityEngine.Random.Range(-3, 3), 0, UnityEngine.Random.Range(10f, 12f)));

        if (GlobalVariable.isOnline)
        {
            if (GameOnlineController.Instance.IsMe(winner))
            {
                SetCatcher(_playerCatcher);
            }
            else
            {
                SetCatcher(_opponentCatcher);
            }
        }
        else
        {
            if (winner == PlayerTag.BOT)
            {
                SetCatcher(_botCatcher[UnityEngine.Random.Range(0, _botCatcher.Count)]);
            }
            else
            {
                SetCatcher(_playerCatcher);
            }
        }

    }

    private void SetCatcher(BallCatcher catcher)
    {
        catcher.TakeBall(this);

        var newPos = _playerCatcher.transform.position;
        transform.position = newPos;
    }

    public PlayerTag GetWinner()
    {
        PlayerTag winner = PlayerTag.NONE;

        if (!ballCutter.hasCut)
        {
            Debug.Log("Has not cut");
            if (GlobalVariable.isOnline)
            {
                if (_lastHolder == null)
                    _lastHolder = _holder;

                if (_lastHolder != null)
                {
                    if (_lastHolder.playerTag == PlayerTag.PLAYER1)
                        winner = PlayerTag.PLAYER2;
                    else
                        winner = PlayerTag.PLAYER1;
                }
            }
            else
            {
                if (_lastHolder != null && _lastHolder.isBot)
                {
                    winner = PlayerTag.PLAYER;
                }
                else
                {
                    winner = PlayerTag.BOT;
                }
            }
        }

        _lastHolder = null;
        _holder = null;

        return winner;
    }
}

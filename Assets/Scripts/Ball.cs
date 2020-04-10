using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public BallBouncer ballBouncer;
    public BallCutter ballCutter;

    public BallCatcher playerCatcher;
    private List<BallCatcher> _botCatcher;

    private BallCatcher _holder;
    private BallCatcher _lastHolder;

    private void Start()
    {
        _botCatcher = new List<BallCatcher>();
        foreach (var catcher in FindObjectsOfType<Bot>())
        {
            var catcherComp = catcher.GetComponentInChildren<BallCatcher>();
            catcherComp.onFire += ballCutter.ResetHasCut;

            _botCatcher.Add(catcherComp);
        }

        SetCatcher(playerCatcher);
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

    public void Reset(Winner winner)
    {
        transform.position = new Vector3(0, 10, 0);
        ballBouncer.SetVelocity(new Vector3(UnityEngine.Random.Range(-3, 3), 0, UnityEngine.Random.Range(10f, 12f)));

        if (winner == Winner.BOT)
        {
            SetCatcher(_botCatcher[UnityEngine.Random.Range(0,_botCatcher.Count)]);
        }
        else
        {
            SetCatcher(playerCatcher);
        }
    }

    private void SetCatcher(BallCatcher catcher)
    {
        catcher.TakeBall(this);

        var newPos = playerCatcher.transform.position;
        transform.position = newPos;
    }

    public Winner GetWinner()
    {
        if (!ballCutter.hasCut)
        {
            Debug.Log("Has cut");
            if (_lastHolder.isBot)
            {
                return Winner.PLAYER;
            }
            else
            {
                return Winner.BOT;
            }
        }
        return Winner.NONE;
    }
}

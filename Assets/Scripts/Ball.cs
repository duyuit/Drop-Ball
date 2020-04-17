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

        playerCatcher.onFire += ballCutter.ResetHasCut;
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

    public void Reset(PlayerTag winner)
    {
        transform.position = new Vector3(0, 10, 0);
        //ballBouncer.SetVelocity(new Vector3(UnityEngine.Random.Range(-3, 3), 0, UnityEngine.Random.Range(10f, 12f)));

        if (winner == PlayerTag.BOT)
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

    public PlayerTag GetWinner()
    {
        if (!ballCutter.hasCut)
        {
            Debug.Log("Has not cut");
            if (_lastHolder != null && _lastHolder.isBot)
            {
                Debug.Log("Last holder is bot");
                return PlayerTag.PLAYER;
            }
            else
            {
                Debug.Log("Last holder is player");
                return PlayerTag.BOT;
            }
        }
        return PlayerTag.NONE;
    }
}

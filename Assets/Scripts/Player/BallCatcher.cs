﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class BallCatcher : MonoBehaviour
{
    public bool isBot = false;
    public Transform arrowParent;
    public Transform arrow;
    public Action onFire;
    public bool isMe = false;

    [HideInInspector]
    public PlayerTag playerTag;

    private Ball _ball;
    private DelayFunctionHelper _delay;
    private Rigidbody _ballRB;

    private float _lastFireTime = 0;
    private float _lastCatchTime = 0;
    private bool _isCatched = false;


    // Start is called before the first frame update
    void Start()
    {
        _delay = gameObject.AddComponent<DelayFunctionHelper>();

        if (isMe)
            InputController.Instance.onMouseRelease += Fire;

        if (GlobalVariable.isOnline)
        {
            NetworkController.Instance.onFireBall += Fire;
        }
        //RotateArrow();
    }

    //private void RotateArrow()
    //{
    //    arrowParent.DOLocalRotate(new Vector3(90, -30, 0), 1).SetEase(Ease.Linear).OnComplete(() =>
    //    {
    //        arrowParent.DOLocalRotate(new Vector3(90, 30, 0), 1).SetEase(Ease.Linear).OnComplete(() => RotateArrow());
    //    });
    //}

    public void Fire(Vector3 forceVector)
    {
        if (_ball != null && _isCatched)
        {
            arrowParent.gameObject.SetActive(false);
            arrow.localScale = Vector3.one;

            _ball.transform.parent = null;

            _ball.GetComponent<Collider>().enabled = true;

            _ballRB.isKinematic = false;
            _ballRB.AddForce(forceVector);

            _ball.GetComponent<Ball>().Release();

            _ballRB = null;
            _ball = null;

            _lastFireTime = Time.time;

            GameController.Instance.isStarted = true;
            SoundController.Instance.PlayFireSound();
            onFire?.Invoke();
            _isCatched = false;
        }
    }

    public void TakeBall(Ball ball)
    {
        arrowParent.gameObject.SetActive(true);
        arrow.DOScaleY(5, 2f);

        _ball = ball;

        _ballRB = _ball.GetComponent<Rigidbody>();
        _ballRB.isKinematic = true;
        _ball.GetComponent<Collider>().enabled = false;

        ball.transform.parent = transform;
        ball.transform.DOLocalMove(Vector3.zero, 0.2f).OnComplete(() =>
        {
            _isCatched = true;
        });

        _lastCatchTime = Time.time;
        _ball.GetComponent<Ball>().SetHolder(this);

        if (isBot)
        {
            _delay.delayFunction(() => { Fire(); }, UnityEngine.Random.Range(1f, 2f));
        }
        else
        {
            if (isMe)
                _delay.delayFunction(() => { Fire(); }, 1.5f);
            //Vibration.Vibrate(100);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Time.time - _lastFireTime > 0.5f && other.tag == "Ball")
        {
            Ball tempBall = other.GetComponent<Ball>();
            if (!tempBall.HasHolder() && tempBall != null && tempBall != _ball)
            {
                TakeBall(tempBall);

                if (isBot)
                {
                    _delay.delayFunction(() => { Fire(); }, UnityEngine.Random.Range(0.5f, 1.3f));
                }
                //else if (!InputController.Instance.IsTap)
                //    InstantFire();
            }
        }
    }

    public void Reset()
    {
        _ball = null;
        _isCatched = false;
        arrowParent.gameObject.SetActive(false);
    }

    private void Fire()
    {
        if (_ball != null && _isCatched)
        {
            arrowParent.gameObject.SetActive(false);
            arrow.localScale = Vector3.one;

            float force = (Time.time - _lastCatchTime) / Time.deltaTime;
            force *= 50;
            force = Mathf.Clamp(force, 1000, 1600);

            _ball.transform.parent = null;

            _ball.GetComponent<Collider>().enabled = true;

            _ballRB.isKinematic = false;
            Vector3 forceVector = arrowParent.up.normalized * force + Vector3.up * force / 3f;
            _ballRB.AddForce(forceVector);

            _ball.GetComponent<Ball>().Release();


            _lastFireTime = Time.time;

            if (GlobalVariable.isOnline)
                GameOnlineController.Instance.isStarted = true;
            else
                GameController.Instance.isStarted = true;

            SoundController.Instance.PlayFireSound();
            onFire?.Invoke();
            _isCatched = false;

            if (GlobalVariable.isOnline)
            {
                Debug.Log(_ball);
                NetworkController.Instance.SendBallPosition(_ball.transform.position);
                NetworkController.Instance.SendFireBall(forceVector);
            }

            _ballRB = null;
            _ball = null;
        }
    }

    public bool HasBall()
    {
        return _ball != null;
    }

    private void InstantFire()
    {
        if (_ball != null)
        {
            arrowParent.gameObject.SetActive(false);
            arrow.localScale = Vector3.one;

            float force = 1000;

            _ball.transform.parent = null;

            _ball.GetComponentInChildren<BallCutter>().enabled = false;

            _ballRB.isKinematic = false;
            _ballRB.AddForce(arrowParent.up.normalized * force);
            _ballRB.AddForce(Vector3.up * force / 3f);

            _ball.GetComponent<Ball>().Release();
            _ballRB = null;
            _ball = null;

            _lastFireTime = Time.time;
            onFire?.Invoke();

        }
    }

}

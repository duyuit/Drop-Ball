using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public enum BotState
{
    RUN,
    IDLE
}
public class Bot : Player.Player
{
    public bool isSuperBot = false;

    Vector2 _currentMoveDirection;
    float _lastChangeDirection = 0;
    float _delayEachAI = 0;
    private BotState _currentState = BotState.IDLE;

    private Ball _ball;
    private bool _isCatching = false;
    private float _lastCatched = 0;

    private void Start()
    {
        _delayEachAI = Random.Range(0.2f, 0.7f);

        _animator.SetTrigger("Run");

        if (isSuperBot)
        {
            _ball = FindObjectOfType<Ball>();
        }
    }

    protected override void FixedUpdate()
    {
        if(!GameController.Instance.isStarted)
        {
            return;
        }
        

        if (isSuperBot &&
            _ball.transform.position.z > -2f &&
            Time.time - _lastCatched > 3f &&
            !_ball.HasHolder())
        {
            Catch();
            return;
        }

        if (Time.time - _lastChangeDirection > _delayEachAI)
        {
            DoAI();
        }

        //if (_currentState == BotState.RUN)
        //{
        var newPos = transform.position + new Vector3(_currentMoveDirection.x, 0, _currentMoveDirection.y) * Time.fixedDeltaTime * speed;
        if (newPos.x > 10)
        {
            _currentMoveDirection.x = Random.Range(-1f, 0f);
        }
        if (newPos.x < -10)
        {
            _currentMoveDirection.x = Random.Range(0f, 1f);
        }

        if (newPos.z < 1)
        {
            _currentMoveDirection.y = Random.Range(0f, 1f);
        }

        if (newPos.z > 14)
        {
            _currentMoveDirection.y = Random.Range(-1f, 0);
        }

        transform.position = newPos;
        Vector3 movement = new Vector3(_currentMoveDirection.x, 0.0f, _currentMoveDirection.y);
        dustPS.transform.rotation = Quaternion.LookRotation(movement);
        //    _animator.SetTrigger("Run");
        //}
        //else
        //{
        //    _animator.SetTrigger("Idle");
        //}

    }

    private void Catch()
    {
        var direction = (_ball.transform.position - transform.position).normalized;
        direction.y = 0;
        transform.position += direction * speed * Time.fixedDeltaTime;

        if (Vector3.Distance(transform.position, _ball.transform.position) < 1f)
        {
            _lastCatched = Time.time;
        }
    }

    private void DoAI()
    {
        _currentMoveDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        _lastChangeDirection = Time.time;
        _delayEachAI = Random.Range(0.2f, 0.7f);
    }

}

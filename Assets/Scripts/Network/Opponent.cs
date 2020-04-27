using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opponent : Player.Player
{
    private Vector2 _velocity;
    private void Start()
    {
        _animator = GetComponent<Animator>();

        NetworkController.Instance.onOpponentMove += UpdatePosition;
        NetworkController.Instance.onOpponentVelo += UpdateVelo;
    }

    private void UpdateVelo(Vector2 velo)
    {
        _velocity = velo;
    }

    public void UpdatePosition(Vector3 pos)
    {
        transform.position = pos;
    }

    protected override void FixedUpdate()
    {
        if (!GameOnlineController.Instance.isWaiting)
            transform.position += new Vector3(_velocity.x, 0, _velocity.y) * Time.fixedDeltaTime * 10;
    }

    private void OnDestroy()
    {
        NetworkController.Instance.onOpponentMove -= UpdatePosition;
        NetworkController.Instance.onOpponentVelo -= UpdateVelo;
    }

}

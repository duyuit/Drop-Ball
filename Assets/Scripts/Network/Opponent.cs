using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opponent : MonoBehaviour
{
    private Vector3 velocity;
    private void Start()
    {
        NetworkController.Instance.onOpponentMove += UpdatePosition;
    }

    public void UpdatePosition(Vector3 pos)
    {
        velocity = pos;
    }

    private void Update()
    {
        transform.position += new Vector3(velocity.x, 0, velocity.y) * Time.fixedDeltaTime * 10;
    }

    private void OnDestroy()
    {
        NetworkController.Instance.onOpponentMove -= UpdatePosition;
    }


}

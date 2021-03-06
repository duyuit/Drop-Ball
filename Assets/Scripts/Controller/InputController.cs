﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InputController : MonoBehaviour
{
    public static InputController Instance = null;
    public Action onMouseRelease;

    private bool _isTap = false;
    public bool IsTap { get { return _isTap; } }

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
    }


    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        //if (GameController.Instance.isWaiting)
        //    return;

        if (GlobalVariable.isOnline)
        {
            if (GameOnlineController.Instance.isWaiting)
                return;
        }
        else
        {
            if (GameController.Instance.isWaiting)
                return;
        }

        if (Input.GetMouseButtonDown(0))
            _isTap = true;

        if (Input.GetMouseButtonUp(0))
        {
            _isTap = false;
            onMouseRelease?.Invoke();
        }
    }
}

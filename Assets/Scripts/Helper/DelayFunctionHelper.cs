using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DelayFunctionHelper : MonoBehaviour
{
    private Action _action;
    private float _delayTime;
    public DelayFunctionHelper(Action action, float delayTime)
    {
        _action = action;
        _delayTime = delayTime;
    }
    public void StartFunction()
    {
        delayFunction(_action,_delayTime);
    }
    // Run a function after wait delayTime
    public void delayFunction(Action action, float delayTime)
    {
        StartCoroutine(delayFunctionIEumerator(action, delayTime));
    }
    IEnumerator delayFunctionIEumerator(Action action, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }
}

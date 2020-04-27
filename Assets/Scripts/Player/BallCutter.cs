using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCutter : MonoBehaviour
{
    public AwakeCircleClipper clipper;
    public AwakeCircleClipper clipper1;
    private DelayFunctionHelper _delay;

    public bool hasCut = false;

    private void Awake()
    {
        _delay = gameObject.AddComponent<DelayFunctionHelper>();
    }

    public void Cut()
    {
        //clipper.Cut();
        //clipper1.Cut();
        hasCut = true;
    }

    public void ResetHasCut()
    {
        hasCut = false;
    }

}

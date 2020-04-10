using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCutter : MonoBehaviour
{
    private AwakeCircleClipper _clipper;
    private DelayFunctionHelper _delay;

    public bool hasCut = false;

    private void Awake()
    {
        _clipper = GetComponent<AwakeCircleClipper>();
        _delay = gameObject.AddComponent<DelayFunctionHelper>();
    }

    public void Cut(GameObject block)
    {
        //_delay.delayFunction(() =>
        //{
        MyBlock blockComp = block.GetComponent<MyBlock>();
        MapGenerator.Instance.Cut(blockComp, _clipper);
        GameController.Instance.SetLastBlock(blockComp);

        hasCut = true;
        //}, 0.01f);
    }

    public void ResetHasCut()
    {
        hasCut = false;
    }

}

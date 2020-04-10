using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ColorBG : MonoBehaviour
{
    private Material _material;
    // Start is called before the first frame update
    void Start()
    {
        _material = GetComponent<MeshRenderer>().material;
        ChangeColor();
    }

    void ChangeColor()
    {
        Color color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
        Color color1 = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));

        _material.DOColor(color, "_Color", 20);
        _material.DOColor(color1, "_Color1", 20).OnComplete(() =>
        {
            ChangeColor();
        });
    }
}

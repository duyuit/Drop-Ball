using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StarItem : MonoBehaviour
{
    public MeshRenderer starMeshRenderer;

    private Material _starMaterial;
    // Start is called before the first frame update
    void Start()
    {
        _starMaterial = starMeshRenderer.material;
        Fade();
    }

    void Fade()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(_starMaterial.DOFade(0.3f, 0.7f));
        sequence.Append(_starMaterial.DOFade(1f, 0.7f));
        sequence.OnComplete(() => Fade());
    }
}

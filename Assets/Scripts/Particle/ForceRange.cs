using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class ForceRange : MonoBehaviour
{
    private MeshRenderer _meshRenderer;

    public void SetForce(float explosionRange)
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        transform.DOScale(explosionRange + 3f, .5f);

        _meshRenderer.material.DOFloat(0, "_FresnelWidth", .5f).OnComplete(
            () => Destroy(gameObject));
    }
}

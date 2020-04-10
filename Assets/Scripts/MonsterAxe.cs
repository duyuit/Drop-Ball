using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class MonsterAxe : MonoBehaviour
{
    public AwakeCircleClipper clipper;
    public ParticleSystem grassPS;

    private bool _isDestroy = false;
    private Rigidbody _rb;

    private List<Collider> colliders = new List<Collider>();

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ground")
        {
            _rb.isKinematic = true;
        }

        if (other.gameObject.tag == "Grass")
        {
            if (!grassPS.isPlaying)
                grassPS.Play();

            if (!colliders.Contains(other))
            {
                clipper.terrain = other.GetComponent<MyBlock>();
                clipper.Cut();
                colliders.Add(other);
            }

            if (!_isDestroy)
            {
                transform.DOScale(0, 2f).OnComplete(() =>
                {
                    Destroy(gameObject);
                });
                _isDestroy = true;
            }
        }


    }


    public void Fly(Vector3 direction)
    {
        float force = UnityEngine.Random.Range(10f, 15f);
        GetComponent<Rigidbody>().velocity = new Vector3(direction.x * force, UnityEngine.Random.Range(10f, 15f), direction.z * force);
    }
}

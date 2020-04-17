using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class BallBouncer : MonoBehaviour
{

    public BallCutter ballCutter;
    public GameObject forceRangePrefab;
    public ParticleSystem bouncePS;

    [SerializeField]
    [Tooltip("Just for debugging, adds some velocity during OnEnable")]
    private Vector3 initialVelocity;

    [SerializeField]
    private float minVelocity = 10f;

    private Vector3 lastFrameVelocity;
    private float _delayGenerateForce = 0.2f;
    private float _lastBounceTime = 0;

    private float _explosionRange = 3f;

    private DelayFunctionHelper _delayHelper;
    private Rigidbody rb;
    private Animator _animator;
    private Collider _collider;


    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _animator = GetComponent<Animator>();
        _delayHelper = gameObject.AddComponent<DelayFunctionHelper>();
    }
    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = initialVelocity;
    }

    public void SetVelocity(Vector3 velo)
    {
        initialVelocity = velo;
        rb.velocity = velo;
        //rb.AddTorque(velo* 100, ForceMode.Force);
    }

    private void Update()
    {
        lastFrameVelocity = rb.velocity;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && Time.time - _lastBounceTime > _delayGenerateForce)
        {
            //Vibration.VibratePop();
            Bounce(collision.contacts[0].normal);
            ballCutter.Cut();
            bouncePS.Play();
        }
    }

    public void AddForce(Vector3 force)
    {
        rb.AddForce(force);
    }

    public void Bounce(Vector3 collisionNormal)
    {
        //_originFixedTimeScale = Time.fixedDeltaTime;

        //Time.timeScale = 0.2f;
        //Time.fixedDeltaTime = Time.timeScale * 0.02f;

        //trampoline.Play("Bounce");
        CameraController.Instance.Shake();
        SoundController.Instance.PlayBounceSound();

        _animator.Play("Bounce");
        //_delayHelper.delayFunction(() =>
        //{
        //    rb.AddForce(new Vector3(initialVelocity.x * 100, 200, initialVelocity.z * 100));
        //    //Debug.Log(direction);

        //}, 0.04f);

        _lastBounceTime = Time.time;
    }

}

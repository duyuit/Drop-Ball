using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    public float speed = 10;
    public ParticleSystem dustPS;
    public Joystick joyStick;
    protected Animator _animator;

    private Rigidbody _rb;
    protected Vector3 _originPos;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

        _originPos = transform.position;
    }

    protected virtual void FixedUpdate()
    {
        HandleInput();
    }

    public void Reset()
    {
        transform.position = _originPos;
    }

    void HandleInput()
    {
        var value = joyStick.Direction;

        if (Input.GetMouseButton(0))
        {
            transform.position = transform.position + new Vector3(value.x, 0, value.y) * Time.fixedDeltaTime * speed;
            Vector3 movement = new Vector3(value.x, 0.0f, value.y);
            //_rb.velocity = movement *  speed;
            dustPS.transform.rotation = Quaternion.LookRotation(movement);
            _animator.SetTrigger("Run");

            //if (!grassPS.isPlaying)
            //    grassPS.Play();
        }
        else
        {
            _animator.SetTrigger("Idle");
            //grassPS.Stop();
        }
    }
}

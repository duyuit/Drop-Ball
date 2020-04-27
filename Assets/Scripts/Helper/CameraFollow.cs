using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    private Vector3 offset;
    private float _posY;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);

        _posY = transform.position.y;
        target = GameObject.FindGameObjectWithTag("Player").transform;
        offset = transform.position - target.position;
    }
    private void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            desiredPosition.y = _posY;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}

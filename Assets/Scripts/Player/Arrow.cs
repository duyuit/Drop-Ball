using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{

    public bool isMe = false;

    [HideInInspector]
    public bool isLeft = true;

    private float _currentYRotation = 0;
    private float _lastSendRotate = 0;
    // Start is called before the first frame update
    void Start()
    {
        transform.localEulerAngles = new Vector3(90, 0, 0);

        if (!isMe && GlobalVariable.isOnline)
            NetworkController.Instance.onUpdateRotation += UpdateRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (isLeft)
        {
            _currentYRotation -= Time.deltaTime * 60;
        }
        else
        {
            _currentYRotation += Time.deltaTime * 60;
        }

        transform.localEulerAngles = new Vector3(90, _currentYRotation, 0);
        if (_currentYRotation > 30f)
            isLeft = true;

        if (_currentYRotation < -30f)
            isLeft = false;

        if (GlobalVariable.isOnline && isMe && Time.time - _lastSendRotate > 0.05f)
        {
            NetworkController.Instance.SendUpdateArrowRotate(isLeft, _currentYRotation);
            _lastSendRotate = Time.time;
        }
    }

    public void UpdateRotation(bool isLeft, float currentY)
    {
        Debug.Log("Update opponent arrow");
        this.isLeft = isLeft;
        _currentYRotation = currentY;
    }
}

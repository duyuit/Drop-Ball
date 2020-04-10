using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticRotation : MonoBehaviour
{
    private Quaternion _originRotation;
    // Start is called before the first frame update
    void Start()
    {
        _originRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = _originRotation; 
    }
}

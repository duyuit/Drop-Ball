using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionObserve : MonoBehaviour
{
    public Transform observer;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(observer.position.x, 1.1f, observer.position.z);        
    }
}

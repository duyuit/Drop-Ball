using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateController : MonoBehaviour
{
    public GameObject ballPrefab;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Spawn", 0, 2);
    }

    void Spawn()
    {
        var ball = Instantiate(ballPrefab);
        ball.transform.position = new Vector3(Random.Range(-9, 9), Random.Range(4f, 11f), 15f);
        ball.GetComponent<BallBouncer>().SetVelocity(new Vector3(Random.Range(-3, 3), 0, Random.Range(-10f,-15f)));
    }
}

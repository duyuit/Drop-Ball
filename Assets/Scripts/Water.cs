using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    public GameObject playerDiePrefab;
    public GameObject enemyDiePrefab;

    private void OnTriggerEnter(Collider other)
    {
        int layer = other.gameObject.layer;

        //GameObject diePS = null;
        switch (layer)
        {
            case 8: // Ball
                Debug.Log("Drop ball");
                GameController.Instance.DropBall();
                //diePS = Instantiate(playerDiePrefab);
                //GameController.Instance.Lose();

                break;
            case 9: // Player 
                if (other.GetComponentInChildren<BallCatcher>().HasBall())
                    GameController.Instance.DropBall();
                break;
            case 10: // Bot
                if (other.GetComponentInChildren<BallCatcher>().HasBall())
                    GameController.Instance.DropBall();
                return;

        }

        //if (diePS != null)
        //    diePS.transform.position = other.gameObject.transform.position;

    }
}

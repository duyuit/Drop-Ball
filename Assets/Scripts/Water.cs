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

                GameController.Instance.DropBall();
                GameController.Instance.Reset();
                //diePS = Instantiate(playerDiePrefab);
                //GameController.Instance.Lose();

                break;
            case 9: // Player 
                GameController.Instance.SetWinner(Winner.BOT);
                //diePS = Instantiate(enemyDiePrefab);
                //GameController.Instance.OnKill(other.GetComponent<Bot>());
                break;
            case 10: // Bot
                if (other.GetComponentInChildren<BallCatcher>().HasBall())
                    GameController.Instance.SetWinner(Winner.PLAYER);
                return;

        }

        //if (diePS != null)
        //    diePS.transform.position = other.gameObject.transform.position;

    }
}

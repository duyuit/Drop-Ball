using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRange : MonoBehaviour
{
    public PlayerTag playerTag;
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Ball")
        {
            GameController.Instance.SetLastRange(playerTag);
        }
    }
}

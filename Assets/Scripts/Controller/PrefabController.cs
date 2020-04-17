using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabController : MonoBehaviour
{
    public GameObject starPrefab;
    public GameObject myBlock;
    public GameObject playerPrefab;
    public GameObject opponentPrefab;

    public static PrefabController Instance = null;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance.gameObject);
            Instance = this;
        }
    }
}

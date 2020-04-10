using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GrassController : MonoBehaviour
{
    public static GrassController Instance = null;

    // Put the gras prefabs here. They will be chosen by random
    public List<GameObject> flowerPrefabs = new List<GameObject>();
    public List<GameObject> mushroomPrefabs = new List<GameObject>();
    public GameObject grassPrefab;

    public int grassNumber = 64;
    public float grassAreaWidth = 5;
    public float grassAreaHeight = 5;
    public string interactionTag = "Player"; // Tag objects with this string, that you want to interact with the gras

    private Vector4[] grassInteractionPositions = new Vector4[4];
    private Transform ground;

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

    public void GeneratePlan()
    {
        ground = transform;
        float groundWidthHalf = grassAreaWidth / 2;
        float groundDepthHalf = grassAreaHeight / 2;

        // Create some gras at random positions in given area
        // Flower
        for (int grassIndex = 0; grassIndex < grassNumber / 2; grassIndex++)
        {
            Vector3 position = transform.position + new Vector3(Random.Range(-groundWidthHalf, groundWidthHalf), 0, Random.Range(-groundDepthHalf, groundDepthHalf));
            GameObject newGrass = Instantiate(flowerPrefabs[Random.Range(0, flowerPrefabs.Count)], ground.transform);
            newGrass.transform.position = position;
            newGrass.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            newGrass.transform.DOScale(4f, 2f);
        }

        // Mushroom
        for (int grassIndex = 0; grassIndex < grassNumber / 4; grassIndex++)
        {
            Vector3 position = transform.position + new Vector3(Random.Range(-groundWidthHalf, groundWidthHalf), 0, Random.Range(-groundDepthHalf, groundDepthHalf));
            GameObject newGrass = Instantiate(mushroomPrefabs[Random.Range(0, mushroomPrefabs.Count)], ground.transform);
            newGrass.transform.position = position;
            newGrass.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            newGrass.transform.DOScale(2f, 2f);
        }

        //// Grass
        //for (int grassIndex = 0; grassIndex < grassNumber * 1.5f; grassIndex++)
        //{
        //    Vector3 position = transform.position + new Vector3(Random.Range(-groundWidthHalf, groundWidthHalf), 0, Random.Range(-groundDepthHalf, groundDepthHalf));
        //    GameObject newGrass = Instantiate(grassPrefab, ground.transform);
        //    newGrass.transform.position = position;
        //    newGrass.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        //    newGrass.transform.DOScale(1.5f, 2f);
        //}
    }


    //private void Update() {
    //    int interactionObjIndex = 0;
    //    foreach (GameObject interactionObj in GameObject.FindGameObjectsWithTag(interactionTag)) {
    //        grassInteractionPositions[interactionObjIndex++] = interactionObj.transform.position + new Vector3(0, 0.5f, 0);
    //    }
    //    Shader.SetGlobalFloat("_PositionArray", interactionObjIndex);
    //    Shader.SetGlobalVectorArray("_Positions", grassInteractionPositions);
    //}
}

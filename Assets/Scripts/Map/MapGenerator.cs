using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject grassPrefab;
    public GameObject groundPrefab;
    public int width = 10;
    public int height = 10;

    public static MapGenerator Instance = null;

    private Dictionary<MyBlock, MyBlock> dictBlocks;
    private MyBlock[,] blocksArray;

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


    // Start is called before the first frame update
    void Start()
    {
        dictBlocks = new Dictionary<MyBlock, MyBlock>();
        blocksArray = new MyBlock[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var grass = Instantiate(grassPrefab, transform);
                grass.transform.localPosition = new Vector3(i, 0.3f, j);

                var ground = Instantiate(groundPrefab, transform);
                ground.transform.localPosition = new Vector3(i, 0, j);

                var grassBlock = grass.GetComponent<MyBlock>();
                var groundBlock = ground.GetComponent<MyBlock>();

                grassBlock.index = new Vector2Int(i, j);
                groundBlock.index = new Vector2Int(i, j);

                dictBlocks.Add(grassBlock, groundBlock);
                blocksArray[i, j] = grassBlock;
            }
        }

        transform.position = new Vector3(-width / 2f, transform.position.y, -height / 2f);
    }

    public void Cut(MyBlock grass, AwakeCircleClipper clipper)
    {
        Vector2Int index = grass.index;
        List<MyBlock> listCutBlock = new List<MyBlock>();

        for (int i = index.x - 1; i <= index.x + 1; i++)
        {
            if (i < 0 || i >= width)
                continue;
            for (int j = index.y - 1; j <= index.y + 1; j++)
            {
                if (j < 0 || j >= height)
                    continue;
                listCutBlock.Add(blocksArray[i, j]);
            }
        }

        foreach(var block in listCutBlock)
        {
            clipper.terrain = block;
            clipper.Cut();
            clipper.terrain = dictBlocks[block];
            clipper.Cut();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField]
    public WorldGenerator worldGenerator;

    // Start is called before the first frame update
    void Start()
    {
        Vector3Int[] positions = new Vector3Int[]
        {
            new Vector3Int(0, 0, 0),
            new Vector3Int(1, 0, -1),
            new Vector3Int(0, 1, -1),
            //new Vector3Int(-1, 1, 0),
            //new Vector3Int(-1, 0, 1),
            //new Vector3Int(0, -1, 1),
            //new Vector3Int(1, -1, 0),
            //new Vector3Int(0, 0, 0),
            //new Vector3Int(0, 0, 0),
            //new Vector3Int(0, 0, 0),
            //new Vector3Int(0, 0, 0),
            //new Vector3Int(1, 0, 0),
            //new Vector3Int(0, 1, 0),
            //new Vector3Int(0, 0, 1),
            //new Vector3Int(-1, 0, 0),
            //new Vector3Int(0, -1, 0),
            //new Vector3Int(0, 0, -1),
            //new Vector3Int(0, 2, 0),
        };

        //foreach (Vector3Int pos in positions)
        //{
        //    GameObject newHex = GameObject.Instantiate(hexagon);
        //    HexCoord hexCoord = new HexCoord(pos);
        //    newHex.GetComponent<HexCell>().coord = hexCoord;
        //    newHex.transform.position = hexCoord.ToGrid();
        //    newHex.transform.GetChild(0).GetComponent<MeshRenderer>().material = materials[Random.Range(0, materials.Length)];

        //}

        //for (int i = 0; i < 10; i++)
        //{
        //    GameObject newHex = GameObject.Instantiate(hexagon);
        //    //newHex.transform.position = new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
        //    //newHex.transform.position = new Vector3(height * i, 0, 0);
        //    //newHex.transform.position = new Vector3(0, 0, width * i);
        //    newHex.transform.position = upRight * i;
        //    newHex.transform.GetChild(0).GetComponent<MeshRenderer>().material = materials[Random.Range(0, materials.Length)];
        //}

        TileType[,] tiles = worldGenerator.Generate();
        worldGenerator.CreateGameObjects(tiles);

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField]
    public WorldGenerator worldGenerator;

    [SerializeField]
    public ResourceManager resourceManager;

    [SerializeField]
    public FogOfWarManager fogOfWarManager;

    [SerializeField]
    public UIManager uiManager;

    [SerializeField]
    public ErrorManager errorManager;

    [SerializeField]
    public CameraController cameraController;

    [SerializeField]
    public Material waterMaterial;

    public static TileType[,] allTiles;
    public static HexCell[,] allCells;

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

        allTiles = worldGenerator.Generate();
        allCells = worldGenerator.CreateGameObjects(allTiles);

        resourceManager.InitializeChanges(allTiles);

        cameraController.lookatPoint = new Vector3(
            1.7f * -worldGenerator.height / 2.0f, 
            0, 
            1.5f * -worldGenerator.width / 2.0f
        );
        Debug.Log($"pos: {cameraController.controlledCamera.transform.position}");
        
        // Generate the noise map from the worldgen and pass it into the water shader
        int w = worldGenerator.width;
        int h = worldGenerator.height;
        Texture2D heightNoiseMap = new Texture2D(w, h, TextureFormat.R8, mipChain: false);
        //heightNoiseMap.SetPixels
        Color[] colors = new Color[w * h];
        for (int i = 0; i < worldGenerator.width; i++)
        {
            for (int j = 0; j < worldGenerator.height; j++)
            {
                float noiseVal = worldGenerator.noiseGenerator.GetValueNormalized(i, j);
                colors[(i * w) + j] = new Color(noiseVal, noiseVal, noiseVal);
            }
        }
        heightNoiseMap.SetPixels(colors);
        heightNoiseMap.Apply();
        this.waterMaterial.SetTexture("_HeightNoiseMap", heightNoiseMap);

        this.fogOfWarManager.Generate();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

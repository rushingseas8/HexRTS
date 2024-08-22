using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Generates the world. Specifically will determine which broad tile type
/// each section of the world is, e.g. forest, hills, etc.
/// </summary>
public class WorldGenerator : MonoBehaviour
{
    [SerializeField]
    public GameObject hexCell;

    //[SerializeField]
    //public GameObject hexagon;

    //[SerializeField]
    //public GameObject tree;

    //[SerializeField]
    //public GameObject camp;
    
    [SerializeField]
    public Material[] materials;

    [SerializeField]
    public GameObject[] meshes;

    [SerializeField]
    public GameObject[] improvements;

    [SerializeField]
    public PerlinGenerator noiseGenerator;

    [SerializeField]
    public int randomSeed = 1337;
    
    [SerializeField]
    [Range(1, 150)]
    public int width = 21;
    
    [SerializeField]
    [Range(1, 150)]
    public int height = 11;

    /// <summary>
    /// Delegate class for a given stage of the world generation pipeline.
    /// 
    /// This represents the transformation you wish to apply to a given tile at a 
    /// specified coordinate. You get a view of the rest of the world in case you want
    /// to influence your behavior based on neighbors.
    /// 
    /// At each stage, all changes are fully applied before moving on; you don't have
    /// to worry about partial progress from a previous stage.
    /// </summary>
    /// <param name="tiles">A view of the world at the start of this stage</param>
    /// <param name="x">X coord into the world</param>
    /// <param name="z">Z coord into the world</param>
    /// <returns></returns>
    public delegate TileType WorldGenTransform(TileType[,] tiles, int x, int z);
    
    public TileType MakeEdge(TileType[,] tiles, int x, int z)
    {
        int boundarySize = 10;
        if (
            x <= boundarySize ||
            x >= tiles.GetLength(0) - boundarySize || 
            z <= boundarySize || 
            z >= tiles.GetLength(1) - boundarySize)
        {
            return TileType.Shadow;
        }
        return tiles[x, z];
    }

    /// <summary>
    /// Does the actual world generation.
    /// </summary>
    /// <returns></returns>
    public TileType[,] Generate()
    {
        Random.InitState(randomSeed);

        // Initialize all tiles to water to start.
        TileType[,] lastTiles = new TileType[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                lastTiles[i, j] = TileType.Water;
            }
        }
        TileType[,] tiles = lastTiles.Clone() as TileType[,];

        // Create a list of all the transformations we want to apply, in order
        //WorldGenTransform[] worldGenStages = new WorldGenTransform[]
        //{
        //    new WorldGenTransform(BasicGeneration),
        //    new WorldGenTransform(MakeBeaches),
        //    new WorldGenTransform(MakeMountains),
        //    new WorldGenTransform(MakeForests),
        //    new WorldGenTransform(MakeOceans),
        //    //new WorldGenTransform(MakeEdge), // Not sure how I feel about this as a fog of war.
        //};

        // Create a list of all the transformations we want to apply, in order
        WorldGenBase[] worldGenStages = new WorldGenBase[]
        {
            new WorldGenOceans(),
            new WorldGenBeaches(),
            //new WorldGenMountains(),
            new WorldGenForests(),
            //new WorldGenZoomFuzzy(),
            //new WorldGenZoomFuzzy(),
            new WorldGenZoom(),
            new WorldGenZoom(),
            new WorldGenBeaches(),
            // new WorldGenTest(),
            // new WorldGenZoom(),
            // new WorldGenZoom(),
        };

        // Iterate through each transform
        foreach (WorldGenBase transform in worldGenStages)
        //foreach (WorldGenTransform transform in worldGenStages)
        {
            // And apply it to each tile
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    tiles[i, j] = transform.Generate(lastTiles, this.noiseGenerator, i, j);
                    //tiles[i, j] = transform(lastTiles, i, j);
                }
            }

            // "tiles" now holds the new changes; we move it back to lastTiles
            // and make a copy for the next transform to mutate
            lastTiles = tiles;
            tiles = lastTiles.Clone() as TileType[,];
        }

        // I don't really like how this looks. It's a basic river carver
        //CarverRiver carverRiver = new CarverRiver();
        //carverRiver.Carve(ref tiles, this.noiseGenerator);

        return tiles;
    }

    /// <summary>
    /// Given a 2D array of TileTypes, actually generates the approprate GameObjects.
    /// </summary>
    /// <param name="tiles"></param>
    public HexCell[,] CreateGameObjects(TileType[,] tiles)
    {
        HexCell[,] allCells = new HexCell[tiles.GetLength(0), tiles.GetLength(1)];
        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            for (int j = 0; j < tiles.GetLength(1); j++)
            {
                TileType tile = tiles[i, j];

                HexCell newCell = CreateCell(tile, i, j);
                allCells[i, j] = newCell;

                //OffsetCoord coord = new OffsetCoord(j - (height - 1) / 2, i - (width - 1) / 2);
                //HexCoord hexCoord = coord.ToHexCoord();

                //GameObject hexCellGameObject = GameObject.Instantiate(hexCell);
                //HexCell cell = hexCellGameObject.GetComponent<HexCell>();
                //allCells[i,j] = cell;  // Save this cell

                //GameObject newMesh = GameObject.Instantiate(meshes[(int) tile._mesh]);
                //cell.coord = hexCoord;
                //cell.tileType = tile;
                //cell.tile = newMesh;
                
                //// Some height generation.
                //float heightOffset = 0;
                //if (tile != TileType.Water)
                //{
                //    heightOffset = -6 * (noiseGenerator.GetValueNormalized(i, j) - 0.5f);
                //    heightOffset = (int)(heightOffset * 4) / 4.0f;
                //    heightOffset += 0.25f;
                //}

                //// Add the hex coord to the world, add some height offset
                //newMesh.transform.position = hexCoord.ToWorld() + new Vector3(0, heightOffset, 0);
                //// Randomize its rotation to any 60 degree interval
                //newMesh.transform.rotation = Quaternion.AngleAxis(Random.Range(0, 6) * 60, Vector3.up);
                //// Pick its material based on its tile type
                //newMesh.transform.GetChild(0).GetComponent<MeshRenderer>().material = materials[(int) tile._material];
                //// Make sure it gets batched
                //newMesh.isStatic = true;

                //// Do we generate an improvement here?
                //if (tile._improvement != TileType.ImprovementType.Empty)
                //{
                //    // Let's create a new one and parent it to the newly generated hex.
                //    GameObject improvement = GameObject.Instantiate(improvements[(int) tile._improvement]);
                //    improvement.transform.SetParent(hexCellGameObject.transform);
                //    // Set its position/rotation
                //    improvement.transform.position = hexCoord.ToWorld() + new Vector3(0, heightOffset, 0);
                //    improvement.transform.rotation = Quaternion.AngleAxis(Random.Range(0, 6) * 60, Vector3.up);
                //    improvement.isStatic = true;
                //    cell.improvement = improvement;
                //}
            }
        }
        return allCells;
    }

    public HexCell CreateCell(TileType newTile, int x, int z)
    {
        OffsetCoord coord = new OffsetCoord(z, x);
        HexCoord hexCoord = coord.ToHexCoord();

        GameObject hexCellGameObject = GameObject.Instantiate(hexCell);
        HexCell cell = hexCellGameObject.GetComponent<HexCell>();

        GameObject newMesh = GameObject.Instantiate(meshes[(int) newTile._mesh]);
        cell.coord = hexCoord;
        cell.tileType = newTile;
        cell.tile = newMesh;
                
        // Some height generation.
        float heightOffset = 0;

        heightOffset = -6 * (noiseGenerator.GetValueNormalized(x, z) - 0.5f);
        heightOffset = (int)(heightOffset * 4) / 4.0f;
        heightOffset += 0.25f;
        
        if (newTile == TileType.Water || newTile == TileType.ShallowWater || newTile == TileType.DeepWater)
        {
            //heightOffset -= 0.5f;
            heightOffset = 0;
        }

        // Add the hex coord to the world, add some height offset
        newMesh.transform.position = hexCoord.ToWorld() + new Vector3(0, heightOffset, 0);
        // Randomize its rotation to any 60 degree interval
        newMesh.transform.rotation = Quaternion.AngleAxis(Random.Range(0, 6) * 60, Vector3.up);
        // Pick its material based on its tile type
        newMesh.transform.GetChild(0).GetComponent<MeshRenderer>().material = materials[(int) newTile._material];
        // Reparent it
        newMesh.transform.parent = hexCellGameObject.transform;
        // Make sure it gets batched
        newMesh.isStatic = true;

        // Do we generate an improvement here?
        if (newTile._improvement != TileType.ImprovementType.Empty)
        {
            // Let's create a new one and parent it to the newly generated hex.
            GameObject improvement = GameObject.Instantiate(improvements[(int) newTile._improvement]);
            improvement.transform.SetParent(hexCellGameObject.transform);
            // Set its position/rotation
            improvement.transform.position = hexCoord.ToWorld() + new Vector3(0, heightOffset, 0);
            improvement.transform.rotation = Quaternion.AngleAxis(Random.Range(0, 6) * 60, Vector3.up);
            improvement.isStatic = true;
            cell.improvement = improvement;
        }

        return cell;
    }

    // Replace a tile in the world with a new TileType.
    public void ReplaceTile(HexCell[,] allCells, TileType[,] allTiles, int x, int z, TileType newTile)
    {
        // Grab the old and create the new tile
        HexCell cellToReplace = allCells[x, z];
        HexCell newCell = CreateCell(newTile, x, z);

        // Delete old
        //GameObject.Destroy(cellToReplace.gameObject);
        
        // Update resources
        GameObject.Find("Game Manager").GetComponent<ResourceManager>().UpdateTile(cellToReplace.tileType, newCell.tileType);

        // Replace the tile in the relevant arrays
        allCells[x, z] = newCell;
        allTiles[x, z] = newTile;

        // Update fog of war
        this.GetComponent<FogOfWarManager>().AddPlayerControlledTile(new Vector2Int(z, x), 4);

        this.StartCoroutine(SwapTiles(cellToReplace, newCell, 0.5f));
    }

    public IEnumerator SwapTiles(HexCell oldCell, HexCell newCell, float time) 
    {
        GameObject newImprovement = newCell.improvement;
        //if (newImprovement != null)
        //{
        //    newImprovement.SetActive(false);
        //}


        GameObject oldImprovement = oldCell.improvement;
        if (oldImprovement != null)
        {
            Vector3 targetPosition = oldImprovement.transform.position;

            // Take 5 seconds
            float squishDuration = time;
            float currentTime = 0f;
            while (currentTime < squishDuration)
            {
                float t = currentTime / squishDuration;

                float scaleDownT = Mathf.Max(0, (t - 0.5f) * 2.0f);

                // Scale down the existing
                Vector3 newScale = Vector3.Lerp(Vector3.one, new Vector3(1f, 0.1f, 1f), scaleDownT);
                oldImprovement.transform.localScale = newScale;

                float tSmooth = t * t * (3f - 2f * t);
                Vector3 newOffset = Vector3.Lerp(new Vector3(0, 10f, 0), new Vector3(0, 0, 0), tSmooth);
                newImprovement.transform.position = targetPosition + newOffset;

                currentTime += Time.deltaTime;
                yield return null;
            }
            oldImprovement.transform.localScale = new Vector3(1f, 0.1f, 1f);
            GameObject.Destroy(oldImprovement);
        }

        //if (newImprovement != null)
        //{
        //    newImprovement.SetActive(true);
        //}
    }
}

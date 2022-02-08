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
    public GameObject hexagon;

    [SerializeField]
    public GameObject tree;

    [SerializeField]
    public GameObject camp;
    
    [SerializeField]
    public Material[] materials;

    [SerializeField]
    public GameObject[] improvements;

    [SerializeField]
    public PerlinGenerator noiseGenerator;
    
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

    public TileType BasicGeneration(TileType[,] tiles, int x, int z)
    {
        float noise = noiseGenerator.GetValueNormalized(x, z);
                
        TileType tile = TileType.Water;
        if (noise < 0.5f)
        {
            tile = TileType.Grass;
        }

        //tiles[i,j] = tile;
        return tile;
    }

    public TileType GetOrEmpty(TileType[,] tiles, int x, int z)
    {
        if (x < 0 || x >= tiles.GetLength(0))
        {
            return TileType.Empty;
        }
        
        if (z < 0 || z >= tiles.GetLength(1))
        {
            return TileType.Empty;
        }

        return tiles[x,z];
    }

    public static readonly int[,,] EVENQ_OFFSETS =
    {
        // Even columns
        {
            { 1, 1 },
            { 1, 0 },
            { 0, -1 },
            { -1, 0 },
            { -1, 1 },
            { 0, 1 },
        },
        // Odd columns
        {
            { 1, 0 },
            { 1, -1 },
            { 0, -1 },
            { -1, -1 },
            { -1, 0 },
            { 0, 1 },
        }
    };

    /// <summary>
    /// Returns the neighbor of a tile in a given direction.
    /// </summary>
    /// <param name="tiles"></param>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public TileType GetNeighbor(TileType[,] tiles, int x, int z, int direction)
    {
        int parity = x & 1;
        return GetOrEmpty(
            tiles, 
            x + EVENQ_OFFSETS[parity, direction, 0], 
            z + EVENQ_OFFSETS[parity, direction, 1]
        );
    }

    /// <summary>
    /// Returns all 6 neighbors of the given tile. If the index is OOB,
    /// returns TileType.Empty instead.
    /// 
    /// </summary>
    /// <param name="tiles"></param>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public TileType[] GetNeighbors(TileType[,] tiles, int x, int z)
    {
        TileType[] toReturn = new TileType[6];
        for (int direction = 0; direction < 6; direction++)
        {
            toReturn[direction] = GetNeighbor(tiles, x, z, direction);
        }
        return toReturn;
    }

    public TileType MakeBeaches(TileType[,] tiles, int x, int z)
    {
        // Get our tile
        TileType ourTile = tiles[x, z];
        // If we are not grass (i.e., water) then ignore.
        if (ourTile != TileType.Grass)
        {
            return ourTile;
        }

        // Check our neighbors. If any are water, return sand.
        TileType[] neighbors = GetNeighbors(tiles, x, z);
        if (neighbors.Any(tile => tile == TileType.Water))
        {
            return TileType.Sand;
        }

        // Landlocked, so just return grass.
        return ourTile;
    }

    public TileType MakeForests(TileType[,] tiles, int x, int z)
    {
        TileType ourTile = tiles[x, z];
        if (ourTile._material == TileType.MaterialType.Grass && !ourTile.HasImprovement())
        {
            int rand = Random.Range(0, 100);
            // Lumber camp is a visual test
            if (rand < 5)
            {
                return TileType.LumberCamp;
            }
            // 80% chance that grass turns into forest
            else if (rand < 80)
            {
                return TileType.Forest;
            }
        }
        return ourTile;
    }

    /// <summary>
    /// Does the actual world generation.
    /// </summary>
    /// <returns></returns>
    public TileType[,] Generate()
    {
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
        WorldGenTransform[] worldGenStages = new WorldGenTransform[]
        {
            new WorldGenTransform(BasicGeneration),
            new WorldGenTransform(MakeBeaches),
            new WorldGenTransform(MakeForests),
        };

        // Iterate through each transform
        foreach (WorldGenTransform transform in worldGenStages)
        {
            // And apply it to each tile
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    tiles[i, j] = transform(lastTiles, i, j);
                }
            }

            // "tiles" now holds the new changes; we move it back to lastTiles
            // and make a copy for the next transform to mutate
            lastTiles = tiles;
            tiles = lastTiles.Clone() as TileType[,];
        }
        return tiles;
    }

    /// <summary>
    /// Given a 2D array of TileTypes, actually generates the approprate GameObjects.
    /// </summary>
    /// <param name="tiles"></param>
    public void CreateGameObjects(TileType[,] tiles)
    {
        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            for (int j = 0; j < tiles.GetLength(1); j++)
            {
                TileType tile = tiles[i, j];

                OffsetCoord coord = new OffsetCoord(j - (height - 1) / 2, i - (width - 1) / 2);
                HexCoord hexCoord = coord.ToHexCoord();

                GameObject newHex = GameObject.Instantiate(hexagon);
                HexCell cell = newHex.GetComponent<HexCell>();
                cell.coord = hexCoord;
                cell.tileType = tile;
                
                // Some height generation.
                float heightOffset = 0;
                if (tile != TileType.Water)
                {
                    heightOffset = -6 * (noiseGenerator.GetValueNormalized(i, j) - 0.5f);
                    heightOffset = (int)(heightOffset * 4) / 4.0f;
                    heightOffset += 0.25f;
                }

                // Add the hex coord to the world, add some height offset
                newHex.transform.position = hexCoord.ToWorld() + new Vector3(0, heightOffset, 0);
                // Randomize its rotation to any 60 degree interval
                newHex.transform.rotation = Quaternion.AngleAxis(Random.Range(0, 6) * 60, Vector3.up);
                // Pick its material based on its tile type
                newHex.transform.GetChild(0).GetComponent<MeshRenderer>().material = materials[(int) tile._material];
                // Make sure it gets batched
                newHex.isStatic = true;

                // Do we generate an improvement here?
                if (tile._improvement != TileType.ImprovementType.Empty)
                {
                    // Let's create a new one and parent it to the newly generated hex.
                    GameObject improvement = GameObject.Instantiate(improvements[(int) tile._improvement]);
                    improvement.transform.SetParent(newHex.transform);
                    // Set its position/rotation
                    improvement.transform.position = hexCoord.ToWorld() + new Vector3(0, heightOffset, 0);
                    improvement.transform.rotation = Quaternion.AngleAxis(Random.Range(0, 6) * 60, Vector3.up);
                    improvement.isStatic = true;
                }
            }
        }
    }
}

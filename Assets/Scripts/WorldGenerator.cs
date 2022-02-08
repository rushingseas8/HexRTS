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
        TileType ourTile = tiles[x,z];
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
                OffsetCoord coord = new OffsetCoord(j - (height - 1) / 2, i - (width - 1) / 2);
                HexCoord hexCoord = coord.ToHexCoord();

                GameObject newHex = GameObject.Instantiate(hexagon);
                newHex.GetComponent<HexCell>().coord = hexCoord;

                float heightOffset = 0;
                if (tiles[i, j] != TileType.Water)
                {
                    heightOffset = -6 * (noiseGenerator.GetValueNormalized(i, j) - 0.5f);
                    heightOffset = (int)(heightOffset * 4) / 4.0f;
                    heightOffset += 0.25f;
                }

                if (tiles[i, j] == TileType.Grass)
                {
                    if (Random.Range(0f, 1f) < 0.05f)
                    {
                        GameObject newCamp = GameObject.Instantiate(camp);
                        newCamp.transform.SetParent(newHex.transform);
                        newCamp.isStatic = true;
                           
                    }
                    else if (Random.Range(0f, 1f) < 0.8f)
                    {
                        GameObject newTree = GameObject.Instantiate(tree);
                        newTree.transform.SetParent(newHex.transform);
                        newTree.isStatic = true;
                    }
                }

                // TODO add verticality?
                newHex.transform.position = hexCoord.ToWorld() + new Vector3(0, heightOffset, 0);
                newHex.transform.rotation = Quaternion.AngleAxis(Random.Range(0, 6) * 60, Vector3.up);
                newHex.transform.GetChild(0).GetComponent<MeshRenderer>().material = materials[tiles[i, j]];
                newHex.isStatic = true;
            }
        }
    }
}

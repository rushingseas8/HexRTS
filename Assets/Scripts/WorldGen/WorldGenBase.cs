using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WorldGenBase
{
    public abstract TileType Generate(TileType[,] tiles, PerlinGenerator noise, int x, int z);

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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenRivers : WorldGenBase
{
    private int[,] riverMask;

    public WorldGenRivers(TileType[,] tiles)
    {
        // Initialize the riverMask array. 0 for oceans (which we will ignore), otherwise
        // an integer from 2 - 300,000. This is how RiverInitLayer works in Minecraft.
        for (int i = 0; i < tiles.GetLength(0); i++) {
            for (int j = 0; j < tiles.GetLength(1); j++) { 
                if (tiles[i, j] == TileType.Empty || tiles[i, j] == TileType.Water) {
                    riverMask[i, j] = 0;
                }
                riverMask[i, j] = 2 + Random.Range(0, 299998);
            }
        }
    }

    private int RiverFilter(TileType tileType)
    {
        return 0;
        //return value >= 2 ? 2 + (value & 1) : value;
    }

    public override TileType Generate(TileType[,] tiles, PerlinGenerator noiseGen, int x, int z)
    {
        //TileType
        int center = RiverFilter(tiles[x, z]);
        return tiles[x, z];
    }
}

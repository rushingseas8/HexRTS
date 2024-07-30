using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenOceans : WorldGenBase
{
    public override TileType Generate(TileType[,] tiles, PerlinGenerator noiseGen, int x, int z)
    {
        float noise = noiseGen.GetValueNormalized(x, z);
                
        TileType tile = TileType.Water;
        if (noise < 0.5f)
        {
            tile = TileType.Grass;
        }

        //tiles[i,j] = tile;
        return tile;
    }
}

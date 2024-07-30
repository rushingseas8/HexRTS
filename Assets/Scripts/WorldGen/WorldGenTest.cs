using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldGenTest : WorldGenBase
{
    public override TileType Generate(TileType[,] tiles, PerlinGenerator noiseGen, int x, int z)
    {
        //if ((x & 1) == 0 ^ (z & 1) == 0) 
        if ((x & 1) == 0 && (z & 1) == 0) 
        {
            return TileType.Water;
        }
        else
        {
            return TileType.Grass;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldGenMountains : WorldGenBase
{
    public override TileType Generate(TileType[,] tiles, PerlinGenerator noiseGen, int x, int z)
    {
        TileType ourTile = tiles[x, z];
        if (ourTile._material == TileType.MaterialType.Grass && !ourTile.HasImprovement())
        {
            int rand = Random.Range(0, 100);

            float noiseValue = noiseGen.GetValueNormalized(x, z);
            if (noiseValue < 0.5f && rand < 5)
            {
                return TileType.Stone;
            }
            if (noiseValue < 0.4f && rand < 75)
            {
                return TileType.Mountain;
            }
        }
        return ourTile;
    }
}

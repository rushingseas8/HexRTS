using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenForests : WorldGenBase
{
    public override TileType Generate(TileType[,] tiles, PerlinGenerator noiseGen, int x, int z)
    {
        
        TileType ourTile = tiles[x, z];
        if (ourTile._material == TileType.MaterialType.Grass && !ourTile.HasImprovement()) 
        {        
            int rand = Random.Range(0, 100);
            // 80% chance that grass turns into forest
            if (rand < 80)
            {
                return TileType.Forest;
            }
        }
        return ourTile;
    }
}

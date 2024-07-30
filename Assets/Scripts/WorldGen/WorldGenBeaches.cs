using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldGenBeaches : WorldGenBase
{
    public override TileType Generate(TileType[,] tiles, PerlinGenerator noiseGen, int x, int z)
    {
        // Get our tile
        TileType ourTile = tiles[x, z];
        // If we are not grass (i.e., water) then ignore.
        if (ourTile != TileType.Grass)
        {
            return ourTile;
        }

        // Check our neighbors. If any are water, return sand.
        TileType[] neighbors = this.GetNeighbors(tiles, x, z);
        if (neighbors.Any(tile => tile == TileType.Water))
        {
            return TileType.Sand;
        }

        // Landlocked, so just return grass.
        return ourTile;
    }
}

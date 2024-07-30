using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Based on GenLayerZoom from minecraft, but has weird artifacts.
// maybe because MC is infinite but this is pulling from finite data?
// ..maybe because it's going along the x/z axis for zoom rather than 3 hex axes.
public class WorldGenZoom : WorldGenBase
{
    public override TileType Generate(TileType[,] tiles, PerlinGenerator noiseGen, int x, int z)
    {
        int i = x / 2;
        int j = z / 2;

        bool xIsEven = (i & 1) == 0;
        bool zIsEven = (j & 1) == 0;
        TileType baseTile = tiles[i, j];
        if (xIsEven && zIsEven)
        {
            return baseTile;
        }

        TileType baseTileP1Z = tiles[i, j + 1];
        if (xIsEven && !zIsEven)
        {
            if (Random.Range(0, 2) == 0)
            {
                return baseTile;
            }
            else
            {
                return baseTileP1Z;
            }
        }
        else
        {
            TileType baseTileP1X = tiles[i + 1, j];
            if (!xIsEven && zIsEven)
            {
                if (Random.Range(0, 2) == 0)
                {
                    return baseTile;
                }
                else
                {
                    return baseTileP1X;
                }
            }
            else
            {
                // x and z are both odd
                TileType baseTileP1X1Z = tiles[i + 1, j + 1];
                return WeirdFuzzer(baseTile, baseTileP1X, baseTileP1Z, baseTileP1X1Z);
            }
        }

        //return tiles[x / 2, z / 2];
        //float noise = noiseGen.GetValueNormalized(x, z);
                
        //TileType tile = TileType.Water;
        //if (noise < 0.5f)
        //{
        //    tile = TileType.Grass;
        //}

        ////tiles[i,j] = tile;
        //return tile;
    }

    private TileType WeirdFuzzer(TileType p1, TileType p2, TileType p3, TileType p4) {
        if (p2 == p3 && p3 == p4) {
            return p2;
        } else if (p1 == p2 && p1 == p3) {
            return p1;
        } else if (p1 == p2 && p1 == p4) {
            return p1;
        } else if (p1 == p3 && p1 == p4) {
            return p1;
        } else if (p1 == p2 && p3 != p4) {
            return p1;
        } else if (p1 == p3 && p2 != p4) {
            return p1;
        } else if (p1 == p4 && p2 != p3) {
            return p1;
        } else if (p2 == p3 && p1 != p4) {
            return p2;
        } else if (p2 == p4 && p1 != p3) {
            return p2;
        } else if (p3 == p4 && p1 != p1) {
            return p3;
        } else {
            int choice = Random.Range(0, 4);
            return new TileType[]{ p1, p2, p3, p4 }[choice];
        }
        // } else {
        //     int choice = Random.Range(0, 4);
        //     return p3 == p4 && p1 != p2 ? p3 : new TileType[]{ p1, p2, p3, p4 }[choice];
        // }
    }

    private TileType WeirdFuzzerFuzzy(TileType p1, TileType p2, TileType p3, TileType p4) {
        int choice = Random.Range(0, 4);
        return new TileType[]{ p1, p2, p3, p4 }[choice];
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rule
{
    public TileType centerTile;

    public TileType[] adjacentTiles;

    /// <summary>
    /// Defines a rule for a center TileType + a variable number of adjacent
    /// TileTypes. Duplicates are allowed.
    /// </summary>
    /// <param name="center">The center TileType</param>
    /// <param name="adjacents">Any adjacent TileTypes</param>
    public Rule(TileType center, params TileType[] adjacents)
    {
        this.centerTile = center;
        this.adjacentTiles = adjacents;
    }

    public List<Rule> Matches(TileType[] allTiles, int x, int z, Rule[] allRules)
    {
        return null;
    }
}

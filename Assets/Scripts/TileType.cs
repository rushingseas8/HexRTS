using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// enum-like class for handling tile types and some helper functions
/// </summary>
public sealed class TileType
{
    public static readonly TileType Empty = new TileType(0, "Empty");
    public static readonly TileType Grass = new TileType(1, "Grass");
    public static readonly TileType Water = new TileType(2, "Water");
    public static readonly TileType Sand = new TileType(3, "Sand");

    public static readonly TileType[] AllTiles = new TileType[]
    {
        Empty,
        Grass,
        Water,
        Sand
    };

    public int _value;

    public string _name;

    public override string ToString()
    {
        return _name;
    }

    private TileType(int value, string name)
    {
        this._value = value;
        this._name = name;
    }

    public static implicit operator int(TileType t) => t._value;
    public static implicit operator TileType(int i) => AllTiles[i];

}
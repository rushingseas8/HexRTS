using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// enum-like class for handling tile types and some helper functions
/// </summary>
public sealed class TileType
{
    public enum MeshType
    {
        Flat
    }

    public enum MaterialType
    {
        Empty,
        Grass,
        Water,
        Sand
    }

    public enum ImprovementType
    {
        Empty,
        Forest,
        LumberCamp
    }

    public static readonly TileType Empty = new TileType(
        MeshType.Flat, 
        MaterialType.Empty, 
        ImprovementType.Empty, 
        "Empty");

    public static readonly TileType Grass = new TileType(
        MeshType.Flat,
        MaterialType.Grass,
        ImprovementType.Empty, 
        "Grass");

    public static readonly TileType Water = new TileType(
        MeshType.Flat,
        MaterialType.Water,
        ImprovementType.Empty,
        "Water");

    public static readonly TileType Sand = new TileType(
        MeshType.Flat,
        MaterialType.Sand,
        ImprovementType.Empty,
        "Sand");

    public static readonly TileType Forest = new TileType(
        MeshType.Flat,
        MaterialType.Grass,
        ImprovementType.Forest,
        "Forest");
    
    public static readonly TileType LumberCamp = new TileType(
        MeshType.Flat,
        MaterialType.Grass,
        ImprovementType.LumberCamp,
        "LumberCamp");

    public static readonly TileType[] AllTiles = new TileType[]
    {
        Empty,
        Grass,
        Water,
        Sand,
        Forest,
        LumberCamp
    };

    public MeshType _mesh;
    public MaterialType _material;
    public ImprovementType _improvement;

    public string _name;

    public override string ToString()
    {
        return _name;
    }

    private TileType(MeshType mesh, MaterialType material, ImprovementType improvement, string name)
    {
        this._mesh = mesh;
        this._material = material;
        this._improvement = improvement;
        this._name = name;
    }

    public bool HasImprovement()
    {
        return this._improvement != ImprovementType.Empty;
    }
}

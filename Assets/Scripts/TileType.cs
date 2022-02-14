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
        Shadow,
        Grass,
        Water,
        Sand,
        Mountain
    }

    public enum ImprovementType
    {
        Empty,
        Shadow,
        Forest,
        LumberCamp,
        Mill,
        House,
        Mountain
    }

    public static readonly TileType Empty = new TileType(
        MeshType.Flat, 
        MaterialType.Empty, 
        ImprovementType.Empty, 
        "Empty");

    public static readonly TileType Shadow = new TileType(
        MeshType.Flat, 
        MaterialType.Shadow, 
        ImprovementType.Shadow, 
        "FogOfWar");

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
        "Lumber Camp",
        producesPerTurn: new Dictionary<ResourceManager.ResourceType, int>() {
            { ResourceManager.ResourceType.Wood, 3 }
        },
        costsOnce: new Dictionary<ResourceManager.ResourceType, int>() {
            { ResourceManager.ResourceType.Population, 1 },
        },
        refundsOnce: new Dictionary<ResourceManager.ResourceType, int>() {
            { ResourceManager.ResourceType.Population, 1 },
        }
    );

    public static readonly TileType Mill = new TileType(
        MeshType.Flat,
        MaterialType.Grass,
        ImprovementType.Mill,
        "Mill",
        producesPerTurn: new Dictionary<ResourceManager.ResourceType, int>() {
            { ResourceManager.ResourceType.Food, 2 }
        },
        costsOnce: new Dictionary<ResourceManager.ResourceType, int>() {
            { ResourceManager.ResourceType.Wood, 3 }
        });

    public static readonly TileType House = new TileType(
        MeshType.Flat,
        MaterialType.Grass,
        ImprovementType.House,
        "House",
        producesOnce: new Dictionary<ResourceManager.ResourceType, int>() {
            { ResourceManager.ResourceType.Housing, 1 }
        },
        costsOnce: new Dictionary<ResourceManager.ResourceType, int>() {
            { ResourceManager.ResourceType.Wood, 3 }
        },
        refundsOnce: new Dictionary<ResourceManager.ResourceType, int>() {
            { ResourceManager.ResourceType.Housing, 1 }
        }
    );

    public static readonly TileType Mountain = new TileType(
        MeshType.Flat,
        MaterialType.Mountain,
        ImprovementType.Mountain, 
        "Mountain");

    public static readonly TileType[] AllTiles = new TileType[]
    {
        Empty,
        Grass,
        Water,
        Sand,
        Forest,
        LumberCamp,
        House
    };

    // Mapping of tiles to the list of possible improvements that can be built there.
    public static readonly Dictionary<TileType, List<TileType>> CanPlace = new Dictionary<TileType, List<TileType>>()
    {
        {
            Grass,
            new List<TileType>()
            {
                House,
                Mill
            }
        },
        {
            Forest,
            new List<TileType>()
            {
                LumberCamp
            }
        },
        {
            House,
            new List<TileType>()
            {
                Grass,
                Mill
            }
        },
        {
            Mill,
            new List<TileType>()
            {
                Grass,
                House
            }
        },
        {
            LumberCamp,
            new List<TileType>()
            {
                Grass
            }
        }
    };

    public MeshType _mesh;
    public MaterialType _material;
    public ImprovementType _improvement;

    public string _name;

    public int[] producesPerTurn;
    public bool doesProducePerTurn;

    public int[] producesOnce;
    public int[] costsPerTurn;
    public int[] costsOnce;
    public int[] refundsPerTurn;
    public int[] refundsOnce;

    public override string ToString()
    {
        return _name;
    }

    /// <summary>
    /// Defines a new TileType.
    /// </summary>
    /// <param name="mesh">Which physical base appearance this tile will take on. E.g., flat or hilly.</param>
    /// <param name="material">What material this tile uses.</param>
    /// <param name="improvement">What improvement is on top of this tile. E.g., forest or lumber camp.</param>
    /// <param name="name">The name of the tile. Used internally and for debugging.</param>
    /// <param name="producesPerTurn">What resoruces this tile produces each turn, if any.</param>
    /// <param name="producesOnce">What resources this tile produces when placed, if any.</param>
    /// <param name="costs">What resources this tile costs to place down, if any.</param>
    private TileType(
        MeshType mesh, 
        MaterialType material, 
        ImprovementType improvement, 
        string name, 
        Dictionary<ResourceManager.ResourceType, int> producesPerTurn=null,
        Dictionary<ResourceManager.ResourceType, int> producesOnce=null,
        Dictionary<ResourceManager.ResourceType, int> costsPerTurn=null,
        Dictionary<ResourceManager.ResourceType, int> costsOnce=null,
        Dictionary<ResourceManager.ResourceType, int> refundsPerTurn=null,
        Dictionary<ResourceManager.ResourceType, int> refundsOnce=null
        )
    {
        this._mesh = mesh;
        this._material = material;
        this._improvement = improvement;
        this._name = name;

        SetupResources(ref this.producesPerTurn, producesPerTurn, out this.doesProducePerTurn);
        SetupResources(ref this.producesOnce, producesOnce, out _);
        SetupResources(ref this.costsPerTurn, costsPerTurn, out _);
        SetupResources(ref this.costsOnce, costsOnce, out _);
        SetupResources(ref this.refundsPerTurn, refundsPerTurn, out _);
        SetupResources(ref this.refundsOnce, refundsOnce, out _);

        //// Assign this value to false to start.
        //this.doesProducePerTurn = false;

        //// This tile doesn't produce anything. Set to defaults.
        //if (producesPerTurn == null)
        //{
        //    this.producesPerTurn = new int[ResourceManager.allResourceTypes.Length];
        //    for (int i = 0; i < this.producesPerTurn.Length; i++)
        //    {
        //        this.producesPerTurn[i] = 0;
        //    }
        //}
        //else
        //{
        //    // Some production is defined. Let's set it.
        //    this.producesPerTurn = new int[ResourceManager.allResourceTypes.Length];
        //    for (int i = 0; i < this.producesPerTurn.Length; i++)
        //    {
        //        // If the value is in the dictionary, let's assign that value
        //        if (producesPerTurn.TryGetValue(ResourceManager.allResourceTypes[i], out int value))
        //        {
        //            this.producesPerTurn[i] = value;
        //            // We found a resource this tile produces! Let's note that.
        //            if (value != 0)
        //            {
        //                this.doesProducePerTurn = true;
        //            }
        //        }
        //        else
        //        {
        //            // Otherwise explicitly assign the value to 0.
        //            this.producesPerTurn[i] = 0;
        //        }
        //    }
        //}
    }

    /// <summary>
    /// Helper method to set up resource definitions. This is used to set up per-turn and on-place
    /// resource bonuses, as well as resource cost to place for a given tile.
    /// 
    /// "ourResources" is a reference to an int[] field that we own, which contains the aggregate
    /// values defined in the dictionary "definedResources" (and fills in empty values with 0).
    /// 
    /// "doesProduce" will be true iff there exists at least one nonzero value in "definedResources",
    /// and is optionally used for optimization reasons.
    /// 
    /// "definedResources" is a dictionary of ResourceType : int, containing any nonzero values.
    /// </summary>
    /// <param name="ourResources"></param>
    /// <param name="doesProduce"></param>
    /// <param name="definedResources"></param>
    private void SetupResources(
        ref int[] ourResources, 
        Dictionary<ResourceManager.ResourceType, int> definedResources,
        out bool doesProduce)
    {
        // Assign this value to false to start.
        doesProduce = false;

        // This tile doesn't produce anything. Set to defaults.
        if (definedResources == null)
        {
            ourResources = new int[ResourceManager.allResourceTypes.Length];
            for (int i = 0; i < ourResources.Length; i++)
            {
                ourResources[i] = 0;
            }
        }
        else
        {
            // Some production is defined. Let's set it.
            ourResources = new int[ResourceManager.allResourceTypes.Length];
            for (int i = 0; i < this.producesPerTurn.Length; i++)
            {
                // If the value is in the dictionary, let's assign that value
                if (definedResources.TryGetValue(ResourceManager.allResourceTypes[i], out int value))
                {
                    ourResources[i] = value;
                    // We found a resource this tile produces! Let's note that.
                    if (value != 0)
                    {
                        doesProduce = true;
                    }
                }
                else
                {
                    // Otherwise explicitly assign the value to 0.
                    ourResources[i] = 0;
                }
            }
        }

    }

    public bool HasImprovement()
    {
        return this._improvement != ImprovementType.Empty;
    }
}

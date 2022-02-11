using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [SerializeField]
    public GameObject resourceBar;

    [SerializeField]
    public GameObject resourceEntry;

    [SerializeField]
    public ResourceUI[] resourceEntries;

    public GameObject[] resourceGameObjects;

    // Status of all current resources in the game.
    public static Resource[] resources;

    // Sum of all production values in the game.
    //public int[] produces;

    // Handles all types of resources
    public enum ResourceType
    {
        Population,
        Housing,
        Food,
        Wood,
        Stone
    }

    // Object used to map ResourceType : (string, Sprite) for UI representation
    [System.Serializable]
    public sealed class ResourceUI
    {
        [SerializeField]
        public string name;

        [SerializeField]
        public Sprite image;

        public ResourceUI(string name, Sprite image)
        {
            this.name = name;
            this.image = image;
        }
    }

    public static ResourceType[] allResourceTypes = new ResourceType[]
    {
        ResourceType.Population,
        ResourceType.Housing,
        ResourceType.Food,
        ResourceType.Wood,
        ResourceType.Stone,
    };

    private static readonly Dictionary<ResourceType, Resource> resourceIndexMap = new Dictionary<ResourceType, Resource>();

    /// <summary>
    /// Static initializer to set up some data we'll use.
    /// </summary>
    static ResourceManager()
    {
        // Init all resources
        resources = new Resource[allResourceTypes.Length];
        for (int i = 0; i < allResourceTypes.Length; i++)
        {
            resources[i] = new Resource(allResourceTypes[i]);

            // Map ResourceTypes : Resources
            resourceIndexMap[allResourceTypes[i]] = resources[i];
        }

        // Initialize resource amounts
        resources[1].amount = 3;
        resources[3].amount = 10;
    }

    // in-game resource
    public class Resource
    {
        public int amount;
        public int change;
        public ResourceType resourceType;

        public Resource(ResourceType resourceType)
        {
            this.amount = 0;
            this.change = 0;
            this.resourceType = resourceType;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // TODO UI stuff (stinky)
        //resourceGameObjects = new GameObject[allResources.Length];
        //for (int resource = 0; resource < allResources.Length; resource++)
        //{
        //    GameObject newResource = GameObject.Instantiate(resourceEntry);
        //    resourceGameObjects[resource] = newResource;
        //    newResource.transform.SetParent(resourceBar.transform, worldPositionStays: false);

        //    ResourceEntry entry = newResource.GetComponent<ResourceEntry>();
        //    entry.resourceName = resources[resource].name;
        //    //Debug.Log($"{resources[resource].image.name}");
        //    entry.resourceSprite = resources[resource].image;

        //    entry.Refresh();
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializeChanges(TileType[,] allTiles)
    {
        //produces = new int[allResourceTypes.Length];
        foreach (TileType tile in allTiles)
        {
            if (tile.doesProducePerTurn)
            {
                for (int i = 0; i < tile.producesPerTurn.Length; i++)
                {
                    resources[i].change += tile.producesPerTurn[i];
                }
            }

            // Sum up number of houses to determine housing resource
            // TODO: implement a general system for improvements that give N resources for existing,
            // rather than all being a per-turn basis
            if (tile == TileType.House)
            {
                // TODO don't hardcode this number, lookup enum -> index
                resources[1].amount += 1;
            }
        }

        // Housing = sum of house tiles

        //for (int i = 0; i < produces.Length; i++)
        //{
        //    Debug.Log($"Resource \"{allResourceTypes[i]}\" produces a total of {produces[i]} per turn.");
        //}
    }

    /// <summary>
    /// Returns true iff we can afford to place the tile.
    /// </summary>
    /// <param name="newTile"></param>
    /// <returns></returns>
    public bool CanAffordTile(TileType newTile)
    {
        for (int i = 0; i < resources.Length; i++)
        {
            if (resources[i].amount - newTile.costsOnce[i] < 0)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Swaps the production of "oldTile" for the production of "newTile".
    /// </summary>
    /// <param name="oldTile"></param>
    /// <param name="newTile"></param>
    public void UpdateTile(TileType oldTile, TileType newTile)
    {
        for (int i = 0; i < oldTile.producesPerTurn.Length; i++)
        {
            // Remove any previous production/costs
            //resources[i].change -= oldTile.producesPerTurn[i];
            //resources[i].change -= oldTile.costsPerTurn[i];
            resources[i].change += oldTile.refundsPerTurn[i];

            // Add any new production/costs
            resources[i].change += newTile.producesPerTurn[i];
            resources[i].change += newTile.costsPerTurn[i];
        }

        // Update the actual amounts (e.g. account for costs of tiles)
        for (int i = 0; i < oldTile.producesOnce.Length; i++)
        {
            //resources[i].amount -= oldTile.producesOnce[i];
            resources[i].amount += oldTile.refundsOnce[i];

            resources[i].amount -= newTile.costsOnce[i];
            resources[i].amount += newTile.producesOnce[i];
        }

        // Handle house adding/removal
        //if (oldTile == TileType.House)
        //{
        //    resources[1].amount -= 1;
        //}

        //if (newTile == TileType.House)
        //{
        //    resources[1].amount += 1;
        //}

        // Testing updates per turn.
        // TODO determine if we want per-turn or continuous updates.
        UpdateResources();
        
        foreach (Resource resource in resources)
        {
            Debug.Log($"Have {resource.amount} (+{resource.change} per turn) \"{resource.resourceType}\".");
        }
    }

    /// <summary>
    /// Implements one iteration of resources updating.
    /// </summary>
    public void UpdateResources()
    {
        foreach (Resource resource in resources)
        {
            resource.amount += resource.change;
        }

        resourceIndexMap[ResourceType.Population].amount = PopulationForFood(resourceIndexMap[ResourceType.Food].amount);
        Debug.Log($"We now have a population of {resourceIndexMap[ResourceType.Food].amount} " +
            $"based on {resourceIndexMap[ResourceType.Population].amount} food.");
    }

    /// <summary>
    /// Returns the number of population that a certain number of food can sustain.
    /// 
    /// For now, this is just food / 3, but later should implement some sort of growth.
    /// </summary>
    /// <param name="food"></param>
    /// <returns></returns>
    public static int PopulationForFood(int food)
    {
        return food / 3;
    }
}

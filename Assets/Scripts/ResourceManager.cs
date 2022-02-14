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

    public ResourceEntry[] resourceGameObjects;

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

        [SerializeField]
        public string hoverText;

        public ResourceUI(string name, Sprite image, string hoverText)
        {
            this.name = name;
            this.image = image;
            this.hoverText = hoverText;
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

    private static Dictionary<ResourceType, ResourceEntry> resourceToUIMap = new Dictionary<ResourceType, ResourceEntry>();

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
        resourceGameObjects = new ResourceEntry[allResourceTypes.Length];
        for (int resource = 0; resource < allResourceTypes.Length; resource++)
        {
            GameObject newResource = GameObject.Instantiate(resourceEntry);
            //resourceGameObjects[resource] = newResource;
            newResource.transform.SetParent(resourceBar.transform, worldPositionStays: false);

            ResourceEntry entry = newResource.GetComponent<ResourceEntry>();
            resourceGameObjects[resource] = entry;
            resourceToUIMap.Add(allResourceTypes[resource], entry);

            //entry.resourceInfo = resourceEntries[resource].name;
            Resource res = resourceIndexMap[allResourceTypes[resource]];
            entry.resourceInfo = $"{res.amount} (+{res.change})";
            entry.resourceSprite = resourceEntries[resource].image;
            entry.hover.hoverText = resourceEntries[resource].hoverText;

            entry.Refresh();
        }
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
    /// Returns null iff we can afford to place the tile.
    /// 
    /// Otherwise, returns the resoruce missing.
    /// </summary>
    /// <param name="newTile"></param>
    /// <returns></returns>
    public ResourceType? CanAffordTile(TileType newTile)
    {
        for (int i = 0; i < resources.Length; i++)
        {
            if (resources[i].amount - newTile.costsOnce[i] < 0)
            {
                return resources[i].resourceType;
            }
        }
        return null;
    }

    /// <summary>
    /// Swaps the production of "oldTile" for the production of "newTile".
    /// </summary>
    /// <param name="oldTile"></param>
    /// <param name="newTile"></param>
    public void UpdateTile(TileType oldTile, TileType newTile)
    {
        UpdateResources();

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

        // Update all existing resources.
        // TODO determine if we want per-turn or continuous updates.
        UpdateResourceUI();
        
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

            // Update UI entries. First we grab the appropriate UI element
            //ResourceEntry entry = resourceToUIMap[resource.resourceType];
            //// Then we set its text
            //Debug.Log($"Updating resource type {resource.resourceType}");
            //entry.resourceInfo = $"{resource.amount} (+{resource.change})";
            //entry.Refresh();
        }

        // population = min(food2Pop(food), housing)
        resourceIndexMap[ResourceType.Population].amount = Mathf.Min(
            PopulationForFood(resourceIndexMap[ResourceType.Food].amount),
            resourceIndexMap[ResourceType.Housing].amount
        );
        //Debug.Log($"We now have a population of {resourceIndexMap[ResourceType.Food].amount} " +
        //    $"based on {resourceIndexMap[ResourceType.Population].amount} food.");

            //ResourceEntry entry = newResource.GetComponent<ResourceEntry>();
            ////entry.resourceInfo = resourceEntries[resource].name;
            //Resource res = resourceIndexMap[allResourceTypes[resource]];
            //entry.resourceInfo = $"{res.amount} (+{res.change})";
    }

    public void UpdateResourceUI()
    {
        foreach (Resource resource in resources)
        {
            //resource.amount += resource.change;

            // Update UI entries. First we grab the appropriate UI element
            ResourceEntry entry = resourceToUIMap[resource.resourceType];
            // Then we set its text
            //Debug.Log($"Updating resource type {resource.resourceType}");
            entry.resourceInfo = $"{resource.amount} (+{resource.change})";
            entry.Refresh();
        }
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

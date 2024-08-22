using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWarManager : MonoBehaviour
{ 
    private WorldGenerator worldGenerator;

    public static Dictionary<Vector2Int, int> playerTilesToRadius;

    public static bool[,] visibleList;

    /// <summary>
    /// Helper function to traverse a ring starting at a center point with a given radius.
    /// </summary>
    /// <param name="center"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    private List<HexCoord> TraverseRing(Vector2Int center, int radius)
    {
        List<HexCoord> results = new List<HexCoord>();

        HexCoord hexCenter = new OffsetCoord(center.x, center.y).ToHexCoord();
        HexCoord corner = new HexCoord(HexCoord.DirectionVectors[4]);

        HexCoord hex = hexCenter + (corner * radius);

        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < radius; j++)
            {
                results.Add(hex);
                hex = hex.Neighbor(i);
            }
        }
        return results;
    }

    /// <summary>
    /// Helper function to traverse multiple rings in a spiral pattern. Used to determine all tiles
    /// within a certain radius of a center point.
    /// </summary>
    /// <param name="center"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    private List<HexCoord> TraverseSpiral(Vector2Int center, int radius)
    {
        List<HexCoord> results = new List<HexCoord>() { new OffsetCoord(center.x, center.y).ToHexCoord() };
        for (int i = 1; i <= radius; i++)
        {
            results.AddRange(TraverseRing(center, i));
        }
        return results;
    }

    /// <summary>
    /// Toggles the visibility within a given radius of a target point.
    /// </summary>
    /// <param name="center"></param>
    /// <param name="radius"></param>
    /// <param name="visibility"></param>
    private void MarkVisible(Vector2Int center, int radius, bool visibility=true)
    {
        List<HexCoord> hexCoords = TraverseSpiral(center, radius);
        foreach (HexCoord hex in hexCoords)
        {
            OffsetCoord pos = hex.ToOffsetCoord();
            Debug.Log($"{pos.y}, {pos.x} is now visible!");
            visibleList[pos.y, pos.x] = visibility;
        }
    }

    /// <summary>
    /// Called on startup by the game manager to intialize the state of the fog manager.
    /// </summary>
    public void Generate()
    {
        worldGenerator = this.GetComponent<WorldGenerator>();

        playerTilesToRadius = new Dictionary<Vector2Int, int>()
        {
            { new Vector2Int(worldGenerator.width / 2, worldGenerator.height / 2), 8 }
        };
        visibleList = new bool[worldGenerator.width, worldGenerator.height];

        for (int i = 0; i < worldGenerator.width; i++)
        {
            for (int j = 0; j < worldGenerator.height; j++)
            {
                visibleList[i, j] = false;
            }
        }

        // Run through all visible tiles and mark them as visible
        foreach (KeyValuePair<Vector2Int, int> pair in playerTilesToRadius)
        {
            MarkVisible(pair.Key, pair.Value);
        }

        // Update FOW on startup to initialize the board state
        RefreshFogOfWar();
    }

    /// <summary>
    /// Adds a tile to the list of known player-controlled tiles. This will update the fog of war
    /// based on the radius specified.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="radius"></param>
    public void AddPlayerControlledTile(Vector2Int position, int radius)
    {
        // Keep track of it
        playerTilesToRadius.Add(position, radius);

        // Mark those tiles as visible
        MarkVisible(position, radius);

        // Update
        RefreshFogOfWar();
    }

    /// <summary>
    /// Updates the state of the game's fog of war.
    /// </summary>
    public void RefreshFogOfWar()
    {
        
        for (int i = 0; i < worldGenerator.width; i++)
        {
            for (int j = 0; j < worldGenerator.height; j++)
            {
                // Grab the cell at the given position
                HexCell cell = GameManager.allCells[i, j];

                // If it's meant to be visible..
                if (visibleList[i, j])
                {
                    // And the cell isn't currently visible (lazy)
                    if (!cell.isActive)
                    {
                        // Enable it
                        cell.SetActive(true);
                    }
                }
                else
                {
                    if (cell.isActive)
                    {
                        cell.SetActive(false);
                    }
                }
            }
        }

    }

    // Start is called before the first frame update
    void Start()
    {
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}

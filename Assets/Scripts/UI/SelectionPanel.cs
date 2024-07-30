using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectionPanel : MonoBehaviour
{
    [SerializeField]
    public GameObject selectionButton;

    public GameObject[] buttons;

    public OffsetCoord selectedCoord;

    // Start is called before the first frame update
    void Start()
    {
        //Button button = buttons[0].GetComponent<Button>();
        //button.onClick.AddListener(() => HandleSelection(0));
        //button.GetComponentInChildren<TextMeshProUGUI>().text = "Testing!";
        //ConfigurePanel(new string[] { "a", "b", "c"});
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ConfigurePanel(OffsetCoord coord, List<TileType> tiles)
    {
        Debug.Log($"ConfigurePanel called with {tiles}");
        foreach (TileType tile in tiles)
        {
            Debug.Log($"Tile: {tile}");
        }
        // What coordinate is this selection panel tied to?
        this.selectedCoord = coord;

        // Initialize buttons
        //string[] textOptions = tiles.Select(tile => tile.ToString()).ToArray();
        buttons = new GameObject[tiles.Count];
        for (int i = 0; i < tiles.Count; i++)
        {
            buttons[i] = GameObject.Instantiate(selectionButton);
            buttons[i].transform.SetParent(this.gameObject.transform);
            ConfigureButton(buttons[i].GetComponent<Button>(), tiles[i], i);
        }
    }

    private void ConfigureButton(Button button, TileType tile, int index)
    {
        Debug.Log($"ConfigureButton called with tile={tile}");
        //Button button = buttons[0].GetComponent<Button>();
        button.onClick.AddListener(() => HandleSelection(tile));
        TextMeshProUGUI textMeshPro = button.GetComponentInChildren<TextMeshProUGUI>();
        textMeshPro.text = tile.ToString();
        TooltipHover hover = textMeshPro.gameObject.AddComponent<TooltipHover>();

        string hoverText = "Costs:";
        for (int i = 0; i < ResourceManager.allResourceTypes.Length; i++) {
            if (tile.costsOnce[i] != 0)
            {
                hoverText += $"\n{tile.costsOnce[i]} {ResourceManager.allResourceTypes[i]}";
            }
        }
        // No resource costs?
        if (hoverText.Equals("Costs:"))
        {
            hoverText = "Free to place!";
        }

        hover.hoverText = hoverText;
    }

    public void HandleSelection(TileType tile)
    {
        //Debug.Log($"Button pressed. Attempting to create {tile.ToString()}.");

        GameManager manager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        ResourceManager resourceManager = manager.resourceManager;
        ErrorManager errorManager = manager.errorManager;

        // Can't replace with the same tile
        TileType existingTile = GameManager.allTiles[selectedCoord.y, selectedCoord.x];
        if (existingTile == tile)
        {
            errorManager.CreateError(this.selectedCoord.ToHexCoord().ToWorld(), "Already placed!");
            return;
        }

        // Can't build if missing a resource
        ResourceManager.ResourceType? missingResource = resourceManager.CanAffordTile(tile);
        if (missingResource != null)
        {
            errorManager.CreateError(
                this.selectedCoord.ToHexCoord().ToWorld(), 
                $"Can't afford!\nNot enough {missingResource}."
            );
            return;
        }

        // Checks out- let's build it.
        manager.worldGenerator.ReplaceTile(
            GameManager.allCells,
            GameManager.allTiles,
            this.selectedCoord.y,
            this.selectedCoord.x,
            tile);
    }
}

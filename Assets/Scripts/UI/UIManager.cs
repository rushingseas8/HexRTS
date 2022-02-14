using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    public GameObject canvas;

    [SerializeField]
    public Tooltip tooltip;

    private GraphicRaycaster raycaster;
    private PointerEventData m_PointerEventData;
    private EventSystem m_EventSystem;

    [SerializeField]
    public GameObject selectionPanelParent;

    [SerializeField]
    public GameObject selectionPanelPrefab;

    private GameObject selectionPanel = null;

    void Start()
    {
        raycaster = canvas.GetComponent<GraphicRaycaster>();
    }

    public bool HasUI()
    {
        //Set up the new Pointer Event
        m_PointerEventData = new PointerEventData(m_EventSystem);
        //Set the Pointer Event Position to that of the mouse position
        m_PointerEventData.position = Input.mousePosition;
        

        //Create a list of Raycast Results
        List<RaycastResult> results = new List<RaycastResult>();

        raycaster.Raycast(m_PointerEventData, results);
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.name != "MainPanel" && result.gameObject.name != "Game Area" && result.gameObject.name != "Tooltip")
            {
                if (result.gameObject.TryGetComponent<TooltipHover>(out TooltipHover tooltipHover))
                {
                    Debug.Log("Hit an object with tooltip hover component!");
                    tooltip.Show($"{tooltipHover.hoverText}");
                }
                else
                {
                    tooltip.Hide();
                }

                //Debug.Log("Hit " + result.gameObject.name);
                return true;
            }
        }
        return false;
    }

    public void NewSelectionPanel(OffsetCoord coord, List<TileType> tiles)
    {
        if (this.selectionPanel == null)
        {
            GameObject newSelectionPanel = GameObject.Instantiate(selectionPanelPrefab);
            SelectionPanel panel = newSelectionPanel.GetComponent<SelectionPanel>();
            panel.ConfigurePanel(coord, tiles);
            newSelectionPanel.transform.SetParent(selectionPanelParent.transform);
            // Place it in the middle so it gets vertically aligned properly
            newSelectionPanel.transform.SetSiblingIndex(1);

            this.selectionPanel = newSelectionPanel;
        } 
        else
        {
            Debug.LogError("Double initialize on selection panel.");
        }
    }

    public void DeleteSelectionPanel()
    {
        if (this.selectionPanel != null)
        {
            GameObject.Destroy(this.selectionPanel);
            this.selectionPanel = null;
        }
        else
        {
            Debug.LogError("Double delete on selection panel.");
        }
    }
}

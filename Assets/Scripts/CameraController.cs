using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    // Handle on the camera GameObject
    //public static GameObject worldCamera;

    [SerializeField]
    public Camera controlledCamera;

    // How much of a border around the edge is a trigger for camera movement
    public const float HorizontalScrollBorder = 0.03f;
    public const float VerticalScrollBorder = 0.02f;

    // Adjusts how fast the camera zooms
    public const float ZoomScaleFactor = 5.0f;

    // How fast we scroll in horizontal/vertical directions
    public static readonly Vector3 MaxHorizontalScroll = new Vector3(12f, 0, 0);
    public static readonly Vector3 MaxVerticalScroll = new Vector3(0, 0, 9f);

    // Float from [0, 1] that determines how zoomed in we are. 0 is close, 1 is far.
    public float zoomLevel = 0.33f;

    // The point that the camera is looking at.
    public Vector3 lookatPoint = Vector3.zero;

    // The offset position from the lookatPoint that the camera is at.
    public static readonly Vector3 CameraOffsetVector = new Vector3(0, 7.5f, -10f);

    [SerializeField]
    public GameObject hoverCursor;
    
    [SerializeField]
    public GameObject targetCursor;
    
    // Which cell is currently being hovered over, if any.
    public HexCell targetedCell;

    // Which cell is currently being clicked on, if any.
    public HexCell selectedCell;

    // Start is called before the first frame update
    void Start()
    {
        // Init camera
        //controlledCamera.transform.position = new Vector3(0, 7.5f, 0);
        controlledCamera.transform.rotation = Quaternion.AngleAxis(45, Vector3.right);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePosRaw = Input.mousePosition;
        Vector3 mousePos = new Vector3(
            Mathf.Clamp(mousePosRaw.x, 0, Screen.width), 
            Mathf.Clamp(mousePosRaw.y, 0, Screen.height));

        // We'll build up the camera's desired movement amount
        Vector3 moveDelta = Vector3.zero;

        // Based on how zoomed out we are, we should move faster or slower.
        float moveScale = Mathf.Lerp(0.5f, 2.0f, zoomLevel);

        if (mousePos.x < Screen.width * HorizontalScrollBorder)
        {
            // This is a left movement.

            // Fraction scales from 0 at the start of the border to 1 at the edge;
            // For a width of 1000 and a border of 200, this means we have:
            // frac = 0 when mousePos.x = 200
            // frac = 1 when mousePos.x = 0
            float frac = 1.0f - (mousePos.x / (Screen.width * HorizontalScrollBorder));
            moveDelta -= moveScale * frac * MaxHorizontalScroll;
        }
        else if (mousePos.x > Screen.width * (1.0 - HorizontalScrollBorder))
        {
            // Right
            // Scales from 0 at (1.0 - border)% right to 1 at 100% right
            float frac = (mousePos.x - (Screen.width * (1.0f - HorizontalScrollBorder))) / (Screen.width * HorizontalScrollBorder);
            moveDelta += moveScale * frac * MaxHorizontalScroll;
        }

        if (mousePos.y < Screen.height * VerticalScrollBorder)
        {
            // Down
            float frac = 1.0f - (mousePos.y / (Screen.height * VerticalScrollBorder));
            moveDelta -= moveScale * frac * MaxVerticalScroll;
        }
        else if (mousePos.y > Screen.height * (1.0 - VerticalScrollBorder))
        {
            // Up
            float frac = (mousePos.y - (Screen.height * (1.0f - VerticalScrollBorder))) / (Screen.height * VerticalScrollBorder);
            moveDelta += moveScale * frac * MaxVerticalScroll;
        }

        // Modify the zoom level based on the scroll wheel
        zoomLevel -= ZoomScaleFactor * Input.mouseScrollDelta.y * Time.deltaTime;
        zoomLevel = Mathf.Clamp(zoomLevel, 0, 1f);

        float cameraAngle = Mathf.Lerp(45, 70f, zoomLevel);
        Quaternion newCameraRot = Quaternion.AngleAxis(cameraAngle, Vector3.right);
        float cameraFov = Mathf.Lerp(55f, 65f, zoomLevel);
        //float cameraFov = Mathf.Lerp(30f, 80f, zoomLevel);

        // Implement the actual camera movement
        // Old and kinda finicky.
        //worldCamera.transform.position += Time.deltaTime * moveDelta;
        //Vector3 newCameraPos = worldCamera.transform.position + (Time.deltaTime * moveDelta);
        //newCameraPos = new Vector3(newCameraPos.x, Mathf.Lerp(5f, 20f, zoomLevel), newCameraPos.z);
        //worldCamera.transform.position = newCameraPos;
        //worldCamera.transform.rotation = newCameraRot;
        //worldCamera.GetComponent<Camera>().fieldOfView = cameraFov;

        // Move the camera target point
        lookatPoint += (Time.deltaTime * moveDelta);
        // Adjust the camera position to be the target + (zoom level * default offset vector).
        controlledCamera.transform.position = lookatPoint + (moveScale * CameraOffsetVector);

        //Vector3 mousePos3d = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f);
        //Vector3 mouseWorldPos3d = worldCamera.GetComponent<Camera>().ScreenToWorldPoint(mousePos3d);
        ////if (mouseCursor != null)
        ////{
        ////    mouseCursor.transform.position = mouseWorldPos3d;
        ////}
        ////Debug.Log($"Mouse pos in world: {mouseWorldPos3d}");

        //if (Physics.Raycast(
        //    mouseWorldPos3d, 
        //    -CameraOffsetVector, 
        //    out RaycastHit hit, 
        //    Mathf.Infinity, 
        //    LayerMask.GetMask("Terrain")))
        //{
        //    GameObject hitObj = hit.collider.gameObject;
        //    if (hitObj != null)
        //    {
        //        //Debug.Log($"Object: {hitObj}");
        //        HexCell cell = hitObj.GetComponentInParent<HexCell>();
        //        if (cell != null)
        //        {
        //            Debug.Log($"Hovering over {cell.coord.ToString()}");
        //            mouseCursor.transform.position = hitObj.transform.position;
        //        }
        //    }
        //}

        // Checks if we're hovering over the UI; if so, stop updating the raycast selection.
        //if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        //GameObject hoveringGO = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        //if (hoveringGO != null)
        //{
        //    Debug.Log($"Pointing over game object: {hoveringGO}");
        //    return;
        //}
        
        GameManager man = GameObject.Find("Game Manager").GetComponent<GameManager>();
        bool hitUI = man.uiManager.HasUI();
        if (hitUI)
        {
            Debug.Log("Hit some UI, ignoring.");
            return;
        }


        // Ray from mouse position in 3d space in the direction camera is looking
        Ray ray = controlledCamera.ScreenPointToRay(Input.mousePosition);
        
        // Raycast forward until we hit a terrain object
        if (Physics.Raycast(ray, out RaycastHit hit, LayerMask.GetMask("Terrain"))) {
            GameObject hitObject = hit.transform.gameObject;
            HexCell hitCell = hitObject.GetComponentInParent<HexCell>() ?? null;
            
            if (hitCell != null)
            {
                Debug.Log($"Hit object: {hitCell} at Hex: {hitCell.coord} Offset: {hitCell.coord.ToOffsetCoord()}");
                // Move the cursor (slightly above) the terrain object hit
                hoverCursor.transform.position = hitObject.transform.position + new Vector3(0, 0.01f, 0);
                this.targetedCell = hitCell;
            }
        }
        if (this.targetedCell != null)
        {
            GameManager manager = GameObject.Find("Game Manager").GetComponent<GameManager>();
            if (Input.GetMouseButtonDown(0))
            {
                if (this.selectedCell != null)
                {
                    manager.uiManager.DeleteSelectionPanel();
                }

                this.selectedCell = this.targetedCell;
                targetCursor.transform.position = hoverCursor.transform.position;
                targetCursor.SetActive(true);

                // Not defined? That's okay- assume nothing can be built for now
                if (!TileType.CanPlace.ContainsKey(this.selectedCell.tileType))
                {
                    Debug.LogWarning($"Failed to lookup {this.selectedCell.tileType} in TileType.CanPlace");
                }
                else
                {
                    // Otherwise, look up what TileTypes this TileType can transform into
                    List<TileType> placeableTiles = TileType.CanPlace[this.selectedCell.tileType];
                    manager.uiManager.NewSelectionPanel(this.selectedCell.coord.ToOffsetCoord(), placeableTiles);
                }

            }

            if (Input.GetMouseButtonDown(1))
            {
                if (this.selectedCell != null)
                {
                    manager.uiManager.DeleteSelectionPanel();
                    targetCursor.SetActive(false);
                    this.selectedCell = null;
                }
            }


            //OffsetCoord hitCoord = selectedCell.coord.ToOffsetCoord();
            //if (Input.GetMouseButtonDown(0))
            //{
            //    if (selectedCell.tileType == TileType.Grass)
            //    {
            //        //Debug.Log("Replacing with lumber camp");
            //        if (!manager.resourceManager.CanAffordTile(TileType.House))
            //        {
            //            Debug.Log("Can't afford to place a house down.");
            //        }
            //        else
            //        {
            //            manager.worldGenerator.ReplaceTile(
            //                GameManager.allCells, 
            //                GameManager.allTiles, 
            //                hitCoord.x,
            //                hitCoord.y,
            //                TileType.House);
            //        }

            //    }
                
            //    if (selectedCell.tileType == TileType.Forest)
            //    {
            //        if (!manager.resourceManager.CanAffordTile(TileType.House))
            //        {
            //            Debug.Log("Can't afford to place a lumber camp down.");
            //        }
            //        else
            //        {
            //            manager.worldGenerator.ReplaceTile(
            //                GameManager.allCells, 
            //                GameManager.allTiles, 
            //                hitCoord.x,
            //                hitCoord.y,
            //                TileType.LumberCamp);

            //        }
            //        //Debug.Log("Replacing with lumber camp");
            //    }
            //} else if (Input.GetMouseButtonUp(0))
            //{
            //    //Debug.Log("Replacing with forest");
            //    //manager.worldGenerator.ReplaceTile(
            //    //    GameManager.allCells, 
            //    //    GameManager.allTiles, 
            //    //    hitCoord.x,
            //    //    hitCoord.y,
            //    //    TileType.Forest);

            //}

        }
        
    }
}

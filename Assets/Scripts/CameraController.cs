using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Handle on the camera GameObject
    public static GameObject worldCamera;

    // How much of a border around the edge is a trigger for camera movement
    public const float HorizontalScrollBorder = 0.15f;
    public const float VerticalScrollBorder = 0.10f;

    // Adjusts how fast the camera zooms
    public const float ZoomScaleFactor = 5.0f;

    // How fast we scroll in horizontal/vertical directions
    public static readonly Vector3 MaxHorizontalScroll = new Vector3(12f, 0, 0);
    public static readonly Vector3 MaxVerticalScroll = new Vector3(0, 0, 9f);

    // Float from [0, 1] that determines how zoomed in we are. 0 is close, 1 is far.
    public float zoomLevel = 0;

    // The point that the camera is looking at.
    public Vector3 lookatPoint = Vector3.zero;

    // The offset position from the lookatPoint that the camera is at.
    public static readonly Vector3 CameraOffsetVector = new Vector3(0, 7.5f, -10f);

    // Start is called before the first frame update
    void Start()
    {
        // Init camera
        worldCamera = GameObject.Find("Main Camera");
        worldCamera.transform.position = new Vector3(0, 7.5f, 0);
        worldCamera.transform.rotation = Quaternion.AngleAxis(45, Vector3.right);
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
        float moveScale = Mathf.Lerp(1.0f, 2.0f, zoomLevel);

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
        worldCamera.transform.position = lookatPoint + (moveScale * CameraOffsetVector);
        
    }
}

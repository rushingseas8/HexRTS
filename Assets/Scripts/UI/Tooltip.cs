using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    //[SerializeField]
    //public GameObject tooltipGameObject;

    [SerializeField]
    public RectTransform tooltipParent;

    [SerializeField]
    public TextMeshProUGUI tooltipText;

    // Start is called before the first frame update
    void Start()
    {
        tooltipText.text = "Hello world";

        tooltipParent.anchorMin = new Vector2(1, 0);
        tooltipParent.anchorMax = new Vector2(0, 1);

        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Input.mousePosition;
        float screenX = mousePos.x;
        float screenY = mousePos.y;

        tooltipParent.position = mousePos;
        tooltipParent.pivot = new Vector2(0.5f, 0.5f);
        if (screenX < Screen.width / 2.0f)
        {
            if (screenY < Screen.height / 2.0f)
            {
                // Bottom-left of screen, so put tooltip on top-right
                tooltipParent.pivot = new Vector2(0.0f, 0.0f);
            }
            else
            {
                // Top-left, so go bottom-right
                tooltipParent.pivot = new Vector2(0.0f, 1.0f);
            }
        }
        else
        {
            if (screenY < Screen.height / 2.0f)
            {
                // Bottom-right of screen, so put tooltip on top-left
                tooltipParent.pivot = new Vector2(1.0f, 0.0f);
            }
            else
            {
                // Top-right, so go bottom-left
                tooltipParent.pivot = new Vector2(1.0f, 1.0f);
            }
        }

    }

    public void Show(string text)
    {
        // Update text
        tooltipText.text = text.Replace("\\n", "\n");

        // Update position before showing
        Update();
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}

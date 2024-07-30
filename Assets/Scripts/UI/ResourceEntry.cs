using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceEntry : MonoBehaviour
{
    public Image resourceImage;
    public TextMeshProUGUI resourceText;
    public Color color = Color.black;

    [SerializeField]
    public string resourceInfo;

    [SerializeField]
    public Sprite resourceSprite;

    [SerializeField]
    public TooltipHover hover;

    // Start is called before the first frame update
    void Start()
    {
        Refresh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Refresh()
    {
        resourceText.text = resourceInfo;
        resourceText.color = color;
        resourceText.alpha = 1;
        resourceImage.sprite = resourceSprite;
    }
}

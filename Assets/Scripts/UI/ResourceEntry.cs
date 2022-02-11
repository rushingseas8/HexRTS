using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceEntry : MonoBehaviour
{
    public Image resourceImage;
    public Text resourceText;

    [SerializeField]
    public string resourceName;

    [SerializeField]
    public Sprite resourceSprite;


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
        resourceText.text = resourceName;
        resourceImage.sprite = resourceSprite;
    }
}

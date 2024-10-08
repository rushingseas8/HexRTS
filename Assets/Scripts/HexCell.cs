﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HexCell : MonoBehaviour
{
    [SerializeField]
    public HexCoord coord;

    [SerializeField]
    public TileType tileType;

    [SerializeField]
    public GameObject tile;

    [SerializeField]
    public GameObject improvement;

    public bool isActive = true;

    public void SetActive(bool visible)
    {
        tile.SetActive(visible);
        if (improvement != null ) {
            improvement.SetActive(visible);
        }

        // Update our internal flag
        isActive = visible;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Representation of "even-q" offset coordinates
/// </summary>
public class OffsetCoord
{
    [SerializeField]
    public int x, y;

    public OffsetCoord(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public HexCoord ToHexCoord()
    {
        int q = y;
        int r = x - (y + (y & 1)) / 2;
        return new HexCoord(q, r, -q - r);
    }
}

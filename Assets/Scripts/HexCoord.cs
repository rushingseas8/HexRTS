using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HexCoord
{
    [SerializeField]
    public int q, r, s;
        
    private const float HEIGHT = 1.73205f;
    private const float WIDTH = 2.0f;

    //private readonly Vector3 POS_Q = new Vector3(HEIGHT, 0, 0);
    //private readonly Vector3 POS_R = new Vector3(HEIGHT * -0.5f, 0, WIDTH * 0.75f);
    //private readonly Vector3 POS_S = new Vector3(HEIGHT * 0.5f, 0, WIDTH * 0.75f);

    private readonly Vector3 POS_Q = new Vector3(0, 0, -WIDTH);
    private readonly Vector3 POS_R = new Vector3(-0.5f * HEIGHT, 0, -0.25f * WIDTH);
    private readonly Vector3 POS_S = new Vector3(0.5f * HEIGHT, 0, -0.25f * WIDTH);

    public HexCoord(int q, int r, int s)
    {
        this.q = q;
        this.r = r;
        this.s = s;
    }

    public HexCoord(Vector3Int vec)
    {
        this.q = vec[0];
        this.r = vec[1];
        this.s = vec[2];
    }

    public Vector3 ToWorld()
    {
        return q * POS_Q + r * POS_R + s * POS_S;
    }

    public OffsetCoord ToOffsetCoord()
    {
        int x = q;
        int y = r + (q + (q & 1)) / 2;
        return new OffsetCoord(x, y);
    }
}

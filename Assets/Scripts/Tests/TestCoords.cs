using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TestCoords : MonoBehaviour
{
    private void Start()
    { 
        TestEquality();
    }
    public void TestEquality()
    {
        for (int i = -50; i < 50; i++)
        {
            for (int j = -50; j < 50; j++)
            {
                OffsetCoord offsetCoord = new OffsetCoord(i, j);
                OffsetCoord twiceConverted = offsetCoord.ToHexCoord().ToOffsetCoord();
                Assert.AreEqual(twiceConverted.x, offsetCoord.x);
                Assert.AreEqual(twiceConverted.y, offsetCoord.y);
            }
        }
    }
}

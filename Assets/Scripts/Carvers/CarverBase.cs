using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract class reprensenting a "Carver", transforming an entire finite world
/// with custom global logic. Think Minecraft rivers or ravines. 
/// </summary>
public abstract class CarverBase
{
    public abstract void Carve(ref TileType[,] tiles, PerlinGenerator noise);

}
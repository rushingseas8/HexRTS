using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class CarverRiver : CarverBase
{
    WorldGenBase worldGenBase;

    public CarverRiver()
    {
        worldGenBase = new WorldGenZoom();
    }

    private static float NoiseToHeight(float noise)
    {
        float height = -6 * (noise - 0.5f);
        height = (int)(height * 4) / 4.0f;
        height += 0.25f;
        return height;
    }

    private void RiverStep(ref TileType[,] tiles, PerlinGenerator noise, Vector2Int point)
    {
        Debug.Log($"RiverStep iterating on point: {point}");
        TileType currentTile = this.worldGenBase.GetOrEmpty(tiles, point.x, point.y);

        // Stop once we've hit the edge of the world or water
        if (currentTile == TileType.Empty || currentTile == TileType.Water)
        {
            return;
        }
        int x = point.x;
        int z = point.y;

        // Set the point to water
        tiles[x, z] = TileType.Water;

        var neighbors = new List<Tuple<int, int, float>>();
        float lowestHeight = NoiseToHeight(noise.GetValueNormalized(x, z));
        for (int direction = 0; direction < 6; direction++)
        {
            int parity = x & 1;
            int newX = x + WorldGenBase.EVENQ_OFFSETS[parity, direction, 0];
            int newZ = z + WorldGenBase.EVENQ_OFFSETS[parity, direction, 1];
            if (newX < 0 || newX >= tiles.GetLength(0))
            {
                continue;
            }
        
            if (newZ < 0 || newZ >= tiles.GetLength(1))
            {
                continue;
            }

            float noiseVal = noise.GetValueNormalized(newX, newZ);
            float height = NoiseToHeight(noiseVal);

            if (height <= lowestHeight)
            {
                // Found a new best, so remove the rest
                if (height < lowestHeight)
                {
                    neighbors.Clear();
                }

                // Add this new candidate (either it's the new best, or equal to the previous best)
                neighbors.Add(new Tuple<int, int, float>(newX, newZ, height));
            }
        }

        // Run through all neighbors and pick a random one
        Debug.Log($"Got {neighbors.Count} neighbors.");
        Tuple<int, int, float> randomNeighbor = neighbors[UnityEngine.Random.Range(0, neighbors.Count)];
        
        // Great, we got a best direction to traverse in. Replace it with water and move on.
        RiverStep(ref tiles, noise, new Vector2Int(randomNeighbor.Item1, randomNeighbor.Item2));

        //// Else we'll look around us for the next lowest point
        //int bestX = x;
        //int bestZ = z;
        //float bestNoise = noise.GetValueNormalized(x, z);
        //for (int direction = 0; direction < 6; direction++)
        //{
        //    //toReturn[direction] = GetNeighbor(tiles, x, z, direction);
        
        //    // Get the tile in the sample direction
        //    int parity = x & 1;
        //    int newX = x + WorldGenBase.EVENQ_OFFSETS[parity, direction, 0];
        //    int newZ = z + WorldGenBase.EVENQ_OFFSETS[parity, direction, 1];
        //    if (newX < 0 || newX >= tiles.GetLength(0))
        //    {
        //        continue;
        //    }
        
        //    if (newZ < 0 || newZ >= tiles.GetLength(1))
        //    {
        //        continue;
        //    }

        //    // Get the noise in that direction
        //    float noiseVal = noise.GetValueNormalized(newX, newZ);

        //    // If it's lower than our current best, take it as the best
        //    if (noiseVal > bestNoise) {
        //        bestX = newX;
        //        bestZ = newZ;
        //        bestNoise = noiseVal;
        //    }

        //    //return this.worldGenBase.GetOrEmpty(tiles, newX, newZ);
        //}

        //// Great, we got a best direction to traverse in. Replace it with water and move on.
        //RiverStep(ref tiles, noise, new Vector2Int(bestX, bestZ));
    }

    public override void Carve(ref TileType[,] tiles, PerlinGenerator noise)
    {
        float minDistance = 5.0f;
        float minHeight = 0.25f;
        int maxSpawns = 6;
        var pointsOfInterest = new List<Vector2Int>();

        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            for (int j = 0; j < tiles.GetLength(1); j++)
            {
                // Too short, don't care about this point
                if (noise.GetValueNormalized(i, j) >= minHeight)
                {
                    continue;
                }

                bool validPoint = true;
                for (int k = 0; k < pointsOfInterest.Count; k++) {
                    Vector2Int testPoint = pointsOfInterest[k];
                    // If it's too close to any point, it's considered invalid
                    if (Vector2.Distance(testPoint, new Vector2(i, j)) <= minDistance) {
                        validPoint = false;
                        break;
                    }
                }

                // It's valid, so we add it
                if (validPoint) {
                    pointsOfInterest.Add(new Vector2Int(i, j));
                }
            }
        }

        Debug.Log($"Got {pointsOfInterest.Count} points of interest.");
        for (int i = 0; i < Math.Min(maxSpawns, pointsOfInterest.Count); i++) {
            Vector2Int vector2Int = pointsOfInterest[i];
            this.RiverStep(ref tiles, noise, vector2Int);
        }
    }
}
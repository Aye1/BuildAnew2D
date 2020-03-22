using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCluster
{
    public List<BaseTileData> tiles;
    public int id;

    public int FloodLevel { get; private set; }

    public WaterCluster(int id)
    {
        tiles = new List<BaseTileData>();
        this.id = id;
    }

    public void AddTile(BaseTileData tile)
    {
        if(!tiles.Contains(tile) && tile.terrainTile is WaterTile)
        {
            tiles.Add(tile);
            ((WaterTile)tile.terrainTile).clusterId = id;
        }
    }

    public void RemoveTile(BaseTileData tile)
    {
        tiles.Remove(tile);
        ((WaterTile)tile.terrainTile).clusterId = 0;
    }

    public void UpdateFloodAmount()
    {
        FloodLevel = 0;
        foreach(BaseTileData tile in tiles)
        {
            FloodLevel += ((WaterTile)tile.terrainTile).FloodLevel;
        }
    }

    public void RemoveFlood(int amount)
    {
        int individualAmount = amount / tiles.Count - 1;
        tiles.ForEach(x => ((WaterTile)x.terrainTile).FloodLevel -= individualAmount);
    }

    public void BalanceCluster()
    {
        UpdateFloodAmount();
        int newIndividualFloodAmount = FloodLevel / tiles.Count;
        tiles.ForEach(x => ((WaterTile)x.terrainTile).FloodLevel = newIndividualFloodAmount);
    }
}

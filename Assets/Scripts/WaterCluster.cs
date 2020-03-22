using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
            ((WaterTile)tile.terrainTile).cluster = this;
        }
    }

    // Maybe useless?
    public void RemoveTile(BaseTileData tile)
    {
        tiles.Remove(tile);
        ((WaterTile)tile.terrainTile).cluster = null;
    }

    public void AddFlood(int amount)
    {
        RecountFloodLevel();
    }

    public void RemoveFlood(int amount)
    {
        int individualAmount = amount / tiles.Count;
        int remainingAmount = amount;
        int remainingTiles = tiles.Count;

        IOrderedEnumerable<BaseTileData> orderedTiles = tiles.OrderBy(x => ((WaterTile)x.terrainTile).FloodLevel);
        foreach(BaseTileData tile in orderedTiles)
        {
            individualAmount = remainingAmount / remainingTiles;

            int realAmount = ((WaterTile)tile.terrainTile).RemoveFlood(individualAmount);
            remainingAmount -= realAmount;
            remainingTiles--;
        }
        RecountFloodLevel();
    }

    public void RecountFloodLevel()
    {
        FloodLevel = 0;
        tiles.ForEach(x => FloodLevel += ((WaterTile)x.terrainTile).FloodLevel);
    }
}

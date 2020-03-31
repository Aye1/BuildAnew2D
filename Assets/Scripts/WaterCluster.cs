﻿using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WaterCluster
{
    public List<BaseTileData> tiles;
    public int id;

    private int _pendingFloodModification = 0;

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
        }
    }

    // Warning: does not rebalance the flood amount between all tiles
    // This method should not be called directly
    // Use 
    public void RemoveFlood(int amount)
    {
        _pendingFloodModification -= amount;
        //FloodLevel = Mathf.Max(0, FloodLevel - amount);
    }

    private void ApplyPendingFloodModifications()
    {
        FloodLevel = Mathf.Max(0, FloodLevel + _pendingFloodModification);
        _pendingFloodModification = 0;
    }

    public Dictionary<BaseTileData, int> BalanceFlood()
    {
        Dictionary<BaseTileData, int> oldFloodLevels = new Dictionary<BaseTileData, int>();
        RecountFloodLevel();
        ApplyPendingFloodModifications();
        int remainingFlood = FloodLevel;
        int remainingTiles = tiles.Count;

        // Try to keep the same flood balancing among the cluster
        // If the tile had low water compared to the cluster, it will still have low water after the balancing
        IEnumerable<BaseTileData> orderedTiles = tiles.OrderBy(x => ((WaterTile)x.terrainTile).NTFloodLevel);

        foreach (BaseTileData tile in orderedTiles)
        {
            oldFloodLevels.Add(tile, ((WaterTile)tile.terrainTile).NTFloodLevel);
            int individualAmount = remainingFlood / remainingTiles;
            ((WaterTile)tile.terrainTile).NTFloodLevel = individualAmount;
            remainingFlood -= individualAmount;
            remainingTiles--;
        }
        RecountFloodLevel();
        return oldFloodLevels;
    }

    public void RecountFloodLevel()
    {
        FloodLevel = tiles.Sum(x => ((WaterTile)x.terrainTile).NTFloodLevel);
    }
}

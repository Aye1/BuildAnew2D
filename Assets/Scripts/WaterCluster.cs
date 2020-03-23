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
        }
    }

    public void RemoveFlood(int amount)
    {
        FloodLevel = Mathf.Max(0, FloodLevel - amount);
        BalanceFlood(false);
    }

    public void BalanceFlood(bool forceRecount)
    {
        if(forceRecount)
        {
            RecountFloodLevel();
        }
        int remainingFlood = FloodLevel;
        int remainingTiles = tiles.Count;

        // Try to keep the same flood balancing among the cluster
        // If the tile had low water compared to the cluster, it will still have low water after the balancing
        IEnumerable<BaseTileData> orderedTiles = tiles.OrderBy(x => ((WaterTile)x.terrainTile).NTFloodLevel);

        foreach (BaseTileData tile in orderedTiles)
        {
            int individualAmount = remainingFlood / remainingTiles;
            ((WaterTile)tile.terrainTile).NTFloodLevel = individualAmount;
            remainingFlood -= individualAmount;
            remainingTiles--;
        }
        RecountFloodLevel();
    }

    public void RecountFloodLevel()
    {
        FloodLevel = tiles.Sum(x => ((WaterTile)x.terrainTile).NTFloodLevel);
    }
}

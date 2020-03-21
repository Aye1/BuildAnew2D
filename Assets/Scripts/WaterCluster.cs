using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCluster
{
    public List<WaterTile> cells;
    public int id;

    private int floodLevel;

    public WaterCluster(int id)
    {
        cells = new List<WaterTile>();
        this.id = id;
    }

    public void AddTile(WaterTile tile)
    {
        if(!cells.Contains(tile))
        {
            cells.Add(tile);
            tile.clusterId = id;
        }
    }

    public void RemoveTile(WaterTile tile)
    {
        cells.Remove(tile);
        tile.clusterId = 0;
    }

    public void IncreaseFlood(int amount)
    {
        floodLevel += amount;
    }

}

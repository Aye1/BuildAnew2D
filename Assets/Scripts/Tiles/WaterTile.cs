using System;
using System.Collections.Generic;

public class WaterTile : TerrainTile
{
    public int FloodLevel { get; set; } = 0;
    public WaterCluster cluster;

    public override TerrainType GetTerrainType()
    {
        return TerrainType.Water;
    }

    public override void OnTurnStarts(IEnumerable<BaseTileData> neighbours)
    {
        FloodLevel++;
        cluster.AddFlood(1);
    }

    public override string GetDebugText()
    {
        return FloodLevel.ToString();
    }

    public int RemoveFlood(int amount)
    {
        int amountToRemove = amount >= FloodLevel ? FloodLevel : amount;
        FloodLevel -= amountToRemove;
        return amountToRemove;
    }
}

using System;
using System.Collections.Generic;

public class WaterTile : TerrainTile
{
    public int FloodLevel { get; set; }
    public int clusterId;

    public override TerrainType GetTerrainType()
    {
        return TerrainType.Water;
    }

    public override void OnTurnStarts(IEnumerable<BaseTileData> neighbours)
    {
        UpFlood();
    }

    public override string GetDebugText()
    {
        return FloodLevel.ToString();
    }

    private void UpFlood()
    {
        // Should not be on all maps
        FloodLevel++;
    }

    public void Drain()
    {
        FloodLevel = Math.Max(0, FloodLevel - 2);
    }
}

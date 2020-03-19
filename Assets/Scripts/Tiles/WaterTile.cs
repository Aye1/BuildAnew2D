using System;

public class WaterTile : TerrainTile
{
    private int floodLevel;
    public override TerrainType GetTerrainType()
    {
        return TerrainType.Water;
    }

    public override void OnTurnStarts(BaseTileData[] neighbours)
    {
        UpFlood();
    }

    public override string GetDebugText()
    {
        return floodLevel.ToString();
    }

    private void UpFlood()
    {
        // Should not be on all maps
        floodLevel++;
    }

    public void Drain()
    {
        floodLevel = Math.Max(0, floodLevel - 2);
    }
}

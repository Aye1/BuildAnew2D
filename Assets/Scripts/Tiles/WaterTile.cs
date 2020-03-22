using System;
using System.Collections.Generic;

public class WaterTile : TerrainTile
{
    public int FloodLevel { get; set; } = 0;
    public int NTFloodLevel { get; set; } = 0;
    public bool isInCluster;

    public override TerrainType GetTerrainType()
    {
        return TerrainType.Water;
    }

    public override void OnTurnStarts(IEnumerable<BaseTileData> neighbours)
    {
        FloodLevel++;
    }

    public override void PredictOnTurnStarts(IEnumerable<BaseTileData> neighbours)
    {
        NTFloodLevel++;
    }

    public override void ApplyPrediction()
    {
        FloodLevel = NTFloodLevel;
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

using System;
using System.Collections.Generic;

public class WaterTile : TerrainTile
{
    public int FloodLevel { get; set; } = 0;
    public int NTFloodLevel { get; set; } = 0;
    public bool clusterFlag;

    public override TerrainType GetTerrainType()
    {
        return TerrainType.Water;
    }

    public override void OnTurnStarts(IEnumerable<BaseTileData> neighbours)
    {
    }

    public override void PredictOnTurnStarts(IEnumerable<BaseTileData> neighbours)
    {
        ApplyPrediction();
        OnSpecificTileModified();
    }

    public override void ApplyPrediction()
    {
        FloodLevel = NTFloodLevel;
    }

    public override string GetDebugText()
    {
        return FloodLevel.ToString() + "(" + NTFloodLevel +")";
    }

    public void IncrementFlood()
    {
        NTFloodLevel++;
    }

    public void DecrementFlood()
    {
        NTFloodLevel--;
    }

    public void AddFlood(int amount)
    { 
        NTFloodLevel += amount;
    }

    public int RemoveFlood(int amount)
    {
        int amountToRemove = amount >= NTFloodLevel ? NTFloodLevel : amount;
        NTFloodLevel -= amountToRemove;
        return amountToRemove;
    }
}

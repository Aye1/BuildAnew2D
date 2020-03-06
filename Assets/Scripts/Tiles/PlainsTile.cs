using UnityEngine;
using UnityEngine.Tilemaps;

public class PlainsTile : BaseTileData
{
    private int debugLevel = 1;


    public override void OnTurnEnds(BaseTileData[] neighbours)
    {
    }

    public override void OnTurnStarts(BaseTileData[] neighbours)
    {
        int upValue = 0;
        foreach(BaseTileData tile in neighbours)
        {
            if (tile is WaterTile)
            {
                upValue++;
            }
        }
        debugLevel += upValue;
    }

    public override string GetDebugText()
    { 
        return debugLevel.ToString();
    }

    public override void DebugOnClick()
    {
        debugLevel++;
        base.OnSpecificTileModified();
    }
}

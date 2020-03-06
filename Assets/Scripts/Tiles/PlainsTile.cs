using UnityEngine;
using UnityEngine.Tilemaps;

public class PlainsTile : BaseTileData
{
    private int debugLevel = 1;


    public override void OnTurnEnds(BaseTileData[] neighbours)
    {
        Debug.Log("end turn plain");
    }

    public override void OnTurnStarts(BaseTileData[] neighbours)
    {
        Debug.Log("start turn plain");
        int upValue = 0;
        foreach(BaseTileData tile in neighbours)
        {
            if (tile is WaterTile)
            {
                upValue++;
            }
        }
        debugLevel += upValue;
        Debug.Log("New debug level: " + debugLevel);
    }

    public override string GetDebugText()
    { 
        return debugLevel.ToString();
    }
}

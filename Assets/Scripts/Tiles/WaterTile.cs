using UnityEngine;
using UnityEngine.Tilemaps;

public class WaterTile : BaseTileData
{
    public override void OnTurnEnds(BaseTileData[] neighbours)
    {
        Debug.Log("end turn water");
    }

    public override void OnTurnStarts(BaseTileData[] neighbours)
    {
        Debug.Log("start turn water");
    }

    public override string GetDebugText()
    {
        return "";
    }
}

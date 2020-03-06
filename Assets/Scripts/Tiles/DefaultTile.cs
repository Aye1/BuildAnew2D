using UnityEngine;
using UnityEngine.Tilemaps;

public class DefaultTile : BaseTileData
{
    public override void OnTurnEnds(BaseTileData[] neighbours)
    {

    }

    public override void OnTurnStarts(BaseTileData[] neighbours)
    {

    }

    public override string GetDebugText()
    {
        return "?";
    }

    public override void DebugOnClick()
    {
    }
}

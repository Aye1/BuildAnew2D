using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class BaseTileData
{
    public Vector3Int gridPosition;
    public TileBase originTile;

    public abstract void OnTurnStarts(BaseTileData[] neighbours);

    public abstract void OnTurnEnds(BaseTileData[] neighbours);

    public abstract string GetDebugText();
}

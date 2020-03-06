using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public abstract class BaseTileData
{
    public Vector3Int gridPosition;
    public Vector3 worldPosition;
    public TileBase originTile;

    #region Events

    public delegate void TileModified();
    public static event TileModified OnTileModified;

    protected virtual void OnSpecificTileModified()
    {
        OnTileModified?.Invoke();
    }

    #endregion

    public abstract void OnTurnStarts(BaseTileData[] neighbours);

    public abstract void OnTurnEnds(BaseTileData[] neighbours);

    public abstract string GetDebugText();

    public abstract void DebugOnClick();
}

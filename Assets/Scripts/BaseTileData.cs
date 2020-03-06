using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public enum TileType { Plains, Water };

public abstract class BaseTileData
{
    public Vector3Int gridPosition;
    public Vector3 worldPosition;
    public TileBase originTile;
    public TileType type;

    #region Events

    public delegate void TileModified();
    public static event TileModified OnTileModified;

    protected virtual void OnSpecificTileModified()
    {
        OnTileModified?.Invoke();
    }

    #endregion

    public virtual void Init() { }

    public virtual void OnTurnStarts(BaseTileData[] neighbours) { }

    public virtual void OnTurnEnds(BaseTileData[] neighbours) { }

    public virtual string GetDebugText() { return ""; }

    public virtual void DebugOnClick() { }
}

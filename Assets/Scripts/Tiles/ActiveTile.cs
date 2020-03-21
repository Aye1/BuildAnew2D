using System.Collections.Generic;

public abstract class ActiveTile
{
    protected ActiveTile() => Init();

    public virtual void Init() 
    {
        OnTileModified?.Invoke(); 
    }

    public virtual void OnTurnStarts(IEnumerable<BaseTileData> neighbours) { }

    public virtual string GetDebugText() { return ""; }
    public virtual string GetText() { return ""; }

    public virtual void DebugOnClick() { }


    #region Events

    public delegate void TileModified();
    public static event TileModified OnTileModified;

    protected virtual void OnSpecificTileModified()
    {
        OnTileModified?.Invoke();
    }

    #endregion
}

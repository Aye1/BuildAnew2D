
public abstract class ActiveTile
{
    protected ActiveTile() => Init();

    public virtual void Init() { }

    public virtual void OnTurnStarts(BaseTileData[] neighbours) { }

    public virtual string GetDebugText() { return ""; }

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

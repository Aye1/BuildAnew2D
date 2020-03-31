
public class ChangeTileCommand : Command
{
    private BaseTileData _oldTile;
    private BaseTileData _newTile;

    public ChangeTileCommand(BaseTileData oldTile, BaseTileData newTile)
    {
        _oldTile = oldTile;
        _newTile = newTile;
    }

    public override void Execute()
    {
        TilesDataManager.Instance.SwapTiles(_oldTile, _newTile);
    }

    public override void Undo()
    {
        TilesDataManager.Instance.SwapTiles(_newTile, _oldTile);
    }

    public override string GetDescription()
    {
        return "Changing tile at position " + _oldTile.gridPosition.ToString();
    }
}

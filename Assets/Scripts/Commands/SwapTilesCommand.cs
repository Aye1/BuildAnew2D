
public class SwapTilesCommand : Command
{
    private BaseTileData _oldTile;
    private BaseTileData _newTile;

    public SwapTilesCommand(BaseTileData oldTile, BaseTileData newTile)
    {
        _oldTile = oldTile;
        _newTile = newTile;
    }

    public override void Execute()
    {
        TilesDataManager.Instance.SwapNextTurnTiles(_oldTile, _newTile);
    }

    public override void Undo()
    {
        TilesDataManager.Instance.SwapNextTurnTiles(_newTile, _oldTile);
    }

    public override string GetDescription()
    {
        return "Changing tile at position " + _oldTile.GridPosition.ToString();
    }
}

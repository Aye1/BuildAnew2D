
public class FloodTileCommand : Command
{
    private WaterCluster _cluster;
    private BaseTileData _originTile;
    private BaseTileData _floodedTile;
    private StructureTile _floodedStructure;
    private SwapTilesCommand _swapTilesCommand;
    private WaterClusterRemoveFloodCommand _removeFloodCommand;

    public FloodTileCommand(WaterCluster cluster, BaseTileData tile)
    {
        _cluster = cluster;
        _originTile = tile;
    }

    public override void Execute()
    {
        _floodedTile = new BaseTileData(_originTile);
        TilesDataManager.Instance.CreateTerrainFromType(TerrainType.Water, _floodedTile);
        TilesDataManager.Instance.ChangeTerrainTileInTilemap(_floodedTile.GetGridPosition(), TerrainType.Water, true);
        _cluster.AddTile(_floodedTile);
        _floodedStructure = _floodedTile.HandleFlood();
        _originTile.HandleFloodPrevision();
        _swapTilesCommand = new SwapTilesCommand(_originTile, _floodedTile);
        _swapTilesCommand.Execute();
        _removeFloodCommand = new WaterClusterRemoveFloodCommand(_cluster, WaterClusterManager.floodThreshold);
        _removeFloodCommand.Execute();
    }

    public override string GetDescription()
    {
        return "Flooding cluster " + _cluster.id;
    }

    public override void Undo()
    {
        _removeFloodCommand.Undo();
        _swapTilesCommand.Undo();
        _originTile.RemoveFloodPrevision();
        _floodedTile.structureTile = _floodedStructure;
        _cluster.RemoveTile(_floodedTile);
    }
}

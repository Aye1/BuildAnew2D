
public class FloodTileCommand : Command
{
    private WaterCluster _cluster;
    private BaseTileData _originTile;
    private BaseTileData _floodedTile;
    private StructureTile _floodedStructure;
    private SwapTilesCommand _swapTilesCommand;
    private WaterClusterRemoveFloodCommand _removeFloodCommand;
    private bool _saveFloodedTile;

    public FloodTileCommand(WaterCluster cluster, BaseTileData tile, bool saveFloodedTile)
    {
        _cluster = cluster;
        _originTile = tile;
        _saveFloodedTile = saveFloodedTile;
    }

    public override void Execute()
    {
        if (_saveFloodedTile)
        {
            // WARNING: this part is not reversible at the moment
            WaterClusterManager.Instance.AddPossibleFloodTile(_cluster, _originTile);
        }

        _floodedTile = new BaseTileData(_originTile);
        _floodedTile.terrainTile = TilesDataManager.Instance.CreateTerrainFromType(TerrainType.Water);
        TilesDataManager.Instance.ChangeTerrainTileInTilemap(_floodedTile.gridPosition, TerrainType.Water, true);
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

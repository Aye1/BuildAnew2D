using System.Collections.Generic;

public class CreateClustersCommand : Command
{
    private List<WaterCluster> _oldClusters;

    public override void Execute()
    {
        _oldClusters = WaterClusterManager.Instance.clusters;
        IEnumerable<BaseTileData> waterTiles = TilesDataManager.Instance.GetTilesWithTerrainType(TerrainType.Water, true);
        WaterClusterManager.Instance.CreateClusters(waterTiles);
    }

    public override string GetDescription()
    {
        return "Creating water clusters";
    }

    public override void Undo()
    {
        WaterClusterManager.Instance.clusters = _oldClusters;
    }
}

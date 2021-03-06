﻿using System.Collections.Generic;

public class CreateClustersCommand : Command
{
    private List<WaterCluster> _oldClusters;
    private BalanceAllWaterClustersCommand _balanceClustersCommand;

    public override void Execute()
    {
        _oldClusters = new List<WaterCluster>();
        List<WaterCluster> currentClusters = WaterClusterManager.Instance.clusters;
        if(currentClusters != null)
        {
            _oldClusters.AddRange(currentClusters);
        }
        IEnumerable<BaseTileData> waterTiles = TilesDataManager.Instance.GetTilesWithTerrainType(TerrainType.Water, true);
        WaterClusterManager.Instance.CreateClusters(waterTiles);
        _balanceClustersCommand = new BalanceAllWaterClustersCommand();
        _balanceClustersCommand.Execute();
    }

    public override string GetDescription()
    {
        return "Creating water clusters";
    }

    public override void Undo()
    {
        _balanceClustersCommand.Undo();
        WaterClusterManager.Instance.clusters = _oldClusters;
    }
}

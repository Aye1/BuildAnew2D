using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FloodMapCommand : Command
{
    private Stack<FloodClusterCommand> _floodClusterCommands;
    private CreateClustersCommand _recreateClustersCommand;

    public override void Execute()
    {
        _floodClusterCommands = new Stack<FloodClusterCommand>();
        IEnumerable<BaseTileData> waterTiles = TilesDataManager.Instance.GetTilesWithTerrainType(TerrainType.Water);
        waterTiles.ToList().ForEach(x => ((WaterTile)x.terrainTile).IncrementFlood());
        List<WaterCluster> clustersToFlood = new List<WaterCluster>();
        // Flooding may change the cluster configuration
        // Thus, we don't do it in the first foreach
        // There's a bit of logic which was clear at first for me, but isn't anymore
        foreach(WaterCluster cluster in WaterClusterManager.Instance.clusters)
        {
            cluster.RecountFloodLevel();
            if(cluster.FloodLevel >= WaterClusterManager.floodThreshold)
            {
                clustersToFlood.Add(cluster);
            }
        }
        foreach(WaterCluster cluster in clustersToFlood)
        {
            FloodClusterCommand floodClusterCommand = new FloodClusterCommand(cluster);
            floodClusterCommand.Execute();
            _floodClusterCommands.Push(floodClusterCommand);
        }
        _recreateClustersCommand = new CreateClustersCommand();
        _recreateClustersCommand.Execute();
    }

    public override string GetDescription()
    {
        return "Flooding map";
    }

    public override void Undo()
    {
        _recreateClustersCommand.Undo();
        while(_floodClusterCommands.Count > 0)
        {
            _floodClusterCommands.Pop().Undo();
        }
        IEnumerable<BaseTileData> waterTiles = TilesDataManager.Instance.GetTilesWithTerrainType(TerrainType.Water);
        waterTiles.ToList().ForEach(x => ((WaterTile)x.terrainTile).DecrementFlood());
    }
}

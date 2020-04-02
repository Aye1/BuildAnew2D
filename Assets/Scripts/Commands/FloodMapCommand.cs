using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FloodMapCommand : Command
{
    private Stack<FloodClusterCommand> _floodClusterCommands;
    private CreateClustersCommand _recreateClustersCommand;
    private bool _ignoreStructures;

    public FloodMapCommand(bool ignoreStructures)
    {
        _ignoreStructures = ignoreStructures;
    }

    public override void Execute()
    {
        // Flood individual tiles
        IEnumerable<BaseTileData> waterTiles = TilesDataManager.Instance.GetTilesWithTerrainType(TerrainType.Water);
        waterTiles.ToList().ForEach(x => ((WaterTile)x.terrainTile).IncrementFlood());

        if (_ignoreStructures)
        {
            // WARNING: this is not reversible at the moment
            // Ignoring structures => creating possible flooded tiles
            WaterClusterManager.Instance.ClearPossibleFloodTiles();
        }
        else
        {
            // We put the structures in the flood computing
        }

        _floodClusterCommands = new Stack<FloodClusterCommand>();
        List<WaterCluster> clustersToFlood = new List<WaterCluster>();
        // Flooding may change the clusters list
        // Thus, we can't do it in this foreach, because we use its enumerator
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
            FloodClusterCommand floodClusterCommand = new FloodClusterCommand(cluster, _ignoreStructures);
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

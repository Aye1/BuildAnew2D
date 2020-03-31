using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaterClusterBalanceCommand : Command
{
    private WaterCluster _cluster;
    private Dictionary<BaseTileData, int> _oldFloodLevels;

    public WaterClusterBalanceCommand(WaterCluster cluster)
    {
        _cluster = cluster;
    }

    public override void Execute()
    {
        _oldFloodLevels = _cluster.BalanceFlood();
    }

    public override void Undo()
    {
        foreach(BaseTileData tile in _oldFloodLevels.Keys)
        {
            if (_oldFloodLevels.TryGetValue(tile, out int value))
            {
                ((WaterTile)tile.terrainTile).NTFloodLevel = value;
            }
        }
    }

    public override string GetDescription()
    {
        return "Balancing cluster " + _cluster.id;
    }
}

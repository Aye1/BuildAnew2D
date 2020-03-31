using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaterClusterRemoveFloodCommand : Command
{
    private WaterCluster _cluster;
    private int _amount;
    private Dictionary<BaseTileData, int> _oldFloodLevels;
    private WaterClusterBalanceCommand _balanceCommand;

    public WaterClusterRemoveFloodCommand(WaterCluster cluster, int amount)
    {
        _cluster = cluster;
        _amount = amount;
        _balanceCommand = new WaterClusterBalanceCommand(cluster);
    }

    public override void Execute()
    {
        _cluster.RemoveFlood(_amount);
        _balanceCommand.Execute();
    }

    public override string GetDescription()
    {
        return "Removing " + _amount + " flood from cluster " + _cluster.id;
    }

    public override void Undo()
    {
        _balanceCommand.Undo();
        _cluster.RecountFloodLevel();
    }
}

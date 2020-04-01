using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BalanceAllWaterClustersCommand : Command
{
    Stack<WaterClusterBalanceCommand> _balanceCommands;

    public override void Execute()
    {
        _balanceCommands = new Stack<WaterClusterBalanceCommand>(); 
        foreach(WaterCluster cluster in WaterClusterManager.Instance.clusters)
        {
            WaterClusterBalanceCommand balanceCommand = new WaterClusterBalanceCommand(cluster);
            balanceCommand.Execute();
            _balanceCommands.Push(balanceCommand);
        }
    }

    public override string GetDescription()
    {
        return "Balancing all water clusters";
    }

    public override void Undo()
    {
        while(_balanceCommands.Count > 0)
        {
            _balanceCommands.Pop().Undo();
        }
    }
}

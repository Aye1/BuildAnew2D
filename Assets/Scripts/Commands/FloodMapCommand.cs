using System.Collections.Generic;

public class FloodMapCommand : Command
{
    private Stack<FloodClusterCommand> _floodClusterCommands;
    private CreateClustersCommand _recreateClustersCommand;
    private Stack<PumpWaterClusterCommand> _pumpWaterCommands;
    private FloodWaterTilesCommand _floodWaterTilesCommand;

    public override void Execute()
    {
        _floodWaterTilesCommand = new FloodWaterTilesCommand();
        _floodWaterTilesCommand.Execute();

        _floodClusterCommands = new Stack<FloodClusterCommand>();
        _pumpWaterCommands = new Stack<PumpWaterClusterCommand>();
        List<WaterCluster> clustersToFlood = new List<WaterCluster>();

        // Flooding may change the clusters list
        // Thus, we can't do it in this foreach, because we use its enumerator
        foreach(WaterCluster cluster in WaterClusterManager.Instance.clusters)
        {
            cluster.RecountFloodLevel();

            PumpWaterClusterCommand pumpCommand = new PumpWaterClusterCommand(cluster);
            pumpCommand.Execute();
            _pumpWaterCommands.Push(pumpCommand);

            if (cluster.FloodLevel >= WaterClusterManager.floodThreshold)
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
        return "(Re-)computing flooding";
    }

    public override void Undo()
    {
        _recreateClustersCommand.Undo();
        while(_floodClusterCommands.Count > 0)
        {
            _floodClusterCommands.Pop().Undo();
        }
        while(_pumpWaterCommands.Count > 0)
        {
            _pumpWaterCommands.Pop().Undo();
        }
        _floodWaterTilesCommand.Undo();
    }
}

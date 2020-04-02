
public class PrepareFloodedTilesCommand : Command
{
    public override void Execute()
    {
        WaterClusterManager.Instance.ClearPossibleFloodTiles();
        FloodWaterTilesCommand temporaryFloodCommand = new FloodWaterTilesCommand();
        temporaryFloodCommand.Execute();
        foreach (WaterCluster cluster in WaterClusterManager.Instance.clusters)
        {
            cluster.RecountFloodLevel();
            if (cluster.FloodLevel >= WaterClusterManager.floodThreshold)
            {
                int neighboursToFlood = cluster.FloodLevel / WaterClusterManager.floodThreshold;
                for (int i = 0; i < neighboursToFlood; i++)
                {
                    WaterClusterManager.Instance.RegisterRandomFloodableTile(cluster);
                }
            }
        }
        temporaryFloodCommand.Undo();
    }

    public override string GetDescription()
    {
        return "Preparing the possible flood tiles";
    }

    public override void Undo()
    {
        // We don't exactly go back to the previous state, as we don't reconstruct the possible flood tiles list
        // But this shouldn't be a problem, as this command is only used at the beginning of the turn
        WaterClusterManager.Instance.ClearPossibleFloodTiles();
    }
}

using System.Collections.Generic;

public class FloodClusterCommand : Command
{
    private WaterCluster _cluster;
    private Stack<FloodTileCommand> _floodCommands;
    private Stack<BaseTileData> _floodedTiles;

    public FloodClusterCommand(WaterCluster cluster)
    {
        _cluster = cluster;
    }

    public override void Execute()
    {
        _floodCommands = new Stack<FloodTileCommand>();
        _floodedTiles = new Stack<BaseTileData>();
        int neighboursToFlood = _cluster.FloodLevel / WaterClusterManager.floodThreshold;
        for (int i = 0; i < neighboursToFlood; i++)
        {
            BaseTileData tileToFlood = WaterClusterManager.Instance.UsePossibleFloodTile(_cluster);
            if (tileToFlood == null)
                return;

            _floodedTiles.Push(tileToFlood);

            FloodTileCommand floodTileCommand = new FloodTileCommand(_cluster, tileToFlood);
            floodTileCommand.Execute();
            _floodCommands.Push(floodTileCommand);
        }
    }

    public override string GetDescription()
    {
        return "Flooding cluster " + _cluster.id;
    }

    public override void Undo()
    {
        while(_floodCommands.Count > 0)
        {
            _floodCommands.Pop().Undo();
        }
        while(_floodedTiles.Count > 0)
        {
            WaterClusterManager.Instance.AddPossibleFloodTile(_cluster, _floodedTiles.Pop());
        }
    }
}

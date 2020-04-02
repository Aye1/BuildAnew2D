using System.Collections.Generic;

public class FloodClusterCommand : Command
{
    private WaterCluster _cluster;
    private Stack<FloodTileCommand> _floodCommands;
    private bool _pickRandomTile;

    public FloodClusterCommand(WaterCluster cluster)
    {
        _cluster = cluster;
        _pickRandomTile = false;
    }

    public FloodClusterCommand(WaterCluster cluster, bool pickRandomTile)
    {
        _cluster = cluster;
        _pickRandomTile = pickRandomTile;
    }

    public override void Execute()
    {
        _floodCommands = new Stack<FloodTileCommand>();
        int neighboursToFlood = _cluster.FloodLevel / WaterClusterManager.floodThreshold;
        for (int i = 0; i < neighboursToFlood; i++)
        {
            BaseTileData tileToFlood = null;
            // Maybe do the choice on WaterClusterManager side
            if (_pickRandomTile)
            {
                tileToFlood = WaterClusterManager.Instance.GetRandomFloodableTile(_cluster);
            }
            else
            {
                tileToFlood = WaterClusterManager.Instance.UsePossibleFloodTile(_cluster);
            }
            if (tileToFlood == null)
                return;

            FloodTileCommand floodTileCommand = new FloodTileCommand(_cluster, tileToFlood, _pickRandomTile);
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
    }
}

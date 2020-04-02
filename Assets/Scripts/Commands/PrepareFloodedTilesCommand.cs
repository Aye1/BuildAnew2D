using UnityEngine;
using System.Collections;

public class PrepareFloodedTilesCommand : Command
{
    FloodMapCommand _floodMapCommand;

    public override void Execute()
    {
        // Create the flooded tilemap
        _floodMapCommand = new FloodMapCommand(true);
        _floodMapCommand.Execute();

        // Undo the tilemap creation, but the WaterClusterManager has kept the flooded tiles in memory
        _floodMapCommand.Undo();
    }

    public override string GetDescription()
    {
        return "Preparing the possible flood tiles";
    }

    public override void Undo()
    {
        WaterClusterManager.Instance.ClearPossibleFloodTiles();
    }
}

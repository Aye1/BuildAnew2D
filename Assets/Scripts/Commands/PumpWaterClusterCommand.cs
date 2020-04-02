using System.Collections.Generic;

public class PumpWaterClusterCommand : Command
{
    private WaterCluster _cluster;
    private int _oldAmount;
    private WaterClusterBalanceCommand _balanceCommand;

    public PumpWaterClusterCommand(WaterCluster cluster)
    {
        _cluster = cluster;
    }

    public override void Execute()
    {
        List<BaseTileData> pumpedTiles = WaterClusterManager.Instance.GetPumpedTilesForCluster(_cluster);
        _oldAmount = _cluster.FloodLevel;


        int pumpedAmount = pumpedTiles.Count * PumpingStationTile.pumpingAmount;
        _cluster.RemoveFlood(pumpedAmount);
        _balanceCommand = new WaterClusterBalanceCommand(_cluster);
        _balanceCommand.Execute();
    }

    public override string GetDescription()
    {
        return "Pumping water";
    }

    public override void Undo()
    {
        _balanceCommand.Undo();
        _cluster.FloodLevel = _oldAmount;
    }
}

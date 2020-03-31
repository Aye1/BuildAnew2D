using System.Collections.Generic;

public class PumpingStationTile : StructureTile
{
    public int pumpingAmount = 5 ;

    public override StructureType GetStructureType()
    {
        return StructureType.PumpingStation;
    }

    public override void OnTurnStarts(IEnumerable<BaseTileData> neighbours)
    {
        base.OnTurnStarts(neighbours);
        //List<WaterCluster> modifiedClusters = new List<WaterCluster>();
        if (IsOn)
        {
            foreach (BaseTileData tile in neighbours)
            {
                if (tile.terrainTile is WaterTile)
                {
                    // PUMP
                    WaterCluster currentCluster = WaterClusterManager.Instance.GetClusterForTile(tile);
                    WaterClusterRemoveFloodCommand removeFloodCommand = new WaterClusterRemoveFloodCommand(currentCluster, pumpingAmount);
                    CommandManager.Instance.ExecuteCommand(removeFloodCommand);
                }
            }
        }
    }
}

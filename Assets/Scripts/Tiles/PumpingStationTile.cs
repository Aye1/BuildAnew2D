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
        if (IsOn)
        {
            foreach (BaseTileData tile in neighbours)
            {
                if (tile.terrainTile is WaterTile)
                {
                    // PUMP
                    WaterClusterManager.Instance.GetClusterForTile(tile).RemoveFlood(pumpingAmount);
                }
            }
        }
    }
}

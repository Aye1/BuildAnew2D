using System.Collections.Generic;

public class PumpingStationTile : StructureTile
{
    public static int pumpingAmount = 5 ;

    public override StructureType GetStructureType()
    {
        return StructureType.PumpingStation;
    }

    public override void OnTurnStarts(IEnumerable<BaseTileData> neighbours)
    {
    }
    public override void InternalToggleStructureIfPossible()
    {
        WaterClusterManager.Instance.RecomputeFlooding();
    }

}

using System.Collections.Generic;

public class PumpingStationTile : StructureTile
{
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
                    ((WaterTile)tile.terrainTile).Drain();
                }
            }
        }
    }
}

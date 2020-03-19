

public class PumpingStationTile : StructureTile
{
    public override void Init()
    {
        structureType = StructureType.PumpingStation;
        base.Init();
    }

    public override void OnTurnStarts(BaseTileData[] neighbours)
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

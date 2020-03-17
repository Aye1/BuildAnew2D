

public class PumpingStationTile : StructureTile
{
    public override void Init()
    {
        base.Init();
        structureType = StructureType.PumpingStation;
        consumesEnergy = true;
        producesEnergy = false;
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

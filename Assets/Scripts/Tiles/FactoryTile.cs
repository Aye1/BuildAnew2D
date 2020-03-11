

public class FactoryTile : StructureTile
{
    public Factory factory;
    public override void Init()
    {
        base.Init();
        structureType = StructureType.Factory;
    }

    public override void OnTurnStarts(BaseTileData[] neighbours)
    {
        base.OnTurnEnds(neighbours);
        if(factory != null && factory.IsON)
        {
            foreach(BaseTileData tile in neighbours)
            {
                if(tile.terrainTile is WaterTile)
                {
                    // PUMP
                    ((WaterTile)tile.terrainTile).Drain();
                }
            }
        }
    }
}


// Data for the PowerPlant MonoBehaviour
public class PowerPlantTile : StructureTile
{
    public override void Init()
    {
        base.Init();
        structureType = StructureType.PowerPlant;
    }

    public override void OnTurnStarts(BaseTileData[] neighbours)
    {
        base.OnTurnStarts(neighbours);
        if(IsOn)
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

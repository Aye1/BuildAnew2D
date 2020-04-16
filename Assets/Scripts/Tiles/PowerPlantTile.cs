
// Data for the PowerPlant MonoBehaviour
public class PowerPlantTile : StructureTile
{
    public override StructureType GetStructureType()
    {
        return StructureType.PowerPlant;
    }

    public override void DestroyStructure()
    {
        BaseTileData baseTileData = TilesDataManager.Instance.GetTileDataAtPos(GridPosition);
        WaterClusterManager.Instance.UnregisterPumpingStation(baseTileData);
        base.DestroyStructure();
    }
}

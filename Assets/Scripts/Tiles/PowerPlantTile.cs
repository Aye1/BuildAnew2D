
// Data for the PowerPlant MonoBehaviour
public class PowerPlantTile : StructureTile
{
    public override StructureType GetStructureType()
    {
        return StructureType.PowerPlant;
    }
    public override void Init()
    {
        base.Init();
        IsOn = true;
    }
}

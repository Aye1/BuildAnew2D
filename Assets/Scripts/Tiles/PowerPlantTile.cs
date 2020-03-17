
// Data for the PowerPlant MonoBehaviour
public class PowerPlantTile : StructureTile
{
    public override void Init()
    {
        base.Init();
        structureType = StructureType.PowerPlant;
        consumesEnergy = false;
        producesEnergy = true;
    }
}

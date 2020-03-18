
public enum StructureType { None, PowerPlant, Sawmill, PumpingStation };

public class StructureTile : ActiveTile
{
    public StructureType structureType;
    public bool IsOn;
    public Building building;
    public bool consumesEnergy;
    public bool producesEnergy;
}

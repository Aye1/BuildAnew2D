
public enum StructureType { None, PowerPlant, Sawmill };

public class StructureTile : ActiveTile
{
    public StructureType structureType;
    public bool IsOn;

}

using System.Collections.Generic;

public class VillageTile : StructureTile
{
    public override StructureType GetStructureType()
    {
        return StructureType.Village;
    }

    public override void OnTurnStarts(IEnumerable<BaseTileData> neighbours)
    {
        base.OnTurnStarts(neighbours);      
    }
}

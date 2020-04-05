using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelayTile : StructureTile
{
    public override StructureType GetStructureType()
    {
        return StructureType.Relay;
    }
}

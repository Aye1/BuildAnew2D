using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainRelayTile : StructureTile
{
    public override void Init()
    {
        base.Init();
        IsOn = true;
    }
    public override StructureType GetStructureType()
    {
        return StructureType.MainRelay;
    }
}

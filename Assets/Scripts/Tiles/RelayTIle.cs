using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelayTile : StructureTile
{
    public override void Init()
    {
        base.Init();
        IsOn = true;
    }
    public override StructureType GetStructureType()
    {
        return StructureType.Relay;
    }
    public override void InternalToggleStructureIfPossible()
    {
        RelayManager.Instance.ComputeInRangeRelays();
    }
}

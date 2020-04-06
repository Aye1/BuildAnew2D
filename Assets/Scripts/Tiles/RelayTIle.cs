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

    public override void InternalUpgradeStructure()
    {
        RelayManager.Instance.ComputeInRangeRelays();
    }

    public virtual int GetActivationAreaRange()
    {
        RelayUpgradeBinding upgradeBinding = (RelayUpgradeBinding)(structureData.upgradeData.GetUpgradeBindingForLevel(structureLevel));
        return upgradeBinding._range;
    }
}

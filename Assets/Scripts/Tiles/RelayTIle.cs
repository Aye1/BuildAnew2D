using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelayTile : StructureTile
{
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
        RelayUpgradeBinding upgradeBinding = (RelayUpgradeBinding)(structureData.upgradeData.GetUpgradeBindingForLevel(GetStructureLevel()));
        return upgradeBinding._range;
    }
    
    public override void FillAreaOfEffectNeighbours() 
    {
        BaseTileData baseTileData = TilesDataManager.Instance.GetTileDataAtPos(GridPosition);
        _areaOfEffect = GridUtils.GetNeighboursTilesOfRelay(baseTileData);
        
    }
}
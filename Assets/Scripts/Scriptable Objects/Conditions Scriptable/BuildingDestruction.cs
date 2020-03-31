using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Condition/BuildingDestructionConditionData", order = 1)]
public class BuildingDestruction : BuildingCondition
{
    public override bool IsStructureVerifyingCondition(BaseTileData tile)
    {
        return tile.structureTile == null;
    }
    protected override string GetBuildingSpecificCondition()
    {
        return " Destroy ";
    }
}

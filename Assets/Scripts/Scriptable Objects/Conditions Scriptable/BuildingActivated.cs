using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Condition/BuildingActivatedConditionData", order = 1)]
public class BuildingActivatedCondition : BuildingCondition
{
    public override bool IsStructureVerifyingCondition(BaseTileData tile)
    {
        return tile.structureTile.IsOn;
    }
}
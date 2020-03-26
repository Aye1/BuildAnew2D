using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum StructureNumber { None, AtLeastOne, All };

public abstract class BuildingCondition : BaseCondition
{
    [SerializeField] private StructureNumber structureNumber;
    [SerializeField] private StructureType structureType;
    public abstract bool IsStructureVerifyingCondition(BaseTileData tile);
    public override bool IsConditionVerified()
    {
        bool isConditionVerified = false;
        IEnumerable<BaseTileData> allTypedStructure = TilesDataManager.Instance.GetTilesWithStrucureType(structureType);
        switch (structureNumber)
        {
            case StructureNumber.None:
                isConditionVerified = !allTypedStructure.Any(x => IsStructureVerifyingCondition(x));
                break;
            case StructureNumber.AtLeastOne:
                isConditionVerified = allTypedStructure.Any(x => IsStructureVerifyingCondition(x));
                break;
            case StructureNumber.All:
                isConditionVerified = allTypedStructure.All(x => IsStructureVerifyingCondition(x));
                break;
        }
        return isConditionVerified;
    }
}
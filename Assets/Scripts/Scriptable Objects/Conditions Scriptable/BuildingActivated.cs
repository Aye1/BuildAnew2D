using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum StructureNumber { None, AtLeastOne, All};

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/BuildingActivatedConditionData", order = 1)]
public class BuildingActivatedCondition : BaseCondition
{
    [SerializeField] private StructureNumber structureNumber;
    [SerializeField] private StructureType structureType;

    public override bool IsConditionVerified()
    {
        bool isConditionVerified = false;
        IEnumerable<BaseTileData> allTypedStructure = TilesDataManager.Instance.GetTilesWithStrucureType(structureType);
        switch (structureNumber)
        {
            case StructureNumber.None:
                isConditionVerified = !allTypedStructure.Any(x => x.structureTile.IsOn);
                break;
            case StructureNumber.AtLeastOne:
                isConditionVerified = allTypedStructure.Any(x => x.structureTile.IsOn);
                break;
            case StructureNumber.All:
                isConditionVerified = allTypedStructure.All(x => x.structureTile.IsOn);
                break;
        }
        return isConditionVerified;
    }
}
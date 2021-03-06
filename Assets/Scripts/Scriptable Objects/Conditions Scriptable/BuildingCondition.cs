﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum StructureNumber { None, AtLeastOne, All };

public abstract class BuildingCondition : BaseCondition
{
    #region Editor objects
#pragma warning disable 0649
    [SerializeField] private StructureNumber structureNumber;
    [SerializeField] private StructureType structureType;
#pragma warning restore 0649
#endregion
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

    protected abstract string GetBuildingSpecificCondition();

    public string GetNumberString()
    {
        string returnText = "";
        switch (structureNumber)
        {
            case StructureNumber.None:
                returnText = " No ";
                break;
            case StructureNumber.AtLeastOne:
                returnText = " At Least One ";
                
                break;
            case StructureNumber.All:
                returnText = " All ";
                break;
        }
        return returnText;
    }
    public override string GetText()
    {
        StructureBinding binding = BuildingManager.Instance.StructuresTemplates.GetStructureBindingFromType(structureType);
        return GetBuildingSpecificCondition() + GetNumberString() + binding.data.structureName ;
    }
}
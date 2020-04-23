using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Condition/BuildingConstructionConditionData", order = 1)]
public class BuildingCreation : BaseCondition
{
    #region Editor objects
#pragma warning disable 0649
    [SerializeField] private StructureType structureType;
#pragma warning restore 0649
    #endregion
    public override string GetText()
    {
        StructureBinding binding = BuildingManager.Instance.StructuresTemplates.GetStructureBindingFromType(structureType);
        return "Create " + binding.data.structureName;
    }

    public override bool IsConditionVerified()
    {
        IEnumerable<BaseTileData> allTypedStructure = TilesDataManager.Instance.GetTilesWithStrucureType(structureType);
        return allTypedStructure.Count<BaseTileData>() > 0;
    }
}

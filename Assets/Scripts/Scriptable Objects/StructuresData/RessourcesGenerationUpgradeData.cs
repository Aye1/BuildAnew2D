using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RessourcesGenerationUpgradeBinding : UpgradeStructureBinding
{
    public Cost _resourcesGeneration;
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/UpgradeStructureData/RessourcesGenerationUpgrade", order = 1)]
public class RessourcesGenerationUpgradeData : UpgradeStructureData
{
    #region Editor objects
#pragma warning disable 0649
    [SerializeField] private List<RessourcesGenerationUpgradeBinding> resourcesUpgrades;

#pragma warning restore 0649
    #endregion
    protected override IReadOnlyList<UpgradeStructureBinding> GetUpgradeStructureBindings()
    {
        IReadOnlyList<UpgradeStructureBinding> ro_list = resourcesUpgrades;
        return ro_list;
    }
}

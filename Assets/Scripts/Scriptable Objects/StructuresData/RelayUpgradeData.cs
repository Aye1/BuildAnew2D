using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class RelayUpgradeBinding : UpgradeStructureBinding
{
    public int _range;
}
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/UpgradeStructureData/RelayUpgradeStructureData", order = 1)]
public class RelayUpgradeData : UpgradeStructureData
{
    #region Editor objects
#pragma warning disable 0649
    [SerializeField] private List<RelayUpgradeBinding> relayUpgrades;

#pragma warning restore 0649
    #endregion
    protected override IReadOnlyList<UpgradeStructureBinding> GetUpgradeStructureBindings()
    {
        IReadOnlyList<UpgradeStructureBinding> ro_list = relayUpgrades;
        return ro_list;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[System.Serializable]
public class UpgradeStructureBinding
{
    public StructureLevel level;
    public List<Cost> upgradeCosts;
    public Cost resourceGenerated;
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/UpgradeStructureData", order = 1)]
public class UpgradeStructureData : ScriptableObject
{
    #region Editor objects
#pragma warning disable 0649
    [SerializeField] private List<UpgradeStructureBinding> upgrades;
#pragma warning restore 0649
    #endregion
    public UpgradeStructureBinding GetUpgradeBindingForLevel(StructureLevel level)
    {
        return upgrades.Find(x => x.level == level);
    }
    public StructureLevel GetMaxLevel()
    {
        return upgrades.Max(x => x.level);
    }
}

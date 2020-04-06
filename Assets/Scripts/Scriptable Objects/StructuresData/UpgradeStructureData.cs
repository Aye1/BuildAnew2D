using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[System.Serializable]
public class UpgradeStructureBinding
{
    public StructureLevel level;
    public List<Cost> upgradeCosts;
    public List<Cost> sellingGain;
}

public abstract class UpgradeStructureData : ScriptableObject
{
    protected abstract IReadOnlyList<UpgradeStructureBinding> GetUpgradeStructureBindings();
    public UpgradeStructureBinding GetUpgradeBindingForLevel(StructureLevel level)
    {
        return GetUpgradeStructureBindings().ToList().Find(x => x.level == level);
    }
    public StructureLevel GetMaxLevel()
    {
        return GetUpgradeStructureBindings().Max(x => x.level);
    }
}

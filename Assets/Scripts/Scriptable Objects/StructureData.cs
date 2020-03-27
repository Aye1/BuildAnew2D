using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/StructureData", order = 1)]
public class StructureData : ScriptableObject
{
    public int producedEnergyAmount;
    public int consumedEnergyAmount;
    public string structureName = "";
    public Sprite icon;
    public List<TerrainType> constructibleTerrainTypes;
    [Tooltip("If empty, can't be constructible")]
    public UpgradeStructureData upgradeData;
    public bool ProducesEnergy
    {
        get { return producedEnergyAmount > 0; }
    }

    public bool ConsumesEnergy
    {
        get { return consumedEnergyAmount > 0; }
    }

    public List<Cost> GetCreationCost()
    {
        return GetUpgradeCostForLevel(StructureLevel.Level0);
    }

    public List<Cost> GetUpgradeCostForLevel(StructureLevel level)
    {
        List<Cost> upgradeCost = null;
        if (upgradeData != null)
        {
            UpgradeStructureBinding binding = upgradeData.GetUpgradeBindingForLevel(level);
            if (binding != null)
            {
                upgradeCost = binding.upgradeCosts;
            }
        }
        return upgradeCost;
    }
}

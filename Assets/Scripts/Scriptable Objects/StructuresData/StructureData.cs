using UnityEngine;
using System.Collections.Generic;

public enum EnergyStrategy { None, ConsumesEnergy, ProducesEnergy }

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/StructureData", order = 1)]
public class StructureData : ScriptableObject
{
    public EnergyStrategy _energyStrategy = EnergyStrategy.None;
    public int _energyAmount;
    public string structureName = "";
    public string buildShortcutName;
    public Sprite icon;
    public List<TerrainType> constructibleTerrainTypes;
    [Tooltip("If empty, can't be constructible")]
    public UpgradeStructureData upgradeData;
    public bool ProducesEnergy
    {
        get { return _energyStrategy == EnergyStrategy.ProducesEnergy; }
    }

    public bool ConsumesEnergy
    {
        get { return _energyStrategy == EnergyStrategy.ConsumesEnergy; }
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
    public List<Cost> GetSellingRefundResourcesForLevel(StructureLevel level)
    {
        List<Cost> sellingRefund = null;
        if (upgradeData != null)
        {
            UpgradeStructureBinding binding = upgradeData.GetUpgradeBindingForLevel(level);
            if (binding != null)
            {
                sellingRefund = binding.sellingGain;
            }
        }
        return sellingRefund;
    }

    public int GetCurrentEnergyAmountForLevel(StructureLevel level)
    {
        int energyAmount = _energyAmount;
        if (upgradeData != null)
        {
            UpgradeStructureBinding binding = upgradeData.GetUpgradeBindingForLevel(level);
            if (binding != null)
            {
                energyAmount = binding.energyAmount;
            }
        }
        return energyAmount;
    }
}

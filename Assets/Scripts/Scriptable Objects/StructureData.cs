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
        List<Cost> creationCost = null;
        if(upgradeData != null)
        {
            UpgradeStructureBinding binding = upgradeData.GetUpgradeBindingForLevel(StructureLevel.Level0);
            if(binding != null)
            {
                creationCost = binding.upgradeCosts;
            }
        }
        return creationCost;
    }
}

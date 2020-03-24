using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/StructureData", order = 1)]
public class StructureData : ScriptableObject
{
    public int producedEnergyAmount;
    public int consumedEnergyAmount;
    public bool isConstructible = true;
    public string structureName = "";
    public Sprite icon;
    public List<Cost> costs;
    public List<TerrainType> constructibleTerrainTypes;

    public bool ProducesEnergy
    {
        get { return producedEnergyAmount > 0; }
    }

    public bool ConsumesEnergy
    {
        get { return consumedEnergyAmount > 0; }
    }
}

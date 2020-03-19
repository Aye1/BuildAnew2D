using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CreateStructureDataScriptableObject", order = 1)]
public class StructureData : ScriptableObject
{
    public int producedEnergyAmount;
    public int consumedEnergyAmount;
    public bool isConstructible = true;
    public string StructureName = "";

    public bool ProducesEnergy
    {
        get { return producedEnergyAmount > 0; }
    }

    public bool ConsumesEnergy
    {
        get { return consumedEnergyAmount > 0; }
    }
}

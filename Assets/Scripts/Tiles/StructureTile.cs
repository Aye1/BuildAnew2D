using UnityEngine;

public enum StructureType { None, PowerPlant, Sawmill, PumpingStation };
public enum ActivationState {ActivationPossible, ImpossibleNeedEnergy, ImpossibleMissEnergy, ImpossibleMissingStructure };
public class StructureTile : ActiveTile
{
    public StructureType structureType;
    public StructureData structureData;
    public bool IsOn;
    public Building building;


    public override void Init()
    {
        base.Init();
        structureData = TilesDataManager.Instance.GetDataForStructure(structureType);
        if(structureData == null)
        {
            Debug.LogError("Data not found for structure " + structureType.ToString() + "\n"
            + "Check TilesDataManager mapping");
        }
    }

    public override string GetText()
    {
        return structureType == StructureType.None ? "" : structureType.ToString();
    }

    private ActivationState CanToggleStructure()
    {
        ActivationState canToggleStructure = ActivationState.ActivationPossible;

        int energyAvailable = ResourcesManager.Instance.EnergyAvailable;
        if (structureData.ProducesEnergy & IsOn)
        {
            canToggleStructure = energyAvailable >= structureData.producedEnergyAmount ? ActivationState.ActivationPossible : ActivationState.ImpossibleNeedEnergy;
        }
        else if (structureData.ConsumesEnergy & !IsOn)
        {
            canToggleStructure = energyAvailable >= structureData.consumedEnergyAmount ? ActivationState.ActivationPossible : ActivationState.ImpossibleMissEnergy;

        }

        return canToggleStructure;
    }

    public ActivationState ToggleStructureIfPossible()
    {
        ActivationState activationState = CanToggleStructure();
        if (activationState == ActivationState.ActivationPossible)
        {
            IsOn = !IsOn;
        }
        return activationState;
    }
}

using UnityEngine;
using System.Collections.Generic;

public enum StructureType { None, PowerPlant, Sawmill, PumpingStation, Village, Mine };
public enum StructureLevel { Level0, Level1 };
public enum ActivationState {ActivationPossible, ImpossibleNeedEnergy, ImpossibleMissEnergy, ImpossibleMissingStructure };
public abstract class StructureTile : ActiveTile
{
    public StructureType structureType;
    public StructureData structureData;
    public bool IsOn;
    public StructureLevel structureLevel = StructureLevel.Level0;
    public Building building;
    public abstract StructureType GetStructureType();

    public override void Init()
    {
        structureType = GetStructureType();
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

    public bool DeactivateStructureIfPossible()
    {
        if (IsOn)
        {
            ToggleStructureIfPossible();
        }
        return IsOn;
    }

    public bool ActivateStructureIfPossible()
    {
        if(!IsOn)
        {
            ToggleStructureIfPossible();
        }
        return IsOn;
    }

    public bool CanStructureBeFlooded()
    {
        return structureData.constructibleTerrainTypes.Contains(TerrainType.Water);
    }

    public void WarnStructureDestruction()
    {
        building.WarnDestruction();
    }
    public void DisableWarnStructureDestruction()
    {
        building.DisableWarningDestruction();
    }

    public void DestroyStructure()
    {
        bool willNeedRecomputation = IsOn;
        ResourcesManager.Instance.UnregisterStructure(this);
        if (willNeedRecomputation)
        {
            ActivationState state = CanToggleStructure();
            if (state == ActivationState.ImpossibleNeedEnergy)
            {
                Debug.Log("Energy consumption");
                ResourcesManager.Instance.RecomputeActiveStructure();
            }
        }
        IsOn = false;
       
    }

    private StructureLevel GetNextLevel()
    {
        return StructureLevel.Level1; // TODO : check real next level
    }

    private List<Cost> GetUpgradeCostForNextLevel()
    {
        return structureData.GetUpgradeCostForLevel(GetNextLevel());
    }

    public bool CanUpgradeStructure()
    {
        return ResourcesManager.Instance.CanPay(GetUpgradeCostForNextLevel());
    }

    public void UpgradeStructure()
    {
        if(CanUpgradeStructure())
        {
            ResourcesManager.Instance.Pay(GetUpgradeCostForNextLevel());
            structureLevel = GetNextLevel();
        }
    }
}

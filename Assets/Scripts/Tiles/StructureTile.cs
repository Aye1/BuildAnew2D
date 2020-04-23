using UnityEngine;
using System.Collections.Generic;

public enum StructureType { None, PowerPlant, Sawmill, PumpingStation, Village, Mine, MainRelay, Relay };
public enum StructureLevel { Level0, Level1 };
public enum ActivationState { ActivationPossible, ImpossibleNeedEnergy, ImpossibleMissEnergy, ImpossibleMissingStructure, OutsideRange };
public abstract class StructureTile : ActiveTile
{
    #region variable Member
    private bool _isOn;
    public bool IsOn
    {
        get
        {
            return _isOn;
        }
        set
        {
            if (_isOn != value)
            {
                _isOn = value;
                OnSpecificPropertyChanged("IsOn");
            }
        }
    }

    private StructureLevel structureLevel = StructureLevel.Level0;
    public StructureLevel StructureLevel { get => structureLevel; set => structureLevel = value; }
    private StructureLevel maxLevel = StructureLevel.Level0;

    private bool isFloodable = true;
    public bool IsFloodable { get => isFloodable; set => isFloodable = value; }

    public List<AbstractModuleScriptable> activeModules = new List<AbstractModuleScriptable>();


    public StructureType structureType;
    public StructureData structureData;//TODO move static info in another class
    public BuildingView building;
    protected List<BaseTileData> _areaOfEffect;
    #endregion

    public abstract StructureType GetStructureType();
    public override void Init()
    {
        structureType = GetStructureType();
        base.Init();
        structureData = TilesDataManager.Instance.GetDataForStructure(structureType);
        IsFloodable = structureData.CanStructureBeFlooded();
        if (structureData == null)
        {
            Debug.LogError("Data not found for structure " + structureType.ToString() + "\n"
            + "Check TilesDataManager mapping");
        }
        if (structureData.upgradeData != null)
        {
            maxLevel = structureData.upgradeData.GetMaxLevel();
        }
        _areaOfEffect = new List<BaseTileData>();
        ActivateStructureIfPossible();
    }

    public override string GetText()
    {
        return structureType == StructureType.None ? "" : structureType.ToString();
    }

    public StructureLevel GetStructureLevel()
    {
        return StructureLevel;
    }
    public ActivationState CanToggleStructure()
    {
        ActivationState canToggleStructure = ActivationState.ActivationPossible;
        BaseTileData data = TilesDataManager.Instance.GetTileDataAtPos(GridPosition);
        if (RelayManager.Instance.IsInsideRelayRange(data))
        {
            int energyAvailable = ResourcesManager.Instance.EnergyAvailable;
            if (structureData.ProducesEnergy & IsOn)
            {
                canToggleStructure = energyAvailable >= structureData._energyAmount ? ActivationState.ActivationPossible : ActivationState.ImpossibleNeedEnergy;
            }
            else if (structureData.ConsumesEnergy & !IsOn)
            {
                canToggleStructure = energyAvailable >= structureData._energyAmount ? ActivationState.ActivationPossible : ActivationState.ImpossibleMissEnergy;
            }
        }
        else
        {
            canToggleStructure = ActivationState.OutsideRange;
        }
        return canToggleStructure;
    }
    public override void InternalSelection()
    {
        ShowAreaOfEffect();
    }

    public void ShowAreaOfEffect()
    {
        FillAreaOfEffectNeighbours();
        foreach (BaseTileData tileData in _areaOfEffect)
        {
            if (_isSelected)
            {
                tileData.terrainTile.terrainInfo.ShowInsideAreaColor();
            }
            else
            {
                tileData.terrainTile.terrainInfo.HideInsideAreaColor();
            }
        }
    }
    public virtual void InternalToggleStructureIfPossible() { }

    public ActivationState ToggleStructureIfPossible()
    {
        ActivationState activationState = CanToggleStructure();
        if (activationState == ActivationState.ActivationPossible)
        {
            IsOn = !IsOn;
            InternalToggleStructureIfPossible();
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
        if (!IsOn)
        {
            ToggleStructureIfPossible();
        }
        return IsOn;
    }

    public bool CanStructureBeFlooded()
    {
        return IsFloodable;
    }

    public void WarnStructureDestruction()
    {
        building.WarnDestruction();
    }

    public void DisableWarnStructureDestruction()
    {
        building.DisableWarningDestruction();
    }

    public virtual void DestroyStructure()
    {
        FillAreaOfEffectNeighbours();
        foreach (BaseTileData tileData in _areaOfEffect)
        {
            tileData.terrainTile.terrainInfo.HideInsideAreaColor();
        }
        ResourcesManager.Instance.UnregisterStructure(this);
        ForceDeactivation();
        building.DestroyBuilding();
    }

    public void ForceDeactivation()
    {
        bool needRecomputation = IsOn;
        ActivationState state = CanToggleStructure();
        IsOn = false;
        if (needRecomputation)
        {
            if (state == ActivationState.ImpossibleNeedEnergy)
            {
                ResourcesManager.Instance.RecomputeActiveStructure();
            }
            if (this is PumpingStationTile)
            {
                WaterClusterManager.Instance.RecomputeFlooding();
            }
        }
    }

    private StructureLevel GetNextLevel()
    {
        StructureLevel nextLevel = GetStructureLevel() + 1;
        nextLevel = nextLevel >= maxLevel ? maxLevel : nextLevel;
        return nextLevel;
    }

    private List<Cost> GetUpgradeCostForNextLevel()
    {
        return structureData.GetUpgradeCostForLevel(GetNextLevel());
    }


    public bool HasNextLevelUpgrade()
    {
        return GetStructureLevel() != maxLevel;
    }
    public bool CanUpgradeStructure()
    {
        return  HasNextLevelUpgrade() && ResourcesManager.Instance.CanPay(GetUpgradeCostForNextLevel());
    }

    public virtual void InternalUpgradeStructure() { }

    public void UpgradeStructure()
    {
        if (CanUpgradeStructure())
        {
            ResourcesManager.Instance.Pay(GetUpgradeCostForNextLevel());
            StructureLevel = GetNextLevel();
            OnSpecificPropertyChanged("StructureDynamicInfo");
            building.UpgradeBuilding();
            InternalUpgradeStructure();
        }
    }

    public bool HasSellingData()
    {
        List<Cost> sellingRefund = structureData.GetSellingRefundResourcesForLevel(GetStructureLevel());
        return sellingRefund != null && sellingRefund.Count != 0;
    }
    public bool CanSellStructure()
    {
        BaseTileData data = TilesDataManager.Instance.GetTileDataAtPos(GridPosition);
        return HasSellingData() && RelayManager.Instance.IsInsideRelayRange(data);
    }

    public void SellStructure(Vector3Int position)
    {
        if (CanSellStructure())
        {
            List<Cost> sellingRefund = structureData.GetSellingRefundResourcesForLevel(GetStructureLevel());
            ResourcesManager.Instance.Repay(sellingRefund);
            BuildingManager.Instance.RemoveStructureAtPos(position, false);
        }
    }

    //Module
    public void AddModule(AbstractModuleScriptable module)
    {
        activeModules.Add(module);
        module.ApplyModuleEffect(this);
    }
     public bool IsModuleActive(AbstractModuleScriptable moduleScriptable)
    {
        return activeModules.Contains(moduleScriptable);
    }

    //Abstract methods
    public virtual void FillAreaOfEffectNeighbours() { }
}

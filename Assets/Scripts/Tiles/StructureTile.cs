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
        get { return _isOn; }
        set
        {
            if (_isOn != value)
            {
                _isOn = value;
                OnSpecificPropertyChanged("IsOn");
            }
        }
    }

    private StructureLevel _structureLevel = StructureLevel.Level0;
    public StructureLevel StructureLevel
    {
        get => _structureLevel;
        set {
            if (_structureLevel != value)
            {
                _structureLevel = value;
                OnSpecificPropertyChanged("StructureLevel");
            }
        }
    }
    private StructureLevel maxLevel = StructureLevel.Level0;

    private bool _isFloodable = true;
    public bool IsFloodable
    {
        get => _isFloodable;
        set
        {
            if (_isFloodable != value)
            {
                _isFloodable = value;
                OnSpecificPropertyChanged("IsFloodable");
            }
        }
    }

    public List<AbstractModuleScriptable> activeModules = new List<AbstractModuleScriptable>();


    public StructureType _structureType;
    public StructureData _structureData;//TODO move static info in another class
    public BuildingView _building;
    protected List<BaseTileData> _areaOfEffect;
    #endregion

    //Abstract methods
    public abstract StructureType GetStructureType();
    public virtual void FillAreaOfEffectNeighbours() { }


    public override void Init()
    {
        _structureType = GetStructureType();
        base.Init();
        _structureData = TilesDataManager.Instance.GetDataForStructure(_structureType);
        IsFloodable = _structureData.CanStructureBeFlooded();
        if (_structureData == null)
        {
            Debug.LogError("Data not found for structure " + _structureType.ToString() + "\n"
            + "Check TilesDataManager mapping");
        }
        if (_structureData.upgradeData != null)
        {
            maxLevel = _structureData.upgradeData.GetMaxLevel();
        }
        _areaOfEffect = new List<BaseTileData>();
        ActivateStructureIfPossible();
    }

    public override string GetText()
    {
        return _structureType == StructureType.None ? "" : _structureType.ToString();
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
            if (_structureData.ProducesEnergy & IsOn)
            {
                canToggleStructure = energyAvailable >= _structureData._energyAmount ? ActivationState.ActivationPossible : ActivationState.ImpossibleNeedEnergy;
            }
            else if (_structureData.ConsumesEnergy & !IsOn)
            {
                canToggleStructure = energyAvailable >= _structureData._energyAmount ? ActivationState.ActivationPossible : ActivationState.ImpossibleMissEnergy;
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
        _building.WarnDestruction();
    }

    public void DisableWarnStructureDestruction()
    {
        _building.DisableWarningDestruction();
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
        _building.DestroyBuilding();
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
        return _structureData.GetUpgradeCostForLevel(GetNextLevel());
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
            _building.UpgradeBuilding();
            InternalUpgradeStructure();
        }
    }

    public bool HasSellingData()
    {
        List<Cost> sellingRefund = _structureData.GetSellingRefundResourcesForLevel(GetStructureLevel());
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
            List<Cost> sellingRefund = _structureData.GetSellingRefundResourcesForLevel(GetStructureLevel());
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

    
}

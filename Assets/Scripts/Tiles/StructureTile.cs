using UnityEngine;
using System.Collections.Generic;

public enum StructureType { None, PowerPlant, Sawmill, PumpingStation, Village, Mine, MainRelay, Relay };
public enum StructureLevel { Level0, Level1 };
public enum ActivationState {ActivationPossible, ImpossibleNeedEnergy, ImpossibleMissEnergy, ImpossibleMissingStructure, OutsideRange };
public abstract class StructureTile : ActiveTile
{
    private bool _isOn;
    public bool IsOn
    {
        get
        {
            return _isOn;
        }
        set
        {
            if(_isOn != value)
            {
                _isOn = value;
                OnSpecificPropertyChanged("IsOn");
            }
        }
    }
    
    public StructureType structureType;
    public StructureData structureData;
    public StructureLevel structureLevel = StructureLevel.Level0;
    public Building building;
    public abstract StructureType GetStructureType();
    private StructureLevel maxLevel = StructureLevel.Level0;
    protected List<BaseTileData> _areaOfEffect;
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
        if(structureData.upgradeData != null)
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
        StructureLevel nextLevel = structureLevel + 1;
        nextLevel = nextLevel >= maxLevel ? maxLevel : nextLevel;
        return nextLevel; 
    }

    private List<Cost> GetUpgradeCostForNextLevel()
    {
        return structureData.GetUpgradeCostForLevel(GetNextLevel());
    }

    public bool CanUpgradeStructure()
    {
        return structureLevel != maxLevel && ResourcesManager.Instance.CanPay(GetUpgradeCostForNextLevel());
    }

    public virtual void InternalUpgradeStructure() { }

    public void UpgradeStructure()
    {
        if(CanUpgradeStructure())
        {
            ResourcesManager.Instance.Pay(GetUpgradeCostForNextLevel());
            structureLevel = GetNextLevel();
            building.UpgradeBuilding();
            InternalUpgradeStructure();
        }
    }
    public bool CanSellStructure()
    {
        List<Cost> sellingRefund = structureData.GetSellingRefundResourcesForLevel(structureLevel);
        return sellingRefund!= null && sellingRefund.Count != 0;
    }

    public void SellStructure(Vector3Int position)
    {
        if (CanSellStructure())
        {
            List<Cost> sellingRefund = structureData.GetSellingRefundResourcesForLevel(structureLevel);
            ResourcesManager.Instance.Repay(sellingRefund);
            TilesDataManager.Instance.RemoveStructureAtPos(position, false);
        }
    }

    public virtual void FillAreaOfEffectNeighbours() { }
}

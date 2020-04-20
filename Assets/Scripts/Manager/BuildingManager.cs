using System.Collections.Generic;
using UnityEngine;

// Dependecies to other managers:
//   Hard dependencies:
//     CommandManager
//     TilesDataManager
//     ResourcesManager
//     RelayManager

public class BuildingManager : Manager
{
    public static BuildingManager Instance { get; private set; }
    public bool IsInBuildMode { get; set; }
    public StructureType CurrentBuildingStructure { get; private set; }
    public List<Cost> staticResourcesCosts;

    #region Events
    public delegate void BuildDone();
    public static BuildDone OnBuildDone;
    #endregion

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // TODO: remove the getter?
    public List<Cost> GetStaticCosts()
    {
        return staticResourcesCosts;
    }

    public void BuildCurrentStructure(BaseTileData selectedTile)
    {
        if (selectedTile != null)
        {
            BuildCommand command = new BuildCommand(CurrentBuildingStructure, selectedTile.GridPosition);
            CommandManager.Instance.ExecuteCommand(command);
            OnBuildDone?.Invoke();
            IsInBuildMode = false;
        }
    }

    public void StructureToBuildSelected(StructureType type)
    {
        CurrentBuildingStructure = type;
        IsInBuildMode = true;
    }

    public void CancelBuildingMode()
    {
        IsInBuildMode = false;
    }

    public bool CanBuildStructureAtPos(StructureType type, Vector3Int pos)
    {
        BaseTileData data = TilesDataManager.Instance.GetTileDataAtPos(pos);
        bool canBuild = true;
        StructureBinding binding = TilesDataManager.Instance.GetStructureBindingFromType(type);
        canBuild = canBuild && binding.data.constructibleTerrainTypes.Contains(data.terrainTile.terrainType);
        canBuild = canBuild && data.structureTile == null;
        canBuild = canBuild && ResourcesManager.Instance.CanPay(binding.data.GetCreationCost());
        canBuild = canBuild && RelayManager.Instance.IsInsideRelayRange(data);
        return canBuild;
    }

    public void BuildStructureAtPos(StructureType type, Vector3Int pos)
    {
        if (CanBuildStructureAtPos(type, pos))
        {
            BaseTileData data = TilesDataManager.Instance.GetTileDataAtPos(pos);
            TilesDataManager.Instance.CreateStructureFromType(type, data);
            ResourcesManager.Instance.Pay(TilesDataManager.Instance.CostForStructure(type));
            data.structureTile.ActivateStructureIfPossible();
        }
    }

    public void RemoveStructureAtPos(Vector3Int pos, bool repay = true)
    {
        BaseTileData data = TilesDataManager.Instance.GetTileDataAtPos(pos);
        StructureTile structure = data.structureTile;
        if (structure != null && structure.building != null)
        {
            if (repay)
            {
                ResourcesManager.Instance.Repay(TilesDataManager.Instance.CostForStructure(data.structureTile.structureType));
            }

            // Warning: possible memory leak
            structure.DestroyStructure();
            data.structureTile = null;
            RelayManager.Instance.ComputeInRangeRelays();
        }
    }
}

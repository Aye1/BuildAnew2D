using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance { get; private set; }
    public bool IsInBuildMode { get; set; }
    public StructureType CurrentBuildingStructure { get; private set; }

    #region Events
    public delegate void BuildDone();
    public static BuildDone OnBuildDone;
    #endregion

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void BuildCurrentStructure()
    {
        BaseTileData selectedTile = MouseManager.Instance.HoveredTile;
        if (selectedTile != null)
        {
            BuildCommand command = new BuildCommand(CurrentBuildingStructure, selectedTile.gridPosition);
            CommandManager.Instance.ExecuteCommand(command);
            OnBuildDone?.Invoke();
            IsInBuildMode = false;
            UIManager.Instance.HideBuildingSelector();
        }
    }

    public void StructureToBuildSelected(StructureType type)
    {
        CurrentBuildingStructure = type;
        IsInBuildMode = true;
    }
}

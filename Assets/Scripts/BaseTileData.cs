using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BaseTileData : IActsOnTurnStart
{

    public Vector3 worldPosition;
    public TileBase originTile;

    public TerrainTile terrainTile;
    public StructureTile structureTile;

    #region Properties
    private bool _isSelected;
    public bool IsSelected
    {
        get
        {
            return _isSelected;
        }
        set
        {
            if (_isSelected != value)
            {
                _isSelected = value;
                terrainTile.SetIsSelected(_isSelected);
                structureTile?.SetIsSelected(_isSelected);
            }
        }
    }

    private Vector3Int _gridPosition;
    public Vector3Int GridPosition
    {
        get { return _gridPosition; }
        set
        {
            if (_gridPosition != null)
            {
                _gridPosition = value;
                if (terrainTile != null)
                {
                    terrainTile.GridPosition = _gridPosition;
                }
                if (structureTile != null)
                {
                    structureTile.GridPosition = _gridPosition;
                }
            }
        }
    }
    #endregion

    public BaseTileData() { }

    public BaseTileData(BaseTileData origin)
    {
        GridPosition = origin.GridPosition;
        worldPosition = origin.worldPosition;
        originTile = origin.originTile;
        terrainTile = origin.terrainTile;
        structureTile = origin.structureTile;
    }

    public bool GetIsSelected()
    {
        return _isSelected;
    }
    public void SetIsSelected(bool isSelected)
    {
        _isSelected = isSelected;
        terrainTile.SetIsSelected(isSelected);
        structureTile?.SetIsSelected(isSelected);      
    }
    public void OnTurnStarts()
    {
        IEnumerable<BaseTileData> neighbours = TilesDataManager.Instance.GetTilesAroundTile(this);
        terrainTile?.OnTurnStarts(neighbours);
        structureTile?.OnTurnStarts(neighbours);
    }

    public void PredictOnTurnStarts()
    {
        IEnumerable<BaseTileData> neighbours = TilesDataManager.Instance.GetTilesAroundTile(this);
        terrainTile?.PredictOnTurnStarts(neighbours);
        structureTile?.PredictOnTurnStarts(neighbours);
    }

    public void ApplyPrediction()
    {
        terrainTile?.ApplyPrediction();
        structureTile?.ApplyPrediction();
    }

    public string GetTerrainText()
    {
        return terrainTile == null ? "" : terrainTile.GetText();
    }

    public string GetStructureText()
    {
        return structureTile == null ? "" : structureTile.GetText();
    }

    public bool IsStructureOn()
    {
        return structureTile != null && structureTile.IsOn;
    }

    public ActivationState ToggleStructureIfPossible()
    {
        ActivationState returnActivationState = CanToggleStructure();
        if (returnActivationState == ActivationState.ActivationPossible)
        {
            returnActivationState = structureTile.ToggleStructureIfPossible();
        }
        return returnActivationState;
    }
    public ActivationState CanToggleStructure()
    {
        ActivationState returnActivationState = ActivationState.ImpossibleMissingStructure;
        
            if (structureTile != null)
            {
                returnActivationState = structureTile.CanToggleStructure();
            }
       
        return returnActivationState;
    }
    public void HandleFloodPrevision()
    {
        if (structureTile != null)
        {
            if (!structureTile.CanStructureBeFlooded())
            {
                structureTile.WarnStructureDestruction();
            }
        }
        terrainTile.terrainInfo.SetTerrainFloodable();
    }

    public void RemoveFloodPrevision()
    {
        if (structureTile != null)
        {
            structureTile.DisableWarnStructureDestruction();
        }
        terrainTile.terrainInfo.HideFloodableInfo();
    }

    public StructureTile HandleFlood()
    {
        terrainTile.terrainInfo.HideFloodableInfo();
        if (structureTile != null)
        {
            if (!structureTile.CanStructureBeFlooded())
            {
                structureTile = null;
                return structureTile;
            }
        }
        return null;
    }
}
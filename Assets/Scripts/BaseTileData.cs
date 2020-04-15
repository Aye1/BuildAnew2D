using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BaseTileData : IActsOnTurnStart
{
    private Vector3Int gridPosition;
    public Vector3Int GetGridPosition()
    {
        return gridPosition;
    }
    public void SetGridPosition(Vector3Int position)
    {
        gridPosition = position;
        if (terrainTile != null)
        {
            terrainTile.GridPosition = position;
        }
        if (structureTile != null)
        {
            structureTile.GridPosition = position;
        }
    }
    public Vector3 worldPosition;
    public TileBase originTile;

    public TerrainTile terrainTile;
    public StructureTile structureTile;
    private bool _isSelected;
    public BaseTileData() { }

    public BaseTileData(BaseTileData origin)
    {
        gridPosition = origin.gridPosition;
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
        if (RelayManager.Instance.IsInsideRelayRange(this))
        {
            if (structureTile != null)
            {
                returnActivationState = structureTile.CanToggleStructure();
            }
        }
        else
        {
            returnActivationState = ActivationState.OutsideRange;
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
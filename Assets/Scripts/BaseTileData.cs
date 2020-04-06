using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BaseTileData : IActsOnTurnStart
{
    public Vector3Int gridPosition;
    public Vector3 worldPosition;
    public TileBase originTile;

    public TerrainTile terrainTile;
    public StructureTile structureTile;

    public BaseTileData() { }

    public BaseTileData(BaseTileData origin)
    {
        gridPosition = origin.gridPosition;
        worldPosition = origin.worldPosition;
        originTile = origin.originTile;
        terrainTile = origin.terrainTile;
        structureTile = origin.structureTile;
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
        terrainTile.terrainInfo.ResetTerrainInfo();
    }

    public StructureTile HandleFlood()
    {
        terrainTile.terrainInfo.ResetTerrainInfo();
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
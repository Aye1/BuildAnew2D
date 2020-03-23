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

    public TerrainTile NTterraintile;
    public StructureTile NTstructureTile;

    public BaseTileData() { }

    public BaseTileData(BaseTileData origin)
    {
        gridPosition = origin.gridPosition;
        worldPosition = origin.worldPosition;
        originTile = origin.originTile;
        terrainTile = origin.terrainTile;
        structureTile = origin.structureTile;
        NTterraintile = origin.NTterraintile;
        NTstructureTile = origin.NTstructureTile;
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
        ActivationState returnActivationState = ActivationState.ImpossibleMissingStructure;
        if (structureTile != null)
        {
            returnActivationState = structureTile.ToggleStructureIfPossible();
        }
        return returnActivationState;
    }
}
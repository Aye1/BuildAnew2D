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

    public void OnTurnStarts()
    {
        IEnumerable<BaseTileData> neighbours = TilesDataManager.Instance.GetTilesAroundTile(this);
        terrainTile?.OnTurnStarts(neighbours);
        structureTile?.OnTurnStarts(neighbours);
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
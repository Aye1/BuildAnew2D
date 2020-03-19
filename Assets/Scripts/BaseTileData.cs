using System;
using UnityEngine;
using UnityEngine.Tilemaps;



public class BaseTileData
{
    public Vector3Int gridPosition;
    public Vector3 worldPosition;
    public TileBase originTile;

    public TerrainTile terrainTile;
    public StructureTile structureTile;

    public void OnTurnStarts(BaseTileData[] neighbours)
    {
        structureTile?.OnTurnStarts(neighbours);
        terrainTile?.OnTurnStarts(neighbours);
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
        return structureTile == null ? false : structureTile.IsOn;
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
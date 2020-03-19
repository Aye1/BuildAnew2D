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

    public bool CanToggleStructure()
    {
        bool canToggleStructure = false;
        if (structureTile != null)
        {
            int energyAvailable = ResourcesManager.Instance.EnergyAvailable;
            if ((structureTile.producesEnergy & structureTile.IsOn) ^ (structureTile.consumesEnergy & !structureTile.IsOn))
            {
                canToggleStructure = energyAvailable > 0;
            }
            else
            {
                canToggleStructure = true;
            }

        }
        return canToggleStructure;
    }

    public void ToggleStructureIfPossible()
    {
        if (CanToggleStructure())
        {
            structureTile.IsOn = !structureTile.IsOn;
        }
    }
}
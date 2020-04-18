using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class RelayManager : Manager
{
    public static RelayManager Instance { get; private set; }
    private List<BaseTileData> _relayInRange;
    private List<BaseTileData> _constructiblesTiles;
    private BaseTileData _mainRelayTile = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        _constructiblesTiles = new List<BaseTileData>();
        _relayInRange = new List<BaseTileData>();

    }

    public void Reset()
    {
        _mainRelayTile = null;
        _relayInRange.Clear();
        _constructiblesTiles.Clear();
    }

    public void RegisterStructure(BaseTileData structure)
    {
        if (structure.structureTile != null )
        {
            StructureType structureType = structure.structureTile.GetStructureType();
            if( structureType == StructureType.Relay ^ structureType == StructureType.MainRelay)
            {
                if(structureType == StructureType.MainRelay)
                {
                    _mainRelayTile = structure;
                }
                 ComputeConstructibleTerrainTiles();
            }
        }
    }
    public void ComputeInRangeRelays()
    {
        List<BaseTileData> oldConstructibles = new List<BaseTileData>();
        oldConstructibles.AddRange(_constructiblesTiles);
        ComputeConstructibleTerrainTiles();
        foreach (BaseTileData baseTile in oldConstructibles)
        {
            if(!_constructiblesTiles.Contains(baseTile))
            {
                 baseTile.terrainTile.terrainInfo.SetTerrainInconstructible();
                if (baseTile.structureTile != null)
                {
                    baseTile.structureTile.ForceDeactivation();
                }
            }
        }
    }

    //returns the difference between old computation and new one
    void ComputeConstructibleTerrainTiles()
    {
        _constructiblesTiles.Clear();
        _relayInRange.Clear();
        if(_mainRelayTile != null)
        {
            if(_mainRelayTile.structureTile != null) // mainRelayTile structure is null when cleaning the level
            {
                FindRelayInRangeRecursively(_mainRelayTile);
            }
        }

        foreach (BaseTileData baseTile in _constructiblesTiles)
        {
            baseTile.terrainTile.terrainInfo.SetTerrainConstructible();
        }
    }

    public void FindRelayInRangeRecursively(BaseTileData rootTile)
    {
        _relayInRange.Add(rootTile);

        List<BaseTileData> neighbour = GridUtils.GetNeighboursTilesOfRelay(rootTile); 
        foreach(BaseTileData tileData in neighbour)
        {
            if(!_constructiblesTiles.Contains(tileData))
            {
                _constructiblesTiles.Add(tileData);
            }
            if(tileData.structureTile != null && tileData.structureTile.GetStructureType() == StructureType.Relay && tileData.structureTile.IsOn)
            {
                if(!_relayInRange.Contains(tileData))
                {
                    FindRelayInRangeRecursively(tileData);
                }
            }
        }
    }

    public bool IsInsideRelayRange(BaseTileData basetileData)
    {
        return _constructiblesTiles.Contains(basetileData);
    }
}

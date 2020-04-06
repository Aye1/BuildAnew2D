using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class RelayManager : MonoBehaviour
{
    public static RelayManager Instance { get; private set; }
    private List<BaseTileData> _relayBastTileData;
    private IEnumerable<BaseTileData> _constructiblesTiles;
    public int _range = 2;
    // Start is called before the first frame update
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
        _relayBastTileData = new List<BaseTileData>();
        _constructiblesTiles = new List<BaseTileData>();

    }
    public void RegisterStructure(BaseTileData structure)
    {
        if (structure.structureTile != null && structure.structureTile.GetStructureType() == StructureType.Relay)
        {
            _relayBastTileData.Add(structure);
            IEnumerable<BaseTileData> intersection = ComputeConstructibleTerrainTiles();
            foreach (BaseTileData baseTile in intersection)
            {
                baseTile.terrainTile.terrainInfo.SetTerrainConstructible();                
            }
        }
    }
    public void UnregisterStructure(BaseTileData structure)
    {
        _relayBastTileData.Remove(structure);
        IEnumerable<BaseTileData> intersection = ComputeConstructibleTerrainTiles();
        foreach (BaseTileData baseTile in intersection)
        {
            baseTile.terrainTile.terrainInfo.SetTerrainInconstructible();
            if (baseTile.structureTile != null)
            {
                baseTile.structureTile.ForceDeactivation();
            }
        }
    }

    //returns the difference between old computation and new one
    List<BaseTileData> ComputeConstructibleTerrainTiles()
    {
        IEnumerable<BaseTileData> _constructiblesTilesintersection = _constructiblesTiles;
        _constructiblesTiles.ToList().Clear();

        List<Vector3Int> uniqueList = new List<Vector3Int>();
        foreach (BaseTileData structureTile in _relayBastTileData)
        {
            List<Vector3Int> list = new List<Vector3Int>();
            list = GridUtils.GetNeighboursPositionsAtDistance(structureTile.gridPosition, _range);
            foreach (Vector3Int vecInt in list)
            {
                if (!uniqueList.Contains(vecInt))
                {
                    uniqueList.Add(vecInt);
                }
            }
        }
        _constructiblesTiles = TilesDataManager.Instance.GetTilesAtPos(uniqueList);

        _constructiblesTilesintersection = _constructiblesTiles.Except(_constructiblesTilesintersection);
        return _constructiblesTilesintersection.ToList();
    }

    public bool IsInsideRelayRange(BaseTileData basetileData)
    {
        return _constructiblesTiles.Contains(basetileData);
    }
}

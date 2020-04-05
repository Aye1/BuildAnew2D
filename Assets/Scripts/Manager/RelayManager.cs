using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class RelayManager : MonoBehaviour
{
    public static RelayManager Instance { get; private set; }
    private List<BaseTileData> _relayBastTileData;
    private IEnumerable<BaseTileData> _constructiblesTiles;
    public GameObject gameobjectTest;
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
        if(structure.structureTile != null && structure.structureTile.GetStructureType() == StructureType.Relay)
        {
            _relayBastTileData.Add(structure);
            ComputeConstructibleTerrainTiles();
        }
    }
    public void UnregisterStructure(BaseTileData structure)
    {
         _relayBastTileData.Remove(structure);
        ComputeConstructibleTerrainTiles();
        //TODO deactivate structure outside of range ( comparer les deux list)
    }

    private void ComputeConstructibleTerrainTiles()
    {
        _constructiblesTiles.ToList().Clear();
        List<Vector3Int> uniqueList = new List<Vector3Int>();
        foreach(BaseTileData structureTile in _relayBastTileData)
        {
            List<Vector3Int> list = new List<Vector3Int>();
            list = GridUtils.GetNeighboursPositionsAtDistance(structureTile.gridPosition, _range);
            foreach(Vector3Int vecInt in list)
            {
                if(!uniqueList.Contains(vecInt))
                {
                    uniqueList.Add(vecInt);
                }
            }
        }
        _constructiblesTiles = TilesDataManager.Instance.GetTilesAtPos(uniqueList);
        foreach(BaseTileData baseTile in _constructiblesTiles)
        {
            Instantiate(gameobjectTest, baseTile.worldPosition, Quaternion.identity);
        }
        
    }

    public bool IsInsideRelayRange(BaseTileData basetileData)
    {
        return _constructiblesTiles.Contains(basetileData);
    }
}

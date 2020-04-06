using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

[System.Serializable]
public class StructureBinding
{
    public StructureType type;
    public Building building;
    public StructureData data;
    [SerializeField] public TileBase buildingTile;
}

[System.Serializable]
public class TerrainBinding
{
    public TerrainType type;
    public TerrainData terrainData;
    public TerrainInfo terrainVisualInfo;
    [SerializeField] public TileBase terrainTile;
}


public class TilesDataManager : MonoBehaviour
{
    private Tilemap _terrainTilemap;
    private Tilemap _structuresTilemap;

    #region Editor objects
#pragma warning disable 0649
    [SerializeField] private Grid _grid;
    [SerializeField] private List<StructureBinding> _structureTemplates;
    [SerializeField] private List<TerrainBinding> _terrainTemplates;
#pragma warning restore 0649
    #endregion

    private Tilemap _NTterrainTilemap;

    public List<BaseTileData> tiles;
    public List<BaseTileData> NTtiles;

    private List<BaseTileData> _modifiedNTTiles;

    public static TilesDataManager Instance { get; private set; }
    public static bool AreTileLoaded { get; private set; }

    private readonly Vector3 _tileOffset = new Vector3(0.0f, 0.25f, 0.0f);

    #region Events
    public delegate void TilesLoaded();
    public static event TilesLoaded OnTilesLoaded;

    public delegate void TilemapModified();
    public static event TilemapModified OnTilemapModified;
    #endregion

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

        TurnManager.OnTurnStart += GoToNextTurnState;
        GameManager.OnLevelLoaded += LoadLevel;
    }

    public void LoadLevel()
    {
        if(AreTileLoaded)
        {
            ClearLevel();
        }
        InitTerrainTiles();
        InitStructuresTiles();
        InitPredictedTiles();
        AreTileLoaded = true;
        OnTilesLoaded?.Invoke();
    }

    private void ClearLevel()
    {
        AreTileLoaded = false;
        foreach(BaseTileData tileData in tiles)
        {
            RemoveStructureAtPos(tileData.gridPosition, false);
        }
        RelayManager.Instance.Reset();
        Destroy(_terrainTilemap.gameObject);
        Destroy(_structuresTilemap.gameObject);
        Destroy(_NTterrainTilemap.gameObject);
    }

    #region Init
    private void InitTerrainTiles()
    {
        
        _terrainTilemap = Instantiate(GameManager.Instance.GetLevelData().GetTerrainTilemap(), Vector3.zero, Quaternion.identity, _grid.transform);
        tiles = new List<BaseTileData>();
        foreach (Vector3Int pos in _terrainTilemap.cellBounds.allPositionsWithin)
        {
            TileBase tile = _terrainTilemap.GetTile(pos);
            if (tile != null)
            {
                BaseTileData newTileData = new BaseTileData();
                newTileData.gridPosition = pos;
                newTileData.originTile = tile;
                newTileData.worldPosition = _terrainTilemap.CellToWorld(pos) + _tileOffset;
                CreateBaseTileData(tile, newTileData);
                tiles.Add(newTileData);
            }
        }
    }

    private void InitStructuresTiles()
    {
        _structuresTilemap = Instantiate(GameManager.Instance.GetLevelData().GetStructureTilemap(), Vector3.zero, Quaternion.identity, _grid.transform);

        foreach (Vector3Int pos in _structuresTilemap.cellBounds.allPositionsWithin)
        {
            TileBase tile = _structuresTilemap.GetTile(pos);
            BaseTileData data = GetTileDataAtPos(pos);
            if(data == null)
            {
                Debug.LogWarning("Structure built on empty tile at position " + pos.ToString());
            }
            else if (tile != null)
            {
                StructureType type = GetStructureTypeFromTile(tile);
                CreateStructureFromType(type, data);
            }
        }
        _structuresTilemap.gameObject.SetActive(false);

    }

    private void InitPredictedTiles()
    {
        _NTterrainTilemap = Instantiate(_terrainTilemap, Vector3.zero, Quaternion.identity, _grid.transform);
        _NTterrainTilemap.name = "Next turn Terrain Tilemap";
        _NTterrainTilemap.GetComponent<TilemapRenderer>().sortOrder = 0;
        _NTterrainTilemap.gameObject.SetActive(false);
        NTtiles = new List<BaseTileData>(tiles);

        _modifiedNTTiles = new List<BaseTileData>();
    }
    #endregion

    #region Build and destroy structures
    public void BuildStructureAtPos(StructureType type, Vector3Int pos)
    {
        if (CanBuildStructureAtPos(type, pos))
        {
            BaseTileData data = GetTileDataAtPos(pos);
            CreateStructureFromType(type, data);
            ResourcesManager.Instance.Pay(CostForStructure(type));
            data.structureTile.ActivateStructureIfPossible();
        }
    }

    public bool CanBuildStructureAtPos(StructureType type, Vector3Int pos)
    {
        BaseTileData data = GetTileDataAtPos(pos);
        bool canBuild = true;
        StructureBinding binding = GetStructureBindingFromType(type);
        canBuild = canBuild && binding.data.constructibleTerrainTypes.Contains(data.terrainTile.terrainType);
        canBuild = canBuild && data.structureTile == null;
        canBuild = canBuild && ResourcesManager.Instance.CanPay(CostForStructure(type));
        canBuild = canBuild && RelayManager.Instance.IsInsideRelayRange(data);
        return canBuild;
    }

    public void RemoveStructureAtPos(Vector3Int pos, bool repay = true)
    {
        BaseTileData data = GetTileDataAtPos(pos);
        StructureTile structure = data.structureTile;
        if (structure != null && structure.building != null)
        {
            if (repay)
            {
                ResourcesManager.Instance.Repay(CostForStructure(data.structureTile.structureType));
            }

            if(structure is PowerPlantTile)
            {
                WaterClusterManager.Instance.UnregisterPumpingStation(data);
            }

            // Warning: possible memory leak
            structure.DestroyStructure();
            Destroy(structure.building.gameObject);
            data.structureTile = null;
            RelayManager.Instance.ComputeInRangeRelays();
        }
    }
    #endregion

    #region Get Tiles
    private List<BaseTileData> GetTiles(bool predict)
    {
        return predict ? NTtiles : tiles;
    }

    private Tilemap GetTerrainTilemap(bool predict)
    {
        return predict ? _NTterrainTilemap : _terrainTilemap;
    }

    public bool HasTile(Vector3Int pos)
    {
        return _terrainTilemap.HasTile(pos);
    }

    public BaseTileData GetTileDataAtPos(Vector3Int pos, bool predict = false)
    {
        BaseTileData data = GetTiles(predict).FirstOrDefault(x => x.gridPosition == pos);
        return data;
    }

    public IEnumerable<BaseTileData> GetTilesInBounds(BoundsInt bounds, bool predict = false)
    {
        List<BaseTileData> res = new List<BaseTileData>();
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            BaseTileData tile = GetTileDataAtPos(pos, predict);
            if (tile != null)
            {
                res.Add(tile);
            }
        }
        return res;
    }

    public IEnumerable<BaseTileData> GetTilesAtPos(IEnumerable<Vector3Int> positions, bool predict = false)
    {
        List<BaseTileData> res = new List<BaseTileData>();
        foreach (Vector3Int pos in positions)
        {
            BaseTileData tile = GetTileDataAtPos(pos, predict);
            if (tile != null)
            {
                res.Add(tile);
            }
        }
        return res.ToArray();

    }

    public IEnumerable<BaseTileData> GetTilesAroundTileAtPos(Vector3Int pos, bool predict = false)
    {
        if (GetTerrainTilemap(predict).cellBounds.Contains(pos))
        {
            BoundsInt localBounds = new BoundsInt(pos.x - 1, pos.y - 1, pos.z, 3, 3, 1);

            return GetTilesInBounds(localBounds);
        }
        return null;
    }

    public IEnumerable<BaseTileData> GetTilesDirectlyAroundTileAtPos(Vector3Int pos, bool predict = false)
    {
        if (GetTerrainTilemap(predict).cellBounds.Contains(pos))
        {
            return GetTilesAtPos(GridUtils.GetDirectNeighboursPositions(pos), predict);
        }
        return null;
    }

    public IEnumerable<BaseTileData> GetTilesAroundTile(BaseTileData tile, bool predict = false)
    {
        return GetTilesAroundTileAtPos(tile.gridPosition, predict);
    }

    public IEnumerable<BaseTileData> GetTilesDirectlyAroundTile(BaseTileData tile, bool predict = false)
    {
        return GetTilesDirectlyAroundTileAtPos(tile.gridPosition, predict);
    }

    public BaseTileData GetTileAtWorldPos(Vector3 pos, bool predict = false)
    {
        Vector3Int tilePos = GetTerrainTilemap(predict).WorldToCell(pos);
        return GetTileDataAtPos(tilePos);
    }

    public IEnumerable<BaseTileData> GetTilesWithTerrainType(TerrainType type, bool predict = false)
    {
        return GetTiles(predict).Where(x => x.terrainTile.terrainType == type);
    }
    public IEnumerable<BaseTileData> GetTilesWithStrucureType(StructureType type)
    {
        return tiles.Where(x => (x.structureTile != null && x.structureTile.structureType == type));
    }
    #endregion

    public BoundsInt GetTilemapBounds()
    {
        return _terrainTilemap.cellBounds;
    }

    public void SwapNextTurnTiles(BaseTileData oldTile, BaseTileData newTile)
    {
        NTtiles.Remove(oldTile);
        NTtiles.Add(newTile);
        _modifiedNTTiles.Add(oldTile);
        OnTilemapModified?.Invoke();
    }

    public void ChangeTerrainTileInTilemap(Vector3Int position, TerrainType type, bool predict = false)
    {
        TerrainBinding binding = GetTerrainBindingFromType(type);
        TileBase tilebase = binding?.terrainTile;
        GetTerrainTilemap(predict).SetTile(position, tilebase);
    }

    private void GoToNextTurnState()
    {
        foreach (BaseTileData oldTile in _modifiedNTTiles)
        {
            BaseTileData newTile = GetTileDataAtPos(oldTile.gridPosition, true);
            if(newTile.structureTile == null && oldTile.structureTile != null)
            {
                RemoveStructureAtPos(oldTile.gridPosition, false);
            }
            SwapTileFromCurrentToNewTilemap(oldTile, newTile, false);
            Destroy(oldTile.terrainTile.terrainInfo.gameObject);
        }

        _modifiedNTTiles.Clear();

        foreach (BaseTileData oldTile in tiles)
        {
            oldTile.ApplyPrediction();
        }
    }

    private void SwapTileFromCurrentToNewTilemap(BaseTileData oldTile, BaseTileData newTile, bool predict = false)
    {
        tiles.Remove(oldTile);
        tiles.Add(newTile);
        ChangeTerrainTileInTilemap(oldTile.gridPosition, newTile.terrainTile.terrainType, false);
    }

    #region Bindings

    public void CreateBaseTileData(TileBase tile, BaseTileData data)
    {
        TerrainBinding binding = GetTerrainBindingFromTile(tile);
        CreateTerrainFromType(binding.type, data);
    }

    public StructureType GetStructureTypeFromTile(TileBase tile)
    {
        StructureType returnType = StructureType.None;
        StructureBinding binding = GetStructureBindingFromTile(tile);
        if (binding != null)
        {
            returnType = binding.type;
        }
        return returnType;
    }

    public StructureBinding GetStructureBindingFromTile(TileBase tile)
    {
        foreach (StructureBinding structureBinding in _structureTemplates)
        {
            if (structureBinding.buildingTile.name.Equals(tile.name))
            {
                return structureBinding;
            }
        }
        return null;
    }

    public StructureBinding GetStructureBindingFromType(StructureType type)
    {
        foreach (StructureBinding structureBinding in _structureTemplates)
        {
            if (structureBinding.type == type)
            {
                return structureBinding;
            }
        }
        return null;
    }

    public TerrainType GetTerrainTypeFromTile(TileBase tile)
    {
        TerrainType returnType = TerrainType.Default;
        TerrainBinding binding = GetTerrainBindingFromTile(tile);
        if (binding != null)
        {
            returnType = binding.type;
        }
        return returnType;
    }

    public TerrainBinding GetTerrainBindingFromTile(TileBase tile)
    {
        foreach (TerrainBinding terrainBinding in _terrainTemplates)
        {
            if (terrainBinding.terrainTile.name.Equals(tile.name))
            {
                return terrainBinding;
            }
        }
        return null;
    }

    public TerrainBinding GetTerrainBindingFromType(TerrainType type)
    {
        foreach (TerrainBinding terrainBinding in _terrainTemplates)
        {
            if (terrainBinding.type == type)
            {
                return terrainBinding;
            }
        }
        return null;
    }
    #endregion

    public StructureTile CreateStructureFromType(StructureType type, BaseTileData data)
    {
        StructureTile newTile = null;

        StructureBinding structureBinding = GetStructureBindingFromType(type);
        if (structureBinding != null)
        {
            switch (type)
            {
                case StructureType.PowerPlant:
                    newTile = new PowerPlantTile();
                    break;

                case StructureType.Sawmill:
                    newTile = new SawmillTile();
                    break;

                case StructureType.PumpingStation:
                    newTile = new PumpingStationTile();
                    WaterClusterManager.Instance.RegisterPumpingStation(data);
                    break;

                case StructureType.Village:
                    newTile = new VillageTile();
                    break;

                case StructureType.Mine:
                    newTile = new MineTile();
                    break;
                case StructureType.Relay:
                    newTile = new RelayTile();
                    break;
                case StructureType.MainRelay:
                    newTile = new MainRelayTile();
                    break;

                default:
                    throw new MissingStructureTypeDefinitionException();
            }
            data.structureTile = newTile;
            Building building = Instantiate(structureBinding.building, data.worldPosition, Quaternion.identity, transform);
            building.dataTile = newTile;
            newTile.building = building;
            ResourcesManager.Instance.RegisterStructure(newTile);
            RelayManager.Instance.RegisterStructure(data);
        }
        return newTile;
    }

    public TerrainTile CreateTerrainFromType(TerrainType type, BaseTileData baseTileData)
    {
        TerrainTile newTile = null;

        TerrainBinding terrainBinding = GetTerrainBindingFromType(type);
        if (terrainBinding != null)
        {
            switch (type)
            {
                case TerrainType.Plains:
                    newTile = new PlainsTile();
                    break;

                case TerrainType.Water:
                    newTile = new WaterTile();
                    break;

                case TerrainType.Wood:
                    newTile = new WoodsTile();
                    break;
                case TerrainType.Stone:
                    newTile = new StoneTile();
                    break;

                case TerrainType.Sand:
                    newTile = new SandTile();
                    break;

                default:
                    throw new MissingTerrainTypeDefinitionException();
            }
        }
        baseTileData.terrainTile = newTile;

        TerrainInfo terrainInfo = Instantiate(terrainBinding.terrainVisualInfo, baseTileData.worldPosition, Quaternion.identity, _terrainTilemap.transform);
        terrainInfo.dataTile = newTile;
        newTile.terrainInfo = terrainInfo;
        return newTile;
    }

    public Sprite GetSpriteForStructure(StructureType type)
    {
        StructureBinding structureBinding = GetStructureBindingFromType(type);
        if (structureBinding != null && structureBinding.building != null)
        {
            return structureBinding.building.GetComponent<SpriteRenderer>().sprite;
        }
        return null;
    }

    public StructureData GetDataForStructure(StructureType type)
    {
        StructureBinding element = _structureTemplates.First(x => x.type == type);
        return element?.data;
    }


    public IEnumerable<StructureBinding> GetAllConstructiblesStructures()
    {
        return _structureTemplates.Where(x => x.data.upgradeData != null );
    }

    public List<Cost> CostForStructure(StructureType type)
    {
        return _structureTemplates.First(x => x.type == type).data.GetCreationCost();
    }

    internal TerrainData GetDataForTerrain(TerrainType terrainType)
    {
        TerrainBinding element = _terrainTemplates.First(x => x.type == terrainType);
        return element?.terrainData;
    }
}

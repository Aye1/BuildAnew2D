using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

// Dependecies to other managers:
//   Hard dependencies:
//     GameManager
//     RelayManager
//     ResourcesManager
//     LevelManager
//   Soft dependencies
//     TurnManager


[System.Serializable]
public class TerrainBinding
{
    public TerrainType type;
    public TerrainData terrainData;
    public TerrainInfo terrainVisualInfo;
    [SerializeField] public TileBase terrainTile;
}

public class TilesDataManager : Manager
{
    #region Editor objects
#pragma warning disable 0649
    [Header("Tiles data")]
    [SerializeField] private List<TerrainBinding> _terrainTemplates;
#pragma warning restore 0649
    #endregion

    private Grid _grid;
    private Tilemap _terrainTilemap;
    private Tilemap _structuresTilemap;
    private Tilemap _NTterrainTilemap;
    private List<BaseTileData> _modifiedNTTiles;
    private readonly Vector3 _tileOffset = new Vector3(0.0f, 0.25f, 0.0f);

    public List<BaseTileData> tiles;
    public List<BaseTileData> NTtiles;

    public static TilesDataManager Instance { get; private set; }
    public static bool AreTileLoaded { get; private set; }

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
            InitState = InitializationState.Initializing;
        }
        else
        {
            Destroy(gameObject);
        }
        _grid = FindObjectOfType<Grid>();
        RegisterCallbacks();
    }

    private void Start()
    {
        LoadLevel();
    }

    public void ReloadLevel()
    {
        InitState = InitializationState.Updating;
        ClearLevel();
        LoadLevel();
    }

    private void LoadLevel()
    {
        AreTileLoaded = false;
        InitTerrainTiles();
        InitStructuresTiles();
        InitPredictedTiles();
        AskForSpecificManagers();
        AreTileLoaded = true;
        OnTilesLoaded?.Invoke();
        InitState = InitializationState.Ready;
    }

    private void RegisterCallbacks()
    {
        TurnManager.OnTurnStart += GoToNextTurnState;
        LevelManager.OnLevelNeedReset += ReloadLevel;
    }

    private void UnregisterCallbacks()
    {
        TurnManager.OnTurnStart -= GoToNextTurnState;
        LevelManager.OnLevelNeedReset -= ReloadLevel;
    }

    private void OnDestroy()
    {
        UnregisterCallbacks();
        AreTileLoaded = false;
    }

    private void ClearLevel()
    {
        Debug.Log("Clear level");
        AreTileLoaded = false;
        foreach (BaseTileData tileData in tiles)
        {
            DestroyStructureAtPos(tileData.GridPosition);
        }
        foreach (BaseTileData tileData in tiles) //clear building before terrain
        {
            Destroy(tileData.terrainTile.terrainInfo.gameObject);
        }

        RelayManager.Instance.Reset();
        Destroy(_terrainTilemap.gameObject);
        Destroy(_structuresTilemap.gameObject);
        Destroy(_NTterrainTilemap.gameObject);
    }


    #region Init
    private void InitTerrainTiles()
    {
        Debug.Log("Init terrain tiles");
        _terrainTilemap = Instantiate(LevelManager.Instance.GetCurrentLevel().GetTerrainTilemap(), Vector3.zero, Quaternion.identity, _grid.transform);
        tiles = new List<BaseTileData>();
        foreach (Vector3Int pos in _terrainTilemap.cellBounds.allPositionsWithin)
        {
            TileBase tile = _terrainTilemap.GetTile(pos);
            if (tile != null)
            {
                BaseTileData newTileData = new BaseTileData();
                newTileData.GridPosition = pos;
                newTileData.originTile = tile;
                newTileData.worldPosition = _terrainTilemap.CellToWorld(pos) + _tileOffset;
                CreateBaseTileData(tile, newTileData);
                tiles.Add(newTileData);
            }
        }
    }

    private void InitStructuresTiles()
    {
        Debug.Log("Init structure tiles");
        _structuresTilemap = Instantiate(LevelManager.Instance.GetCurrentLevel().GetStructureTilemap(), Vector3.zero, Quaternion.identity, _grid.transform);

        foreach (Vector3Int pos in _structuresTilemap.cellBounds.allPositionsWithin)
        {
            TileBase tile = _structuresTilemap.GetTile(pos);
            BaseTileData data = GetTileDataAtPos(pos);
            if (data == null)
            {
                Debug.LogWarning("Structure built on empty tile at position " + pos.ToString());
            }
            else if (tile != null)
            {
                StructureType type = BuildingManager.Instance.StructuresTemplates.GetStructureTypeFromTile(tile);
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

    private void AskForSpecificManagers()
    {
        if (tiles.Any(x => x.terrainTile is WaterTile))
        {
            InitializationManager.Instance.AskForManagerCreation(typeof(WaterClusterManager));
        }
    }
    #endregion

    #region Build and destroy structures

    public void DestroyStructureAtPos(Vector3Int pos)
    {
        BaseTileData data = GetTileDataAtPos(pos);
        StructureTile structure = data.structureTile;
        if (structure != null && structure._building != null)
        {
            structure.DestroyStructure();
            data.structureTile = null;
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
        BaseTileData data = GetTiles(predict).FirstOrDefault(x => x.GridPosition == pos);
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
        return GetTilesAtPos(GridUtils.GetNeighboursPositionsAtDistance(tile.GridPosition, 1), predict);
    }

    public IEnumerable<BaseTileData> GetTilesDirectlyAroundTile(BaseTileData tile, bool predict = false)
    {
        return GetTilesDirectlyAroundTileAtPos(tile.GridPosition, predict);
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
    public IEnumerable<BaseTileData> GetTilesWithStrucureType(StructureType type, bool predict = false)
    {
        return GetTiles(predict).Where(x => (x.structureTile != null && x.structureTile._structureType == type));
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
            BaseTileData newTile = GetTileDataAtPos(oldTile.GridPosition, true);
            if (newTile.structureTile == null && oldTile.structureTile != null)
            {
                DestroyStructureAtPos(oldTile.GridPosition);
            }
            SwapTileFromCurrentToNewTilemap(oldTile, newTile, false);
        }
        foreach (BaseTileData oldTile in _modifiedNTTiles)
        {
            Destroy(oldTile.terrainTile.terrainInfo.gameObject);
        }

        _modifiedNTTiles.Clear();

        foreach (BaseTileData oldTile in tiles)
        {
            oldTile.ApplyPrediction();
        }
        RelayManager.Instance.ComputeInRangeRelays();
    }

    private void SwapTileFromCurrentToNewTilemap(BaseTileData oldTile, BaseTileData newTile, bool predict = false)
    {
        tiles.Remove(oldTile);
        tiles.Add(newTile);
        ChangeTerrainTileInTilemap(oldTile.GridPosition, newTile.terrainTile.terrainType, false);
    }

    #region Bindings

    public void CreateBaseTileData(TileBase tile, BaseTileData data)
    {
        TerrainBinding binding = GetTerrainBindingFromTile(tile);
        CreateTerrainFromType(binding.type, data);
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

        StructureBinding structureBinding = BuildingManager.Instance.StructuresTemplates.GetStructureBindingFromType(type);
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
            BuildingView building = Instantiate(structureBinding.building, data.worldPosition, Quaternion.identity, transform);
            building.dataTile = newTile;
            newTile._building = building;
            newTile.GridPosition = data.GridPosition;
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
        newTile.GridPosition = baseTileData.GridPosition;
        return newTile;
    }

    
    internal TerrainData GetDataForTerrain(TerrainType terrainType)
    {
        TerrainBinding element = _terrainTemplates.First(x => x.type == terrainType);
        return element?.terrainData;
    }
}

using System.Collections;
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
    [SerializeField] public TileBase terrainTile;
}


public class TilesDataManager : MonoBehaviour
{
    #region Editor objects
#pragma warning disable 0649
    [SerializeField] private Tilemap _terrainTilemap;
    [SerializeField] private Tilemap _structuresTilemap;
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

    private const string PLAINS = "PlainsTile";
    private const string WATER = "WaterRuleTile";
    private const string WOODS = "WoodsTile";
    private const string POWERPLANT = "PowerPlantTile";
    private const string SAWMILL = "SawmillTile";
    private const string PUMPINGSTATION = "PumpingStationTile";

    private readonly Vector3 _tileOffset = new Vector3(0.0f, 0.25f, 0.0f);

    #region Events
    public delegate void TilesLoaded();
    public static event TilesLoaded OnTilesLoaded;
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
    }

    // Start is called before the first frame update
    void Start()
    {
        InitTerrainTiles();
        InitStructuresTiles();
        InitPredictedTiles();
        AreTileLoaded = true;
        OnTilesLoaded?.Invoke();
        TurnManager.OnTurnStart += GoToNextTurnState;
    }

    #region Init
    private void InitTerrainTiles()
    {
        tiles = new List<BaseTileData>();
        foreach (Vector3Int pos in _terrainTilemap.cellBounds.allPositionsWithin)
        {
            TileBase tile = _terrainTilemap.GetTile(pos);
            if (tile != null)
            {
                BaseTileData newTileData = CreateBaseTileData(tile);
                newTileData.gridPosition = pos;
                newTileData.originTile = tile;
                newTileData.worldPosition = _terrainTilemap.CellToWorld(pos) + _tileOffset;
                tiles.Add(newTileData);
            }
        }
    }

    private void InitStructuresTiles()
    {
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
                data.structureTile = CreateStructureFromType(type, data);
            }
        }
    }

    private void InitPredictedTiles()
    {
        _NTterrainTilemap = Instantiate(_terrainTilemap, Vector3.zero, Quaternion.identity, _terrainTilemap.GetComponentInParent<Grid>().transform);
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
            data.structureTile = CreateStructureFromType(type, data);
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

            // Warning: possible memory leak
            structure.DestroyStructure();
            Destroy(structure.building.gameObject);
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
            return GetTilesAtPos(GetDirectNeighboursPositions(pos), predict);
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

    // TODO: move into an helper/utils class
    public IEnumerable<Vector3Int> GetDirectNeighboursPositions(Vector3Int position)
    {
        IEnumerable<Vector3Int> neighbours = new List<Vector3Int>
        {
            new Vector3Int(position.x - 1, position.y, position.z),
            new Vector3Int(position.x + 1, position.y, position.z),
            new Vector3Int(position.x, position.y - 1, position.z),
            new Vector3Int(position.x, position.y + 1, position.z)
        };
        return neighbours;
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

    public void ChangeTileTerrain(Vector3Int position, TerrainType type, bool predict = false)
    {
        TerrainTile newTerrain = CreateTerrainFromType(type);
        BaseTileData oldTile = GetTileDataAtPos(position, predict);
        SwapTileTerrain(oldTile, newTerrain, predict);
    }

    public void SwapTileTerrain(BaseTileData baseTile, TerrainTile newTileTerrain, bool predict = false)
    {
        TerrainBinding terrainBinding = GetTerrainBindingFromType(newTileTerrain.terrainType);
        if (terrainBinding != null)
        {
            TileBase newTilebase = terrainBinding.terrainTile;
            GetTerrainTilemap(predict).SetTile(baseTile.gridPosition, newTilebase);
            baseTile.terrainTile = newTileTerrain;
            if (predict)
            {
                _modifiedNTTiles.Add(baseTile);
            }
        }
    }

    public BaseTileData CreateNewTileForNextTurn(BaseTileData oldTile, TerrainType type)
    {
        BaseTileData createdTile = new BaseTileData(oldTile);
        createdTile.terrainTile = CreateTerrainFromType(type);
        ChangeTerrainTile(createdTile.gridPosition, TerrainType.Water, true);
        createdTile.HandleFlood();
        oldTile.HandleFloodPrevision();
        NTtiles.Remove(oldTile);
        NTtiles.Add(createdTile);
        _modifiedNTTiles.Add(oldTile);
        return createdTile;
    }

    public void ChangeTerrainTile(Vector3Int position, TerrainType type, bool predict = false)
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
        ChangeTerrainTile(oldTile.gridPosition, newTile.terrainTile.terrainType, false);
    }

    #region Bindings

    public BaseTileData CreateBaseTileData(TileBase tile)
    {
        BaseTileData data = new BaseTileData();
        TerrainBinding binding = GetTerrainBindingFromTile(tile);
        data.terrainTile = CreateTerrainFromType(binding.type);
        return data;
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
                    break;

                case StructureType.Village:
                    newTile = new VillageTile();
                    break;

                case StructureType.Mine:
                    newTile = new MineTile();
                    break;

                default:
                    throw new MissingStructureTypeDefinitionException();
            }

            Building building = Instantiate(structureBinding.building, data.worldPosition, Quaternion.identity, transform);
            building.dataTile = newTile;
            newTile.building = building;
            ResourcesManager.Instance.RegisterStructure(newTile);
        }
        return newTile;
    }

    public TerrainTile CreateTerrainFromType(TerrainType type)
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
        return _structureTemplates.Where(x => x.data.isConstructible);
    }

    public List<Cost> CostForStructure(StructureType type)
    {
        return _structureTemplates.First(x => x.type == type).data.costs;
    }

}

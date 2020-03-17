using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class TilesDataManager : MonoBehaviour
{
    #region Editor objects
#pragma warning disable 0649
    [SerializeField] private Tilemap _terrainTilemap;
    [SerializeField] private Tilemap _structuresTilemap;

    [SerializeField] private PowerPlant _powerPlantTemplate;
    [SerializeField] private Sawmill _sawmillTemplate;
    [SerializeField] private PumpingStation _pumpingStationTemplate;
#pragma warning restore 0649
    #endregion

    public Dictionary<Vector3Int, BaseTileData> tiles;
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
        if(Instance == null)
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
        AreTileLoaded = true;
        OnTilesLoaded?.Invoke();
    }

    private void InitTerrainTiles()
    {
        tiles = new Dictionary<Vector3Int, BaseTileData>();
        foreach(Vector3Int pos in _terrainTilemap.cellBounds.allPositionsWithin)
        {
            TileBase tile = _terrainTilemap.GetTile(pos);
            if(tile != null)
            {
                BaseTileData newTileData = GetTileDataFromTileBase(tile);
                newTileData.gridPosition = pos;
                newTileData.originTile = tile;
                newTileData.worldPosition = _terrainTilemap.CellToWorld(pos) + _tileOffset;
                tiles.Add(pos, newTileData);
            }
        }
    }

    private void InitStructuresTiles()
    { 
        foreach(Vector3Int pos in _structuresTilemap.cellBounds.allPositionsWithin)
        {
            TileBase tile = _structuresTilemap.GetTile(pos);
            BaseTileData data = GetTileDataAtPos(pos);
            if(data == null)
            {
                Debug.LogWarning("Structure buit on empty tile at position " + pos.ToString());
            }
            else
            {
                if (tile != null)
                {
                    StructureType type = GetTypeFromTile(tile);
                    data.structureTile = CreateStructureFromType(type, data);
                }
            }
        }
    }

    public bool HasTile(Vector3Int pos)
    {
        return _terrainTilemap.HasTile(pos);
    }

    public void BuildStructureAtPos(StructureType type, Vector3Int pos)
    {
        if(CanBuildStructureAtPos(type, pos))
        {
            BaseTileData data = GetTileDataAtPos(pos);
            data.structureTile = CreateStructureFromType(type, data);
        }
    }

    public bool CanBuildStructureAtPos(StructureType type, Vector3Int pos)
    {
        BaseTileData data = GetTileDataAtPos(pos);
        bool canBuild = true;
        canBuild = canBuild && !data.terrainTile.Equals(TerrainType.Water);
        canBuild = canBuild && data.structureTile == null;
        return canBuild;
    }

    public void RemoveStructureAtPos(Vector3Int pos)
    {
        BaseTileData data = GetTileDataAtPos(pos);
        StructureTile structure = data.structureTile;
        if (structure != null && structure.building != null)
        {
            Destroy(structure.building.gameObject);
            data.structureTile = null;
            // Warning: possible memory leak
        }
    }

    #region Get Tiles
    public BaseTileData GetTileDataAtPos(Vector3Int pos)
    {
        BaseTileData data = tiles.FirstOrDefault(x => x.Value.gridPosition == pos).Value;
        return data;
    }

    public BaseTileData[] GetTilesInBounds(BoundsInt bounds)
    {
        List<BaseTileData> res = new List<BaseTileData>();
        foreach(Vector3Int pos in bounds.allPositionsWithin)
        {
            BaseTileData tile = GetTileDataAtPos(pos);
            if(tile != null)
            {
                res.Add(tile);
            }
        }
        return res.ToArray();
    }

    public BaseTileData[] GetTilesAroundTileAtPos(Vector3Int pos)
    {
        if (_terrainTilemap.cellBounds.Contains(pos))
        {
            BoundsInt localBounds = new BoundsInt(pos.x - 1, pos.y - 1, pos.z, 3, 3, 1);

            return GetTilesInBounds(localBounds);
        }
        return null;
    }

    public BaseTileData[] GetTilesAroundTile(BaseTileData tile)
    {
        return GetTilesAroundTileAtPos(tile.gridPosition);
    }

    public BaseTileData GetTileAtWorldPos(Vector3 pos)
    {
        Vector3Int tilePos = _terrainTilemap.WorldToCell(pos);
        return GetTileDataAtPos(tilePos);
    }
    #endregion

    #region Bindings

    public BaseTileData GetTileDataFromTileBase(TileBase tile) 
    {
        BaseTileData data = new BaseTileData();
        data.terrainTile = GetTerrainFromTileBase(tile);
        return data;
    }

    public TerrainTile GetTerrainFromTileBase(TileBase tile)
    {
        if (tile.name.Equals(PLAINS))
        {
            return new PlainsTile();
        }
        if (tile.name.Equals(WATER))
        {
            return new WaterTile();
        }
        if (tile.name.Equals(WOODS))
        {
            return new WoodsTile();
        }
        return new DefaultTile();
    }

    public StructureType GetTypeFromTile(TileBase tile)
    {
        if (tile.name.Equals(POWERPLANT))
        {
            return StructureType.PowerPlant;
        }

        if (tile.name.Equals(SAWMILL))
        {
            return StructureType.Sawmill;
        }

        if (tile.name.Equals(PUMPINGSTATION))
        {
            return StructureType.PumpingStation;
        }
        return StructureType.None;
    }
    #endregion

    public StructureTile CreateStructureFromType(StructureType type, BaseTileData data)
    {
        Building toInstantiate = null;
        StructureTile newTile = null;
        switch (type)
        {
            case StructureType.PowerPlant:
                toInstantiate = _powerPlantTemplate;
                newTile = new PowerPlantTile();
                break;

            case StructureType.Sawmill:
                toInstantiate = _sawmillTemplate;
                newTile = new SawmillTile();
                break;

            case StructureType.PumpingStation:
                toInstantiate = _pumpingStationTemplate;
                newTile = new PumpingStationTile();
                break;  

            case StructureType.None:
                break;
        }

        if (toInstantiate != null)
        {
            Building building = Instantiate(toInstantiate, data.worldPosition, Quaternion.identity, transform);
            building.dataTile = newTile;
            newTile.building = building;
            ResourcesManager.Instance.RegisterStructure(newTile);
        }
        return newTile;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class TilesDataManager : MonoBehaviour
{

    [SerializeField] private Tilemap _terrainTilemap;
    [SerializeField] private Tilemap _structuresTilemap;
    public Dictionary<Vector3Int, BaseTileData> tiles;
    public static TilesDataManager Instance { get; private set; }

    [SerializeField] private GameObject _factoryTemplate;


    private const string PLAINS = "PlainsTile";
    private const string WATER = "WaterRuleTile";
    private const string FACTORY = "FactoryTile";

    private readonly Vector3 _tileOffset = new Vector3(0.0f, 0.25f, 0.0f);

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
                newTileData.Init();
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
                    data.structureType = GetStructureFromTileBase(tile);
                    Instantiate(_factoryTemplate, data.worldPosition, Quaternion.identity, transform);
                }
            }
        }
    }

    public bool HasTile(Vector3Int pos)
    {
        return _terrainTilemap.HasTile(pos);
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
        if (tile.name.Equals(PLAINS))
        {
            return new PlainsTile();
        }

        if (tile.name.Equals(WATER))
        {
            return new WaterTile();
        }
        return new DefaultTile();
    }

    public StructureType GetStructureFromTileBase(TileBase tile)
    {
        if (tile.name.Equals(FACTORY)) {
            return StructureType.Factory;
        }
        return StructureType.None;
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class TilesDataManager : MonoBehaviour
{

    public Tilemap _tilemap;
    public Dictionary<Vector3Int, BaseTileData> tiles;
    public static TilesDataManager Instance { get; private set; }


    private const string PLAINS = "PlainsTile";
    private const string WATER = "WaterRuleTile";

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
        InitTiles();
    }

    private void InitTiles()
    {
        tiles = new Dictionary<Vector3Int, BaseTileData>();
        foreach(Vector3Int pos in _tilemap.cellBounds.allPositionsWithin)
        {
            TileBase tile = _tilemap.GetTile(pos);
            if(tile != null)
            {
                BaseTileData newTileData;
                if (tile.name.Equals(PLAINS))
                {
                    newTileData = new PlainsTile();
                } else if (tile.name.Equals(WATER))
                {
                    newTileData = new WaterTile();
                }
                else
                {
                    newTileData = new DefaultTile();
                }
                newTileData.gridPosition = pos;
                newTileData.originTile = tile;
                newTileData.worldPosition = _tilemap.CellToWorld(pos) + _tileOffset;
                newTileData.Init();
                tiles.Add(pos, newTileData);
            }
        }
    }

    public bool HasTile(Vector3Int pos)
    {
        return _tilemap.HasTile(pos);
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
        if (_tilemap.cellBounds.Contains(pos))
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
        Vector3Int tilePos = _tilemap.WorldToCell(pos);
        Debug.Log("found pos " + tilePos);
        return GetTileDataAtPos(tilePos);
    }
    #endregion
}

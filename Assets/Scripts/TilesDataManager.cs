﻿using System.Collections;
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

    [SerializeField] private Factory _factoryTemplate;
    [SerializeField] private Sawmill _sawmillTemplate;
#pragma warning restore 0649
    #endregion

    public Dictionary<Vector3Int, BaseTileData> tiles;
    public static TilesDataManager Instance { get; private set; }
    public static bool AreTileLoaded { get; private set; }

    private const string PLAINS = "PlainsTile";
    private const string WATER = "WaterRuleTile";
    private const string WOODS = "WoodsTile";
    private const string FACTORY = "FactoryTile";
    private const string SAWMILL = "SawmillTile";

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
                newTileData.terrainTile.Init();
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
                    data.structureTile = InitStructureFromTileBase(tile, data);
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

    public StructureTile InitStructureFromTileBase(TileBase tile, BaseTileData data)
    {
        if (tile.name.Equals(FACTORY))
        {
            Factory factoryObject = Instantiate(_factoryTemplate, data.worldPosition, Quaternion.identity, transform);
            FactoryTile newFactory = new FactoryTile();
            newFactory.factory = factoryObject;
            return newFactory;
        }

        if (tile.name.Equals(SAWMILL))
        {
            Sawmill sawmillObject = Instantiate(_sawmillTemplate, data.worldPosition, Quaternion.identity, transform);
            SawmillTile newSawmill = new SawmillTile();
            newSawmill.sawmill = sawmillObject;
            return newSawmill;
        }
        Debug.LogWarning("Structure not found");
        return null;
    }
    #endregion
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

[System.Serializable]
public class BuildingBinding
{
    public StructureType type;
    public Building building;
    public StructureData data;
}

public class TilesDataManager : MonoBehaviour
{
    #region Editor objects
#pragma warning disable 0649
    [SerializeField] private Tilemap _terrainTilemap;
    [SerializeField] private Tilemap _structuresTilemap;
    [SerializeField] private List<BuildingBinding> _templates;
    private Dictionary<StructureType, Building> _templatesDico;

    [SerializeField] private TileBase _waterTile;
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
        _templatesDico = new Dictionary<StructureType, Building>();
        foreach(BuildingBinding bind in _templates)
        {
            _templatesDico.Add(bind.type, bind.building);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InitTerrainTiles();
        InitStructuresTiles();
        WaterClusterManager.Instance.CreateAllClusters(GetTilesWithTerrainType(TerrainType.Water));
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
                Debug.LogWarning("Structure built on empty tile at position " + pos.ToString());
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
            ResourcesManager.Instance.Pay(CostForStructure(type));
        }
    }

    public bool CanBuildStructureAtPos(StructureType type, Vector3Int pos)
    {
        BaseTileData data = GetTileDataAtPos(pos);
        bool canBuild = true;
        canBuild = canBuild && !data.terrainTile.terrainType.Equals(TerrainType.Water);
        canBuild = canBuild && data.structureTile == null;
        canBuild = canBuild && ResourcesManager.Instance.CanPay(CostForStructure(type));
        return canBuild;
    }

    public void RemoveStructureAtPos(Vector3Int pos, bool repay=true)
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
            Destroy(structure.building.gameObject);
            data.structureTile = null;
        }
    }

    #region Get Tiles
    public BaseTileData GetTileDataAtPos(Vector3Int pos)
    {
        BaseTileData data = tiles.FirstOrDefault(x => x.Value.gridPosition == pos).Value;
        return data;
    }

    public IEnumerable<BaseTileData> GetTilesInBounds(BoundsInt bounds)
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
        return res;
    }

    public IEnumerable<BaseTileData> GetTilesAtPos(IEnumerable<Vector3Int> positions)
    {
        List<BaseTileData> res = new List<BaseTileData>();
        foreach(Vector3Int pos in positions)
        {
            BaseTileData tile = GetTileDataAtPos(pos);
            if(tile != null)
            {
                res.Add(tile);
            }
        }
        return res.ToArray();

    }

    public IEnumerable<BaseTileData> GetTilesAroundTileAtPos(Vector3Int pos)
    {
        if (_terrainTilemap.cellBounds.Contains(pos))
        {
            BoundsInt localBounds = new BoundsInt(pos.x - 1, pos.y - 1, pos.z, 3, 3, 1);

            return GetTilesInBounds(localBounds);
        }
        return null;
    }

    public IEnumerable<BaseTileData> GetTilesDirectlyAroundTileAtPos(Vector3Int pos)
    {
        if(_terrainTilemap.cellBounds.Contains(pos))
        {
            return GetTilesAtPos(GetDirectNeighboursPositions(pos));
        }
        return null;
    }

    public IEnumerable<BaseTileData> GetTilesAroundTile(BaseTileData tile)
    {
        return GetTilesAroundTileAtPos(tile.gridPosition);
    }

    public IEnumerable<BaseTileData> GetTilesDirectlyAroundTile(BaseTileData tile)
    {
        return GetTilesDirectlyAroundTileAtPos(tile.gridPosition);
    }

    public BaseTileData GetTileAtWorldPos(Vector3 pos)
    {
        Vector3Int tilePos = _terrainTilemap.WorldToCell(pos);
        return GetTileDataAtPos(tilePos);
    }

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

    public IEnumerable<BaseTileData> GetTilesWithTerrainType(TerrainType type)
    {
        return tiles.Where(x => x.Value.terrainTile.terrainType == type).Select(x => x.Value);
    }
    #endregion

    #region Bindings

    public BaseTileData GetTileDataFromTileBase(TileBase tile) 
    {
        BaseTileData data = new BaseTileData
        {
            terrainTile = CreateTerrainTileFromTileBase(tile)
        };
        return data;
    }

    public TerrainTile CreateTerrainTileFromTileBase(TileBase tile)
    {
        if (tile.name.Equals(PLAINS))
        {
            return new PlainsTile();
        }
        if (tile.Equals(_waterTile))
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
        StructureTile newTile = null;
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

            case StructureType.None:
                break;
        }

        _templatesDico.TryGetValue(type, out Building toInstantiate);
        if (toInstantiate != null)
        {
            Building building = Instantiate(toInstantiate, data.worldPosition, Quaternion.identity, transform);
            building.dataTile = newTile;
            newTile.building = building;
            ResourcesManager.Instance.RegisterStructure(newTile);
        }
        return newTile;
    }

    public Sprite GetSpriteForStructure(StructureType type)
    {
        _templatesDico.TryGetValue(type, out Building building);
        if(building != null)
        {
            return building.GetComponent<SpriteRenderer>().sprite;
        }
        return null;
    }

    public StructureData GetDataForStructure(StructureType type)
    {
        BuildingBinding element = _templates.First(x => x.type == type);
        return element?.data;
    }


    public IEnumerable<BuildingBinding> GetAllConstructiblesStructures()
    {
        return _templates.Where(x => x.data.isConstructible);
    }

    public void ChangeTileTerrain(Vector3Int position, TerrainType type)
    {
        BaseTileData data = GetTileDataAtPos(position);
        TileBase newTilebase = null;

        if (type.Equals(TerrainType.Water))
        {
            newTilebase = _waterTile;
        }

        _terrainTilemap.SetTile(position, newTilebase);
        data.terrainTile = CreateTerrainTileFromTileBase(newTilebase);
    }

    public void DebugChangeToWater()
    {
        if(MouseManager.Instance.SelectedTile != null)
        {
            ChangeTileTerrain(MouseManager.Instance.SelectedTile.gridPosition, TerrainType.Water);
        }
    }

    public List<Cost> CostForStructure(StructureType type)
    {
        return _templates.First(x => x.type == type).data.costs;
    }

}

using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// Dependecies to other managers:
//   Hard dependencies:
//     TilesDataManager
//     CommandManager
//   Soft dependencies:
//     TurnManager

public class WaterClusterManager : Manager
{
    public static WaterClusterManager Instance { get; private set; }
    public static bool AreClustersCreated;
    public List<WaterCluster> clusters;

    public static int floodThreshold = 30;

    private int _nextClusterId = 0;
    private int _tilesChecked; // Used to prevent infinite loop in case of error

    private FloodMapCommand _latestFloodCommand;
    private Dictionary<WaterCluster, Stack<BaseTileData>> _possibleFloodTiles; // the list of floodable tiles for this turn (ignoring structures)
    private List<BaseTileData> _pumpingStationsTiles; // the list of 

    #region Events
    public delegate void ClustersCreated();
    public static ClustersCreated OnClustersCreated;

    public delegate void FloodDone();
    public static FloodDone OnFloodDone;
    #endregion


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        _pumpingStationsTiles = new List<BaseTileData>();
        TilesDataManager.OnTilesLoaded += Init;
        TurnManager.OnTurnPredict += PredictFlooding;
    }

    private void Init()
    {
        ClearPossibleFloodTiles();
        _possibleFloodTiles = new Dictionary<WaterCluster, Stack<BaseTileData>>();
        RecreateClusters();
    }

    private void OnDestroy()
    {
        TilesDataManager.OnTilesLoaded -= Init;
        TurnManager.OnTurnPredict -= PredictFlooding;
    }

    private void PredictFlooding()
    {
        _latestFloodCommand = null;
        // First pass on the flooding, we just compute the possible flooded tiles
        PrepareFloodedTilesCommand prepareFloodedTilesCommand = new PrepareFloodedTilesCommand();
        CommandManager.Instance.ExecuteCommand(prepareFloodedTilesCommand);

        // Second pass, we compute flooded tiles, counting the structures
        RecomputeFlooding();
    }

    // Computes the flooding
    // Does not generate the floodable tiles, it has already been done at the start of the turn with PredictFlooding
    public void RecomputeFlooding()
    {
        if(_latestFloodCommand != null)
        {
            _latestFloodCommand.Undo();
        }
        FloodMapCommand floodMapCommand = new FloodMapCommand();
        _latestFloodCommand = floodMapCommand;
        CommandManager.Instance.ExecuteCommand(floodMapCommand);
        OnFloodDone?.Invoke();
    }

    public void RecreateClusters()
    {
        AreClustersCreated = false;
        CreateClustersCommand createClustersCommand = new CreateClustersCommand
        {
            isUndoable = false
        };
        CommandManager.Instance.ExecuteCommand(createClustersCommand);
        AreClustersCreated = true;
        OnClustersCreated?.Invoke();
    }

    public void CreateClusters(IEnumerable<BaseTileData> waterTiles)
    { 
        if(clusters != null)
        {
            clusters.Clear();
        }
        clusters = new List<WaterCluster>();

        _tilesChecked = 0;
        _nextClusterId = 0;

        // Reset cluster flags, as they are our reference for the algorithm
        foreach (BaseTileData tile in waterTiles)
        {
            ((WaterTile)tile.terrainTile).clusterFlag = false;
        }

        foreach(BaseTileData tile in waterTiles)
        {
            if (!((WaterTile)tile.terrainTile).clusterFlag)
            {
                _nextClusterId++;
                WaterCluster cluster = new WaterCluster(_nextClusterId);
                FlagTileAndPropagate(tile, cluster);
                cluster.RecountFloodLevel();
                clusters.Add(cluster);
            }
        }
    }

    private void FlagTileAndPropagate(BaseTileData tile, WaterCluster cluster)
    {
        // Basic security
        _tilesChecked++;
        if (_tilesChecked >= 1000)
        {
            throw new AlgorithmTakesTooLongException();
        }

        // Flag current tile and add it to the cluster
        ((WaterTile)tile.terrainTile).clusterFlag = true;
        cluster.AddTile(tile);

        // Check neighbours to flag
        foreach(BaseTileData neighbourTile in GetDirectWaterNeighbours(tile, true))
        {
            if(!((WaterTile)neighbourTile.terrainTile).clusterFlag)
            {
                FlagTileAndPropagate(neighbourTile, cluster);
            }
        }
    }

    public IEnumerable<BaseTileData> GetDirectWaterNeighbours(BaseTileData refTile, bool predict = false)
    {
        IEnumerable<Vector3Int> neighboursPositions = GridUtils.GetDirectNeighboursPositions(refTile.GridPosition);
        return TilesDataManager.Instance.GetTilesWithTerrainType(TerrainType.Water, predict).Where(x => neighboursPositions.Contains(x.GridPosition));
    }

    public BaseTileData RegisterRandomFloodableTile(WaterCluster cluster)
    {
        BaseTileData tile = GetRandomFloodableTile(cluster);
        AddPossibleFloodTile(cluster, tile);
        return tile;
    }

    public BaseTileData GetRandomFloodableTile(WaterCluster cluster)
    {
        IEnumerable<BaseTileData> floodableTiles = GetFloodableTiles(cluster);
        if (floodableTiles.Any())
        {
            int selectedIndex = Alea.GetInt(0, floodableTiles.Count());
            return floodableTiles.ElementAt(selectedIndex);
        }
        return null;
    }

    public IEnumerable<BaseTileData> GetFloodableTiles(WaterCluster cluster)
    {
        List<BaseTileData> possibleNeighbours = new List<BaseTileData>();
        foreach(BaseTileData tile in cluster.tiles)
        {
            IEnumerable<BaseTileData> currentNeighbours = TilesDataManager.Instance.GetTilesDirectlyAroundTile(tile, true);
            foreach(BaseTileData neighbour in currentNeighbours)
            {
                if(!possibleNeighbours.Contains(neighbour) && neighbour.terrainTile.terrainData.canBeFlooded)
                {
                    possibleNeighbours.Add(neighbour);
                }
            }
        }
        return possibleNeighbours;
    }

    public WaterCluster GetClusterForTile(BaseTileData tile)
    {
        WaterCluster res = null;
        foreach(WaterCluster cluster in clusters)
        {
            if(cluster.tiles.Contains(tile))
            {
                res = cluster;
            }
        }
        return res;
    }

    #region Manage possible flood tiles
    public void ClearPossibleFloodTiles()
    {
        if (_possibleFloodTiles != null)
        {
            foreach (Stack<BaseTileData> associatedTiles in _possibleFloodTiles.Values)
            {
                associatedTiles.Clear();
            }
            _possibleFloodTiles.Clear();
        }
    }

    public void AddPossibleFloodTile(WaterCluster cluster, BaseTileData tile)
    {
        if(_possibleFloodTiles.ContainsKey(cluster))
        {
            _possibleFloodTiles.TryGetValue(cluster, out Stack<BaseTileData> associatedTiles);
            associatedTiles.Push(tile);
        }
        else
        {
            Stack<BaseTileData> associatedTiles = new Stack<BaseTileData>();
            associatedTiles.Push(tile);
            _possibleFloodTiles.Add(cluster, associatedTiles);
        }
    }

    public BaseTileData UsePossibleFloodTile(WaterCluster cluster)
    {
        if(!_possibleFloodTiles.ContainsKey(cluster))
        {
            throw new NoFloodableTileForClusterException();
        }
        _possibleFloodTiles.TryGetValue(cluster, out Stack<BaseTileData> associatedTiles);
        return associatedTiles.Pop();
    }

    #endregion

    #region Manage Pumping Stations

    public void RegisterPumpingStation(BaseTileData tile)
    {
        if (!_pumpingStationsTiles.Contains(tile))
        {
            _pumpingStationsTiles.Add(tile);
        }
    }

    public void UnregisterPumpingStation(BaseTileData tile)
    {
        _pumpingStationsTiles.Remove(tile);
    }

    public List<BaseTileData> GetPumpedTiles()
    {
        List<BaseTileData> pumpedTiles = new List<BaseTileData>();
        IEnumerable<BaseTileData> activeStations = _pumpingStationsTiles.Where(x => x.IsStructureOn());
        foreach(BaseTileData tile in activeStations)
        {
            IEnumerable<BaseTileData> pumpedNeighbours = TilesDataManager.Instance.GetTilesDirectlyAroundTile(tile).Where(x => x.terrainTile is WaterTile);
            if(pumpedNeighbours != null)
            {
                pumpedTiles.AddRange(pumpedNeighbours);
            }
        }
        return pumpedTiles;
    }

    public List<BaseTileData> GetPumpedTilesForCluster(WaterCluster cluster)
    {
        List<BaseTileData> pumpedTiles = GetPumpedTiles();
        List<BaseTileData> clusterPumpedTiles = new List<BaseTileData>();
        foreach(BaseTileData tile in pumpedTiles)
        {
            if(cluster.ContainsTile(tile))
            {
                clusterPumpedTiles.Add(tile);
            }
        }
        return clusterPumpedTiles;
    }
    #endregion
}

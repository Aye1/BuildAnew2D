using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WaterClusterManager : MonoBehaviour
{
    public static WaterClusterManager Instance { get; private set; }
    public static bool AreClustersCreated;
    public List<WaterCluster> clusters;

    public static int floodThreshold = 30;

    private int _nextClusterId = 0;
    private int _tilesChecked; // Used to prevent infinite loop in case of error

    public delegate void ClustersCreated();
    public static ClustersCreated OnClustersCreated;

    public delegate void FloodDone();
    public static FloodDone OnFloodDone;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (TilesDataManager.AreTileLoaded)
        {
            InitClusters();
        }
        else
        {
            TilesDataManager.OnTilesLoaded += InitClusters;
        }
        TurnManager.OnTurnPredict += PredictFlooding;
    }

    private void PredictFlooding()
    {
        FloodMapCommand floodMapCommand = new FloodMapCommand();
        CommandManager.Instance.ExecuteCommand(floodMapCommand); OnFloodDone?.Invoke();
    }

    public void InitClusters()
    {
        RecreateClusters();
        TilesDataManager.OnTilesLoaded -= InitClusters;
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
        IEnumerable<Vector3Int> neighboursPositions = TilesDataManager.Instance.GetDirectNeighboursPositions(refTile.gridPosition);
        return TilesDataManager.Instance.GetTilesWithTerrainType(TerrainType.Water, predict).Where(x => neighboursPositions.Contains(x.gridPosition));
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
}

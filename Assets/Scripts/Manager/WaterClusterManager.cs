using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WaterClusterManager : MonoBehaviour
{
    public static WaterClusterManager Instance { get; private set; }
    public List<WaterCluster> clusters;

    public int floodThreshold = 50;

    private int _nextClusterId = 0;
    private int _tilesChecked; // Used to prevent infinite loop in case of error

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
            InitAllClusters();
        }
        else
        {
            TilesDataManager.OnTilesLoaded += InitAllClusters;
        }
        TurnManager.OnTurnPredict += PredictFlooding;
    }

    private void PredictFlooding()
    {
        Debug.Log("Predicting flooding");
        CheckFlooding();
        OnFloodDone?.Invoke();
    }

    public void CheckFlooding()
    {
        // Don't change the collection while enumerating
        List<WaterCluster> clustersToFlood = new List<WaterCluster>();
        foreach(WaterCluster cluster in clusters)
        {
            cluster.RecountFloodLevel();
            if(cluster.FloodLevel >= floodThreshold)
            {
                clustersToFlood.Add(cluster);
            }
        }
        clustersToFlood.ForEach(FloodAndBalance);
        RecreateClusters(TilesDataManager.Instance.GetTilesWithTerrainType(TerrainType.Water, true));
        clusters.ForEach(x => x.BalanceFlood(true));
    }

    public void FloodAndBalance(WaterCluster cluster)
    {
        int neighboursToFlood = cluster.FloodLevel / floodThreshold;
        for(int i=0; i<neighboursToFlood; i++)
        {
            FloodNeighbour(cluster);
        }
        cluster.RemoveFlood(neighboursToFlood * floodThreshold);
    }

    public void FloodNeighbour(WaterCluster cluster)
    {
        Debug.Log("Flood");
        IEnumerable<BaseTileData> floodableTiles = GetFloodableTiles(cluster);
        if (floodableTiles.Any())
        {
            int selectedIndex = Alea.GetInt(0, floodableTiles.Count());
            BaseTileData tileToFlood = floodableTiles.ElementAt(selectedIndex);
            BaseTileData tileFlooded = TilesDataManager.Instance.CreateNewTileForNextTurn(tileToFlood, TerrainType.Water);
            cluster.AddTile(tileFlooded);
        }
    }

    public void InitAllClusters()
    {
        IEnumerable<BaseTileData> waterTiles = TilesDataManager.Instance.GetTilesWithTerrainType(TerrainType.Water, false);
        CreateClusters(waterTiles);
    }

    public void CreateClusters(IEnumerable<BaseTileData> waterTiles)
    { 
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
        Debug.Log("Water clusters parsed: " + clusters.Count);
    }

    public void RecreateClusters(IEnumerable<BaseTileData> waterTiles)
    {
        Debug.Log("Recreate clusters");
        // WARNING: possible memory leak
        clusters.Clear();
        CreateClusters(waterTiles);
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

    private IEnumerable<BaseTileData> GetDirectWaterNeighbours(BaseTileData refTile, bool predict = false)
    {
        IEnumerable<Vector3Int> neighboursPositions = TilesDataManager.Instance.GetDirectNeighboursPositions(refTile.gridPosition);
        return TilesDataManager.Instance.GetTilesWithTerrainType(TerrainType.Water, predict).Where(x => neighboursPositions.Contains(x.gridPosition));
    }

    private IEnumerable<BaseTileData> GetFloodableTiles(WaterCluster cluster)
    {
        List<BaseTileData> possibleNeighbours = new List<BaseTileData>();
        foreach(BaseTileData tile in cluster.tiles)
        {
            IEnumerable<BaseTileData> currentNeighbours = TilesDataManager.Instance.GetTilesDirectlyAroundTile(tile, true);
            foreach(BaseTileData neighbour in currentNeighbours)
            {
                if(!possibleNeighbours.Contains(neighbour) && !(neighbour.terrainTile is WaterTile))
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

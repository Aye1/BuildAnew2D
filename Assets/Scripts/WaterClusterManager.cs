using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class WaterClusterManager : MonoBehaviour
{
    public static WaterClusterManager Instance { get; private set; }
    public List<WaterCluster> clusters;

    private int _nextClusterId = 0;
    private int _tilesChecked;
    private IEnumerable<BaseTileData> _tiles;

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

    public void CreateAllClusters(IEnumerable<BaseTileData> waterTiles)
    {
        clusters = new List<WaterCluster>();
        _tiles = waterTiles;
        _tilesChecked = 0;
        _nextClusterId = 0;

        // Reset clusterIds, as they are our reference for the all algorithm
        foreach (BaseTileData tile in waterTiles)
        {
            ((WaterTile)tile.terrainTile).clusterId = 0;
        }

        foreach(BaseTileData tile in waterTiles)
        {
            if (((WaterTile)tile.terrainTile).clusterId == 0)
            {
                _nextClusterId++;
                WaterCluster cluster = new WaterCluster(_nextClusterId);
                FlagTileAndPropagate(tile, cluster);
                clusters.Add(cluster);
            }
        }
        Debug.Log("Water clusters parsed");
    }

    public void RecreateAllClusters(IEnumerable<BaseTileData> waterTiles)
    {
        clusters.Clear(); // WARNING: possible memory leak
        CreateAllClusters(waterTiles);
    }

    private void FlagTileAndPropagate(BaseTileData tile, WaterCluster cluster)
    {
        _tilesChecked++;

        if (_tilesChecked >= 100)
        {
            // Basic security
            return;
        }
        ((WaterTile)tile.terrainTile).clusterId = cluster.id;
        cluster.AddTile(tile.terrainTile as WaterTile);
        foreach(BaseTileData neighbourTile in GetDirectNeighbours(tile))
        {
            if(((WaterTile)neighbourTile.terrainTile).clusterId == 0)
            {
                FlagTileAndPropagate(neighbourTile, cluster);
            }
        }
    }

    private IEnumerable<BaseTileData> GetDirectNeighbours(BaseTileData refTile)
    {
        IEnumerable<Vector3Int> neighboursPositions = GetDirectNeighboursPositions(refTile.gridPosition);
        return _tiles.Where(x => neighboursPositions.Contains(x.gridPosition));
    }

    private IEnumerable<Vector3Int> GetDirectNeighboursPositions(Vector3Int position)
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
}

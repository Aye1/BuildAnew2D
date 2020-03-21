﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System;

public class WaterClusterManager : MonoBehaviour
{
    public static WaterClusterManager Instance { get; private set; }
    public List<WaterCluster> clusters;
    public int floodThreshold = 30;

    private int _nextClusterId = 0;
    private int _tilesChecked;
    private IEnumerable<BaseTileData> _tiles;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            TurnManager.OnTurnStart += CheckFlooding;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CheckFlooding()
    {
        // Don't change the collection while enumerating
        List<WaterCluster> clustersToFlood = new List<WaterCluster>();
        foreach(WaterCluster cluster in clusters)
        {
            cluster.UpdateFloodAmount();
            if(cluster.FloodLevel >= floodThreshold)
            {
                clustersToFlood.Add(cluster);
            }
        }
        clustersToFlood.ForEach(FloodNeighbour);
    }

    public void FloodNeighbour(WaterCluster cluster)
    {
        IEnumerable<BaseTileData> floodableTiles = GetFloodableTiles(cluster);
        int selectedIndex = Alea.GetInt(0, floodableTiles.Count());
        BaseTileData tileToFlood = floodableTiles.ElementAt(selectedIndex);
        TilesDataManager.Instance.ChangeTileTerrain(tileToFlood.gridPosition, TerrainType.Water);
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
        cluster.AddTile(tile);
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
        IEnumerable<Vector3Int> neighboursPositions = TilesDataManager.Instance.GetDirectNeighboursPositions(refTile.gridPosition);
        return _tiles.Where(x => neighboursPositions.Contains(x.gridPosition));
    }



    private IEnumerable<BaseTileData> GetFloodableTiles(WaterCluster cluster)
    {
        List<BaseTileData> possibleNeighbours = new List<BaseTileData>();
        foreach(BaseTileData tile in cluster.tiles)
        {
            IEnumerable<BaseTileData> currentNeighbours = TilesDataManager.Instance.GetTilesDirectlyAroundTile(tile);
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
}

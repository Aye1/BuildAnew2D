﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/LevelData", order = 1)]
public class LevelData : ScriptableObject
{
    #region Editor objects
#pragma warning disable 0649

    [Header("UI Info")]
    [SerializeField] private string _levelName;

    [Header("Conditions")]
    [SerializeField] private List<BaseCondition> _successConditions;
    [SerializeField] private List<BaseCondition> _defeatConditions;
    [Header("Resources")]
    [SerializeField] private List<Cost> _initialResources;
    [Header("Tutorial")]
    public TutorialData tutorialData;
    [Header("TileMap")]
    public string terrainTileMapPath;
    public string structureTileMapPath;
    private Tilemap _terrainTilemap;
    private Tilemap _structuresTilemap;

    
#pragma warning restore 0649
    #endregion

    public List<BaseCondition> GetSuccessConditions()
    {
        return _successConditions;
    }
    public List<BaseCondition> GetDefeatConditions()
    {
        return _defeatConditions;
    }
    public List<Cost> GetInitialResources()
    {
        return _initialResources;
    }
    public Tilemap GetTerrainTilemap()
    {
        if(_terrainTilemap == null)
        {
            string path = terrainTileMapPath.Remove(0, terrainTileMapPath.IndexOf("Resources") + 10); //+10 bc resources = 9 chars + 1 for '/'
            path = path.Remove(path.IndexOf(".")); //Removes the extension
            _terrainTilemap = Resources.Load<Tilemap>(path);
        }
        return _terrainTilemap;
    }
    public Tilemap GetStructureTilemap()
    {
        if (_structuresTilemap == null)
        {
            string path = structureTileMapPath.Remove(0, structureTileMapPath.IndexOf("Resources") + 10); //+10 bc resources = 9 chars + 1 for '/'
            path = path.Remove(path.IndexOf(".")); //Removes the extension
            _structuresTilemap = Resources.Load<Tilemap>(path);
        }
        return _structuresTilemap;
    }

    public string GetLevelName()
    {
        return _levelName;
    }
}

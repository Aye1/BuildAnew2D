﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Dependecies to other managers:
// LevelManager
// BuildingManager
// TilesDataManager

public class MouseManager : Manager
{

#pragma warning disable 0649
    [Header("Editor bindings")]
    [SerializeField] private SpriteRenderer selectedTileSprite;
    [SerializeField] private SpriteRenderer hoveredTileSprite;
    [SerializeField] private SpriteRenderer phantomBuildingSprite;
#pragma warning restore 0649

    public static MouseManager Instance { get; private set; }
    public BaseTileData SelectedTile { get; private set; }
    public BaseTileData HoveredTile { get; private set; }

    private Vector3 _awayPos = new Vector3(1000, 1000, 1000);
    private Color _transpColor = new Color(1.0f, 1.0f, 1.0f, 0.8f);
    private Color _invisibleColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    private Color _transpRedColor = new Color(0.8f, 0.0f, 0.0f, 0.8f);

    #region Events
    public delegate void PlayerClick();
    public static event PlayerClick OnPlayerClick;
    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        phantomBuildingSprite.color = _transpColor;
        LevelManager.OnLevelNeedReset += Reset;
    }

    private void Reset()
    {
        SelectedTile = null;
        HoveredTile = null;
    }

    void Update()
    {
        ManageHover();
        ManageClick();
        ManageSelectedTile();
    }

    private void ManageClick()
    {
        // The second part of the condition prevents from interacting with the tilemap if we click on UI
        if (Input.GetMouseButtonDown(0) && EventSystem.current.currentSelectedGameObject == null && !BlockingUI.IsBlocked)
        {
            BaseTileData tileClicked = GetTileAtMousePos();

            if (tileClicked != null)
            {
                if (BuildingManager.Instance.IsInBuildMode)
                {
                    BuildingManager.Instance.BuildCurrentStructure(HoveredTile);
                }
                else
                {
                    tileClicked.terrainTile.DebugOnClick();
                }
                if(SelectedTile != null)
                {
                    SelectedTile.SetIsSelected(false);
                }
                SelectedTile = tileClicked;
                SelectedTile.SetIsSelected(true);
            }
            else
            {
                if (SelectedTile != null)
                {
                    SelectedTile.SetIsSelected(false);
                }
                SelectedTile = null;
            }
            OnPlayerClick?.Invoke();
        }
    }

    private void ManageSelectedTile()
    {
        if (SelectedTile != null)
        {
            selectedTileSprite.transform.position = SelectedTile.worldPosition;
        }
        else
        {
            selectedTileSprite.transform.position = _awayPos;
        }
    }

    private void ManageHover()
    {
        if (!BlockingUI.IsBlocked)
        {
            BaseTileData tileHovered = GetTileAtMousePos();
            StructureType currentBuildType = BuildingManager.Instance.CurrentBuildingStructure;
            phantomBuildingSprite.sprite = TilesDataManager.Instance.GetSpriteForStructure(currentBuildType);
            if (tileHovered != null)
            {
                HoveredTile = tileHovered;
                hoveredTileSprite.transform.position = HoveredTile.worldPosition;
                if (BuildingManager.Instance.IsInBuildMode)
                {
                    bool canBuild = BuildingManager.Instance.CanBuildStructureAtPos(currentBuildType, tileHovered.GridPosition);
                    phantomBuildingSprite.transform.position = HoveredTile.worldPosition;
                    phantomBuildingSprite.color = canBuild ? _transpColor : _transpRedColor;
                }
                else
                {
                    phantomBuildingSprite.color = _invisibleColor;
                }
            }
            else
            {
                HoveredTile = null;
                hoveredTileSprite.transform.position = _awayPos;
                phantomBuildingSprite.color = _invisibleColor;
            }
        }
    }

    private BaseTileData GetTileAtMousePos()
    {
        BaseTileData tile = TilesDataManager.Instance.GetTileAtWorldPos(GetMouseWorldPos());
        return tile;
    }

    public Vector3 GetMouseScreenPos()
    {
        Vector3 pos = Input.mousePosition;
        pos.z = 0;
        return pos;
    }
    public Vector3 GetMouseWorldPos()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        return pos;
    }

    private void OnDestroy()
    {
        LevelManager.OnLevelNeedReset -= Reset;
    }
}
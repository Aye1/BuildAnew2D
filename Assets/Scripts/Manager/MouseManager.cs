using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseManager : MonoBehaviour
{

    public static MouseManager Instance { get; private set; }
    public SpriteRenderer selectedTileSprite;
    public SpriteRenderer hoveredTileSprite;
    public SpriteRenderer phantomBuildingSprite;
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
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
        phantomBuildingSprite.color = _transpColor;
    }

    public void Start()
    {
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
                    BuildingManager.Instance.BuildCurrentStructure();
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
                    bool canBuild = TilesDataManager.Instance.CanBuildStructureAtPos(currentBuildType, tileHovered.GetGridPosition());
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
}
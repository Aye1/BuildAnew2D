using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseManager : MonoBehaviour
{

    public static MouseManager Instance { get; private set; }
    public SpriteRenderer selectedTileSprite;
    public SpriteRenderer hoveredTileSprite;
    public BaseTileData SelectedTile { get; private set; }
    public BaseTileData HoveredTile { get; private set; }

    private Vector3 _awayPos = new Vector3(1000, 1000, 1000);

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
        if (Input.GetMouseButtonDown(0) && EventSystem.current.currentSelectedGameObject == null)
        {
            BaseTileData tileClicked = GetTileAtMousePos();
            if (tileClicked != null)
            {
                tileClicked.terrainTile.DebugOnClick();
                SelectedTile = tileClicked;
            }
            else
            {
                SelectedTile = null;
            }
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
        BaseTileData tileHovered = GetTileAtMousePos();
        if (tileHovered != null)
        {
            HoveredTile = tileHovered;
            hoveredTileSprite.transform.position = HoveredTile.worldPosition;
        }
        else
        {
            HoveredTile = null;
            hoveredTileSprite.transform.position = _awayPos;
        }
    }

    private BaseTileData GetTileAtMousePos()
    {
        BaseTileData tile = TilesDataManager.Instance.GetTileAtWorldPos(GetMouseWorldPos());
        return tile;
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        return pos;
    }
}
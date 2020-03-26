using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using System;

public class DebugTextManager : MonoBehaviour
{
    private Tilemap tilemap;
    public TextMeshProUGUI templateText;

    private Dictionary<Vector3Int, TextMeshProUGUI> _debugTextsDico;
    private TilesDataManager _tileDataManager;

    [SerializeField] private bool _debugIsVisible = true;


    private bool _isVisible = true;
    public bool IsVisible
    {
        get { return _isVisible; }
        set
        {
            if (value != _isVisible)
            {
                _isVisible = value;
                UpdateTilesDebugText();
            }
        }
    }

    void Start()
    {
        _debugTextsDico = new Dictionary<Vector3Int, TextMeshProUGUI>();
        _tileDataManager = TilesDataManager.Instance;
        GameManager.OnLevelLoaded += OnLevelLoaded;
    }
    private void OnLevelLoaded()
    {
        tilemap = GameManager.Instance.GetLevelData().GetTerrainTilemap();

        if (TilesDataManager.AreTileLoaded)
        {
            // Tiles already loaded, we can init debug texts
            InitTilesDebugText();
        }
        else
        {
            // Wait for the tiles loading finished to init debug texts
            TilesDataManager.OnTilesLoaded += InitTilesDebugText;
        }
    }

    private void Update()
    {
        IsVisible = _debugIsVisible;
    }

    private void OnDestroy()
    {
        TurnManager.OnTurnStart -= UpdateTilesDebugText;
        ActiveTile.OnTileModified -= UpdateTilesDebugText;
    }

    private void InitTilesDebugText()
    {
        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            TextMeshProUGUI newText = Instantiate(templateText, Vector3.zero, Quaternion.identity, transform);
            _debugTextsDico.Add(pos, newText);

            BaseTileData data = _tileDataManager.GetTileDataAtPos(pos);
            if(data != null)
                newText.transform.position = data.worldPosition;
        }
        Destroy(templateText);
        TurnManager.OnTurnStart += UpdateTilesDebugText;
        ActiveTile.OnTileModified += UpdateTilesDebugText;
        WaterClusterManager.OnFloodDone += UpdateTilesDebugText;
        TilesDataManager.OnTilesLoaded -= InitTilesDebugText;
        UpdateTilesDebugText();
    }

    private void UpdateTilesDebugText()
    {
        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            _debugTextsDico.TryGetValue(pos, out TextMeshProUGUI text);
            BaseTileData data = _tileDataManager.GetTileDataAtPos(pos, true);
            text.text = _isVisible ? data.terrainTile.GetDebugText() : "";
        }
    }
}

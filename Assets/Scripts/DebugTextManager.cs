﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class DebugTextManager : MonoBehaviour
{
    public Tilemap tilemap;
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
        TilesDataManager.OnTilesLoaded += InitTilesDebugText;
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
        UpdateTilesDebugText();
    }

    private void UpdateTilesDebugText()
    {
        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            _debugTextsDico.TryGetValue(pos, out TextMeshProUGUI text);
            BaseTileData data = _tileDataManager.GetTileDataAtPos(pos);
            text.text = _isVisible ? data.terrainTile.GetDebugText() : "";
        }
    }
}

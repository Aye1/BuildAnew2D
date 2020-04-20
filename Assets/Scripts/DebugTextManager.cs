using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class DebugTextManager : MonoBehaviour
{
    private Tilemap _tilemap;
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
        _tileDataManager = TilesDataManager.Instance;
        TilesDataManager.OnTilesLoaded += InitTilesDebugText;
        if(TilesDataManager.AreTileLoaded)
        {
            InitTilesDebugText();
        }
    }
    
    private void Update()
    {
        IsVisible = _debugIsVisible;
    }

    private void OnDestroy()
    {
        UnregisterCallbacks();
    }

    private void InitTilesDebugText()
    {
        _tilemap = LevelManager.Instance.GetCurrentLevel().GetTerrainTilemap();
        _debugTextsDico = new Dictionary<Vector3Int, TextMeshProUGUI>();

        foreach (Vector3Int pos in _tilemap.cellBounds.allPositionsWithin)
        {
            TextMeshProUGUI newText = Instantiate(templateText, Vector3.zero, Quaternion.identity, transform);
            _debugTextsDico.Add(pos, newText);

            BaseTileData data = _tileDataManager.GetTileDataAtPos(pos);
            if(data != null)
                newText.transform.position = data.worldPosition;
        }
        Destroy(templateText);
        RegisterCallbacks();
        TilesDataManager.OnTilesLoaded -= InitTilesDebugText;
        UpdateTilesDebugText();
    }

    private void UpdateTilesDebugText()
    {
        foreach (Vector3Int pos in _tilemap.cellBounds.allPositionsWithin)
        {
            _debugTextsDico.TryGetValue(pos, out TextMeshProUGUI text);
            BaseTileData data = _tileDataManager.GetTileDataAtPos(pos, true);
            text.text = _isVisible ? data.terrainTile.GetDebugText() : "";
        }
    }

    private void UpdateTilesDebugText(Command cmd)
    {
        UpdateTilesDebugText();
    }

    private void RegisterCallbacks()
    {
        TurnManager.OnTurnStart += UpdateTilesDebugText;
        ActiveTile.OnTileModified += UpdateTilesDebugText;
        WaterClusterManager.OnFloodDone += UpdateTilesDebugText;
        TilesDataManager.OnTilemapModified += UpdateTilesDebugText;
        CommandManager.OnCommandDone += UpdateTilesDebugText;
        CommandManager.OnCommandUndone += UpdateTilesDebugText;
    }

    private void UnregisterCallbacks()
    {
        TurnManager.OnTurnStart -= UpdateTilesDebugText;
        ActiveTile.OnTileModified -= UpdateTilesDebugText;
        WaterClusterManager.OnFloodDone -= UpdateTilesDebugText;
        TilesDataManager.OnTilemapModified -= UpdateTilesDebugText;
        CommandManager.OnCommandDone -= UpdateTilesDebugText;
        CommandManager.OnCommandUndone -= UpdateTilesDebugText;
    }
}

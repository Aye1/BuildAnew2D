using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class DebugTextManager : MonoBehaviour
{
    public Tilemap tilemap;
    public Grid grid;
    public TextMeshProUGUI templateText;
    public bool isVisible = true;

    private Dictionary<Vector3Int, TextMeshProUGUI> _debugTextsDico;
    private TilesDataManager _tileDataManager;

    // Start is called before the first frame update
    void Start()
    {
        _debugTextsDico = new Dictionary<Vector3Int, TextMeshProUGUI>();
        _tileDataManager = TilesDataManager.Instance;
        InitTilesDebugText();
        TurnManager.OnTurnStart += UpdateTilesDebugText;
        BaseTileData.OnTileModified += UpdateTilesDebugText;
        UpdateTilesDebugText();
    }

    private void OnDestroy()
    {
        TurnManager.OnTurnStart -= UpdateTilesDebugText;
        BaseTileData.OnTileModified -= UpdateTilesDebugText;
    }

    private void InitTilesDebugText()
    {
        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            TextMeshProUGUI newText = Instantiate(templateText, Vector3.zero, Quaternion.identity, transform);
            _debugTextsDico.Add(pos, newText);
            
            // Debug position, not scaling
            newText.transform.localPosition = new Vector3((pos.x - pos.y) * 100 + 85, (pos.x + pos.y) * 50 + 55, pos.z);

        }
        Destroy(templateText);
    }

    private void UpdateTilesDebugText()
    {
        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            _debugTextsDico.TryGetValue(pos, out TextMeshProUGUI text);
            BaseTileData data = _tileDataManager.GetTileDataAtPos(pos);
            text.text = isVisible ? data.GetDebugText() : "";
        }
    }
}

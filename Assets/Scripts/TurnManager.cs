using UnityEngine;
using UnityEngine.Tilemaps;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    public Tilemap tilemap;

    private int _turnCounter;

    #region Events
    public delegate void TurnStart();
    public static event TurnStart OnTurnStart;
    #endregion

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

    public void NextTurn()
    {
        _turnCounter++;
        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            if(TilesDataManager.Instance.HasTile(pos))
            {
                BaseTileData tile = TilesDataManager.Instance.GetTileDataAtPos(pos);
                tile.OnTurnStarts(TilesDataManager.Instance.GetTilesAroundTile(tile));
            }
        }
        OnTurnStart?.Invoke();
    }
}

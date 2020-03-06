using UnityEngine;
using UnityEngine.Tilemaps;

public class TurnManager : MonoBehaviour
{
    #region Singleton
    private static TurnManager _instance;
    public static TurnManager Instance
    {
        get
        {
            return _instance;
        }
        set
        {
            if (_instance == null) 
            {
                _instance = value;
            }
        }
    }
    #endregion

    public Tilemap tilemap;

    private int _turnCounter;

    #region Events
    public delegate void TurnStart();
    public static event TurnStart OnTurnStart;
    #endregion

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
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

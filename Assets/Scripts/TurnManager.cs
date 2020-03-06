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
    private readonly Vector3Int _neighboursSize = new Vector3Int(3, 3, 0);

    public delegate void TurnStart();
    public static event TurnStart OnTurnStart;

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

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextTurn()
    {
        _turnCounter++;
        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            if(TilesDataManager.Instance.HasTile(pos))
            {
                BaseTileData tile = TilesDataManager.Instance.GetTileDataAtPos(pos);
                tile.OnTurnStarts(GetTilesAroundTile(tile));
            }
        }
        OnTurnStart?.Invoke();
    }

    public BaseTileData[] GetTilesAroundTileAtPos(Vector3Int pos)
    {
        if (tilemap.cellBounds.Contains(pos))
        {
            BoundsInt localBounds = new BoundsInt(pos.x - 1, pos.y - 1, pos.z, 3, 3, 1);

            return TilesDataManager.Instance.GetTilesInBounds(localBounds);
        }
        return null;
    }

    public BaseTileData[] GetTilesAroundTile(BaseTileData tile)
    {
        return GetTilesAroundTileAtPos(tile.gridPosition);
    }
}

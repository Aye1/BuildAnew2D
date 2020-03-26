using UnityEngine;
using UnityEngine.Tilemaps;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    private int _turnCounter;

    #region Events
    public delegate void TurnStart();
    public static event TurnStart OnTurnStart;
    public delegate void TurnPredict();
    public static event TurnPredict OnTurnPredict;
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

    public void Start()
    {
        if (TilesDataManager.AreTileLoaded)
        {
            PredictNextTurn();
        }
        else
        {
            TilesDataManager.OnTilesLoaded += PredictNextTurn;
        }
    }

    public void NextTurn()
    {
        _turnCounter++;
        foreach (Vector3Int pos in TilesDataManager.Instance.GetTilemapBounds().allPositionsWithin)
        {
            if(TilesDataManager.Instance.HasTile(pos))
            {
                BaseTileData tile = TilesDataManager.Instance.GetTileDataAtPos(pos);
                tile.OnTurnStarts();
            }
        }
        OnTurnStart?.Invoke();
        PredictNextTurn();
        OnTurnPredict?.Invoke();
    }

    private void PredictNextTurn()
    {
        foreach(Vector3Int pos in TilesDataManager.Instance.GetTilemapBounds().allPositionsWithin)
        {
            if(TilesDataManager.Instance.HasTile(pos))
            {
                BaseTileData tile = TilesDataManager.Instance.GetTileDataAtPos(pos);
                tile.PredictOnTurnStarts();
            }
        }
    }
}

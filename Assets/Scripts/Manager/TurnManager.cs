using UnityEngine;

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
        GameManager.OnGameReady += InitTurns;
    }

    private void Update()
    {
        CatchShortcuts();
    }

    private void InitTurns()
    {
        _turnCounter = 0;
        PredictNextTurn();
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
        OnTurnPredict?.Invoke();
    }

    private void CatchShortcuts()
    {
        if(InputManager.Instance.GetKeyDown("nextTurn") && !UIManager.Instance.IsBlocked)
        {
            NextTurn();
        }
    }
}

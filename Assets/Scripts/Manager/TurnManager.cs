using UnityEngine;

// Dependecies to other managers:
//   Hard dependencies: 
//     TilesDataManager
//     InputManager
//     UIManager

public class TurnManager : Manager
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
            InitState = InitializationState.Ready;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        CatchShortcuts();
    }

    public void InitTurns()
    {
        InitState = InitializationState.Updating;
        _turnCounter = 0;
        PredictNextTurn();
        InitState = InitializationState.Ready;
    }

    public void NextTurn()
    {
        InitState = InitializationState.Updating;
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
        InitState = InitializationState.Ready;
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

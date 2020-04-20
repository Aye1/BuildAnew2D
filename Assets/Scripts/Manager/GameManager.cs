using UnityEngine;
using System.Linq;

// Dependecies to other managers:
//   Hard dependencies:
//     LevelManager
//     ResourcesManager
//   Soft dependencies:
//     TurnManager
//     TilesDataManager

public enum GameState { Default, Running, Won, Failed };
public class GameManager : Manager
{
    public static GameManager Instance { get; private set; }
    public static bool IsGameReady;
    public static bool IsLevelLoaded;

    private bool _shouldRaiseGameReady;

    #region Events
    public delegate void LevelLoaded();
    public static event LevelLoaded OnLevelLoaded;

    public delegate void GameReady();
    public static event GameReady OnGameReady;

    public delegate void GameStateChanged(GameState newState);
    public static event GameStateChanged OnGameStateChanged;
    #endregion

    private GameState _state;
    public GameState State
    {
        get
        {
            return _state;
        }
        set
        {
            if(_state != value)
            {
                _state = value;
                OnGameStateChanged?.Invoke(_state);
            }
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitState = InitializationState.Initializing;
            SettingsLoader.LoadInitialSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        RegisterCallbacks();
        LoadLevel();
        CheckIfGameIsReady();
    }

    private void OnDestroy()
    {
        UnregisterCallbacks();
    }

    private void RegisterCallbacks()
    {
        LevelManager.OnLevelNeedReset += Reset;
        TurnManager.OnTurnStart += ComputeEndGameCondition;
        TilesDataManager.OnTilesLoaded += CheckIfGameIsReady;
    }

    private void UnregisterCallbacks()
    {
        TilesDataManager.OnTilesLoaded -= CheckIfGameIsReady;
        TurnManager.OnTurnStart -= ComputeEndGameCondition;
        LevelManager.OnLevelNeedReset -= Reset;
    }

    private void CheckIfGameIsReady()
    {
        bool gameReady = IsLevelLoaded;
        gameReady &= TilesDataManager.AreTileLoaded;
        if(gameReady && _shouldRaiseGameReady)
        {
            InitState = InitializationState.Ready;
            IsGameReady = true;
            OnGameReady?.Invoke();
            _shouldRaiseGameReady = false;
        }
    }

    public void LoadLevel()
    {
        _shouldRaiseGameReady = true;
        IsLevelLoaded = false;
        LevelData levelData = LevelManager.Instance.GetCurrentLevel();
        if (levelData != null)
        {
            ResourcesManager.Instance.InitializeResources(levelData.GetInitialResources());
        }
        else
        {
            Debug.LogWarning("Missing level data into LevelManager");
        }
        IsLevelLoaded = true;
        OnLevelLoaded?.Invoke();
    }

    private void Reset()
    {
        LoadLevel();
    }

    /*private void OnPlayerClick()
    {
        ComputeEndGameCondition();
    }*/

    private void ComputeEndGameCondition()
    {
        LevelData levelData = LevelManager.Instance.GetCurrentLevel();
        if (levelData.GetDefeatConditions().Any(x => x.IsConditionVerified()))
        {
            State = GameState.Failed;
        }
        else if (levelData.GetSuccessConditions().Count > 0 && levelData.GetSuccessConditions().All(x => x.IsConditionVerified()))
        {
            State = GameState.Won;
        }
    }
}
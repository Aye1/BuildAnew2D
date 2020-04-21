using UnityEngine;
using System.Linq;

// Dependecies to other managers:
//   Hard dependencies:
//     LevelManager
//     TurnManager

public enum GameState { Default, Initializing, Running, Paused, Won, Failed };
public class GameManager : Manager
{
    public static GameManager Instance { get; private set; }

    #region Events
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
            if (_state != value)
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
            State = GameState.Initializing;
            InitState = InitializationState.Initializing;
            SettingsLoader.LoadInitialSettings();
            RegisterCallbacks();
            InitState = InitializationState.Ready;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        UnregisterCallbacks();
    }

    private void RegisterCallbacks()
    {
        InitializationManager.Instance.OnInitStateChanged += CheckInitState;
        LevelManager.OnLevelNeedReset += StartGame;
        TurnManager.OnTurnStart += ComputeEndGameCondition;
    }

    private void UnregisterCallbacks()
    {
        TurnManager.OnTurnStart -= ComputeEndGameCondition;
        LevelManager.OnLevelNeedReset += StartGame;
        InitializationManager.Instance.OnInitStateChanged -= CheckInitState;
    }

    private void CheckInitState(Manager manager, InitializationState state)
    {
        if(state == InitializationState.Initializing)
        {
            State = GameState.Initializing;
        } else if (state == InitializationState.Ready)
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        State = GameState.Running;
        TurnManager.Instance.InitTurns();
    }

    private void PauseGame(bool pause)
    {
        if(pause && State == GameState.Running)
        {
            State = GameState.Paused;
        }
        else if (!pause && State == GameState.Paused)
        {
            State = GameState.Running;
        }
    }

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
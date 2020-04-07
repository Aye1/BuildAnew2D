using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public enum GameState { Default, Running, Won, Failed };
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static bool IsGameReady;
    public static bool IsLevelLoaded;

    private LevelData _levelData;
    private bool _shouldRaiseGameReady;

    #region Events
    public delegate void LevelLoaded();
    public static event LevelLoaded OnLevelLoaded;

    public delegate void GameReady();
    public static event GameReady OnGameReady;
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
                if(_state == GameState.Failed)
                {
                    TriggerGameOver();
                } else if (_state == GameState.Won)
                {
                    TriggerGameSuccess();
                }
            }
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        RegisterGameReadyCallbacks();
        MouseManager.OnPlayerClick += OnPlayerClick;
        LevelManager.OnLevelNeedReset += Reset;
        TurnManager.OnTurnStart += ComputeEndGameCondition;
        LoadLevel();
        CheckIfGameIsReady();
    }

    private void RegisterGameReadyCallbacks()
    {
        TilesDataManager.OnTilesLoaded += CheckIfGameIsReady;
        WaterClusterManager.OnClustersCreated += CheckIfGameIsReady;
    }

    private void UnregisterGameReadyCallbacks()
    {
        TilesDataManager.OnTilesLoaded -= CheckIfGameIsReady;
        WaterClusterManager.OnClustersCreated -= CheckIfGameIsReady;
    }

    private void CheckIfGameIsReady()
    {
        bool gameReady = IsLevelLoaded;
        gameReady &= TilesDataManager.AreTileLoaded;
        gameReady &= WaterClusterManager.AreClustersCreated;
        if(gameReady && _shouldRaiseGameReady)
        {
            IsGameReady = true;
            OnGameReady?.Invoke();
            _shouldRaiseGameReady = false;
        }
    }

    public LevelData GetLevelData()
    {
        return _levelData;
    }

    public void LoadLevel()
    {
        _shouldRaiseGameReady = true;
        IsLevelLoaded = false;
        _levelData = LevelManager.Instance.GetCurrentLevel();
        if (_levelData != null)
        {
            ResourcesManager.Instance.InitializeResources(_levelData.GetInitialResources());
        }
        else
        {
            Debug.LogWarning("Missing level data into GameManager");
        }
        IsLevelLoaded = true;
        OnLevelLoaded?.Invoke();
    }

    private void Reset()
    {
        LoadLevel();
        UIManager.Instance.ResetUI();
    }

    private void OnPlayerClick()
    {
        ComputeEndGameCondition();
    }

    public void NextTurn()
    {
        TurnManager.Instance.NextTurn();
        ComputeEndGameCondition();        
    }

    private void ComputeEndGameCondition()
    {
        if (_levelData.GetDefeatConditions().Any(x => x.IsConditionVerified()))
        {
            //TriggerGameOver();
            State = GameState.Failed;
        }
        else if (_levelData.GetSuccessConditions().Count > 0 && _levelData.GetSuccessConditions().All(x => x.IsConditionVerified()))
        {
            //TriggerGameSuccess();
            State = GameState.Won;
        }
    }

    private void TriggerGameOver()
    {
        UIManager.Instance.TriggerEndGame();
    }

    private void TriggerGameSuccess()
    {
        UIManager.Instance.TriggerEndGame();
    }
}